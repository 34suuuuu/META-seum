using System;
using System.Collections.Generic;
using System.Net;
using StarterAssets;
using StarterAssets.Packet;
using UnityEngine;
using Random = UnityEngine.Random;


public class InputController : MonoBehaviour
{
    private NetworkManager networkManager;
    private PacketDatagram userDatagram;
    private Vector3 playerPos;
    private int packetNum;
    public static Dictionary<int, float> packetTime;

    //public static Queue<float> times;
    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        userDatagram = new PacketDatagram();
        packetNum = 0;
        //packetTime = new Dictionary<int, float>();
    }

    private void Start()
    {
        //times = new Queue<float>();
        packetTime = new Dictionary<int, float>();
        PacketDatagram pd = new PacketDatagram();
        
        pd.packetNum = -999;
        pd.status = "request";
        pd.source = "client";
        pd.dest = "server";
        pd.playerPosPacket = new PlayerPosPacket(NetworkUtility.ChangeVector3Package(transform.position));
        pd.playerCamPacket = new PlayerCamPacket(NetworkUtility.ChangeQuaternionPackage(transform.rotation));
        pd.playerInfoPacket = new PlayerInfoPacket();

        IPEndPoint syncEp1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6061);
        IPEndPoint syncEp2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6062);
        IPEndPoint syncEp3 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6063);
        
        networkManager.SendPacket(pd);
        networkManager.SendPacket(pd, syncEp1);
        networkManager.SendPacket(pd, syncEp2);
        networkManager.SendPacket(pd, syncEp3);
    }

    private void FixedUpdate()
    {
        if ((transform.position - playerPos).sqrMagnitude > 0.00001f)
        {
            //packetTime.Add(packetNum, Time.time);
            
            playerPos = transform.position;
            userDatagram.packetNum = packetNum++;
            userDatagram.status = "connected";
            userDatagram.source = "client";
            userDatagram.dest = "server";
            userDatagram.playerPosPacket = new PlayerPosPacket(NetworkUtility.ChangeVector3Package(transform.position));
            userDatagram.playerCamPacket = new PlayerCamPacket(NetworkUtility.ChangeQuaternionPackage(transform.rotation));
            userDatagram.playerInfoPacket = new PlayerInfoPacket
                {
                    id = networkManager.id,
                    group = StartScript.groupIdStatic,
                    roomNum = StartScript.roomIdStatic
                };

            packetTime.Add(userDatagram.packetNum, Time.time);
            //Debug.Log("Send:" + userDatagram.packetNum + "," + Time.time);
            networkManager.SendPacket(userDatagram);
        }
    }

    private void OnApplicationQuit()
    {
        PacketDatagram pd = new PacketDatagram();
        
        pd.packetNum = -999;
        pd.status = "quit";
        pd.source = "client";
        pd.dest = "server";
        pd.playerPosPacket = new PlayerPosPacket();
        pd.playerCamPacket = new PlayerCamPacket();
        pd.playerInfoPacket = new PlayerInfoPacket();
        pd.playerInfoPacket.id = networkManager.id;
        
        networkManager.SendPacket(pd);
    }
}