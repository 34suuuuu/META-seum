using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using StarterAssets;
using StarterAssets.Packet;
using UnityEngine;

namespace StarterAssets
{
    public class NetworkManager : MonoBehaviour
    {
        private string serverIp = StartScript.ipAddressStatic;
        private int port = StartScript.portStatic;

        public IPEndPoint EndPoint;
        public Socket Udp;
        public int id;
        
        //private InputController inputController;


        // Start is called before the first frame update
        void Start()
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
            Udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //inputController = GetComponent<InputController>();
            while (Udp.Available != 0)
            {
                byte[] buf = new byte[1024];
                Udp.Receive(buf);

                PacketDatagram pd = (PacketDatagram) PacketSerializer.Deserializer(buf);
                Debug.Log(pd.playerInfoPacket.id);

                if (InputController.packetTime.ContainsKey(pd.packetNum))
                {
                    float latency = Time.time - InputController.packetTime[pd.packetNum];
                    CapsuleGenerator.realTime = latency;
                    CapsuleGenerator.elapsedTimeSum += latency;
                    CapsuleGenerator.elapsedTimeCount++;

                    if (latency < CapsuleGenerator.minLatency && latency > 0)
                        CapsuleGenerator.minLatency = latency;
                    if (latency > CapsuleGenerator.maxLatency)
                        CapsuleGenerator.maxLatency = latency;
                }
            }
        }

        public void SendPacket(PacketDatagram pd, EndPoint ep = null)
        {
            byte[] packet = PacketSerializer.Serializer(pd);
            if (ep == null)
                Udp.SendTo(packet, EndPoint);
            else
                Udp.SendTo(packet, ep);
        }

        private void OnApplicationQuit()
        {
            Udp.Close();
        }
    }
}
