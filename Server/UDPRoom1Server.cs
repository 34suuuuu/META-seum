using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using StarterAssets.Packet;

namespace Server
{
    public class UDPRoom1Server
    {
        private int port = 8080;
        private int sync1Port = 6061;
        private int sync2Port = 6062;
        private int sync3Port = 6063;
        private IPEndPoint syncServer1EP, syncServer2EP, syncServer3EP;
        private Socket udp;
        private IPAddress ip;
        private Dictionary<EndPoint, Client> clients;
        private Dictionary<EndPoint, int> weights;

        private const int group1Weight = 1;
        private const int group2Weight = 3;
        private const int group3Weight = 5;


        public UDPRoom1Server()
        {
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            clients = new Dictionary<EndPoint, Client>();
            weights = new Dictionary<EndPoint, int>();
            ip = IPAddress.Parse("127.0.0.1");

            BeginReceive();
        }

        public void Start()
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
            syncServer1EP = new IPEndPoint(ip, sync1Port);
            syncServer2EP = new IPEndPoint(ip, sync2Port);
            syncServer3EP = new IPEndPoint(ip, sync3Port);

            weights.Add(syncServer1EP, 0);
            weights.Add(syncServer2EP, 0);
            weights.Add(syncServer3EP, 0);
            udp.Bind(localEP);

            Console.WriteLine("Room1 Server Start!");

        }

        private void BeginReceive()
        {
            byte[] buffer = new byte[1024];
            EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
            udp.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref clientEP, new AsyncCallback(OnReceive), buffer);

        }

        private void OnReceive(IAsyncResult ar)
        {
            byte[] buffer = (byte[])ar.AsyncState;
            EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
            int bytesRead = udp.EndReceiveFrom(ar, ref clientEP);

            PacketDatagram packet = PacketSerializer.Deserializer(buffer) as PacketDatagram;
            if (packet != null)
            {
                HandlePacket(ref packet, clientEP);
            }
            BeginReceive();
        }

        private void HandlePacket(ref PacketDatagram pd, EndPoint clientEP)
        {
            if (!clients.ContainsKey(clientEP))
            {
                clients.Add(clientEP, new Client(pd.playerInfoPacket.id, pd));
            }
            if (pd.status.Equals("connected"))
            {
                EndPoint minEndPoint = ReturnEndPoint();
                CalcWeight(1, minEndPoint, ref pd);
                SendPacket(ref pd, minEndPoint);
            }
        }



        private void SendPacket(ref PacketDatagram pd, EndPoint addr)
        {
            byte[] packet = PacketSerializer.Serializer(pd);
            udp.SendTo(packet, addr);
        }

        EndPoint ReturnEndPoint()
        {
            int minWeight = 999;
            EndPoint minSyncEP = syncServer1EP;
            foreach (KeyValuePair<EndPoint, int> w in weights)
            {
                if (minWeight > w.Value)
                {
                    minWeight = w.Value;
                    minSyncEP = w.Key;
                }
            }
            return minSyncEP;
        }

        void CalcWeight(int addOrSub, EndPoint SyncEp, ref PacketDatagram pd)
        {
            int groupId = pd.playerInfoPacket.group;
            int weight = groupId == 1 ? group1Weight : groupId == 2 ? group2Weight : group3Weight;
            if (addOrSub == 1)
                weights[SyncEp] += weight;
            else if (addOrSub == -1)
                weights[SyncEp] -= weight;
        }
    }
}