using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using StarterAssets.Packet;

namespace Server
{
    public class UDPRoom1Server
    {
        private int port = 5051;
        private int syc1Port = 6061;
        private int syc2Port = 6062;
        private int syc3Port = 6063;
        private IPEndPoint sycServer1EP, sycServer2EP, sycServer3EP;
        private Socket udp;
        private IPAddress ip;
        private Dictionary<EndPoint, Client> clients;
        private Dictionary<EndPoint, int> weights;
        private int idAssignIndex = 0;

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
            sycServer1EP = new IPEndPoint(ip, syc1Port);
            sycServer2EP = new IPEndPoint(ip, syc2Port);
            sycServer3EP = new IPEndPoint(ip, syc3Port);
            weights.Add(sycServer1EP, 0);
            weights.Add(sycServer2EP, 0);
            weights.Add(sycServer3EP, 0);
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
                if (packet.status == "request")
                {
                    if (packet.source == "client" && packet.dest == "server")
                    {
                        HandleNewClient(ref packet);
                    }
                    //HandleNewClient(ref packet);
                }
                else if (packet.status == "quit")
                {
                    DisconnectClient(ref packet);
                }
                else if (packet.status == "connected")
                {
                    HandleConnectedClient(ref packet, clientEP);
                    //int packetId = packet.playerInfoPacket.id;
                    //int seqNum = packet.packetNum;
                    //Console.WriteLine(packet.packetNum);
                    //if (packetId == -1 || seqNum == -1)
                    //    return;
                    //HandleUserMoveInput(packet.portNum, ref packet, seqNum);
                }
            }
            BeginReceive();
        }


        private void HandleNewClient(ref PacketDatagram pd)
        {
            pd.packetNum = 0;
            pd.playerInfoPacket.group = 3;
            pd.status = "request";
            pd.playerInfoPacket.id = idAssignIndex++;

            EndPoint minSycEP = ReturnEndPoint();
            Console.WriteLine("{0} SycServer - minWeight: {1}", minSycEP, weights[minSycEP]);

            CalcWeight(1, minSycEP, ref pd);

            Console.WriteLine("{0} SycServer - minWeight: {1}  <-- After Calc", minSycEP, weights[minSycEP]);
            SendPacket(ref pd, minSycEP);
        }


        private void HandleConnectedClient(ref PacketDatagram pd, EndPoint remoteEP)
        {
            IPEndPoint clientEP = new IPEndPoint(ip, pd.portNum);
            if (pd.source == "server" && pd.dest == "client" && pd.packetNum == 0)
            {
                if (!clients.ContainsKey(clientEP))
                {
                    clients.Add(clientEP, new Client(pd.playerInfoPacket.id, pd));
                }
                CalcWeight(-1, remoteEP, ref pd);

                BroadcastToNewClient(ref pd);
                SendPositionToAllClients(ref pd);
            }
            else
            {
                int packetId = pd.playerInfoPacket.id;
                int seqNum = pd.packetNum;
                if (packetId == -1 || seqNum == -1)
                    return;
                if (pd.source == "client" && pd.dest == "server")
                {
                    EndPoint minSycEP = ReturnEndPoint();
                    Console.WriteLine("{0} SycServer - minWeight: {1}", minSycEP, weights[minSycEP]);

                    CalcWeight(1, minSycEP, ref pd);

                    Console.WriteLine("{0} SycServer - minWeight: {1}  <-- After Calc", minSycEP, weights[minSycEP]);
                    SendPacket(ref pd, sycServer1EP);
                }
                else if (pd.source == "server" && pd.dest == "client")
                {
                    CalcWeight(-1, remoteEP, ref pd);
                    Console.WriteLine("{0} SycServer - minWeight: {1}  <-- After minus Calc", remoteEP, weights[remoteEP]);
                    HandleUserMoveInput(pd.portNum, ref pd, seqNum);
                }
            }
        }

        //private void HandleNewClient(ref PacketDatagram pd)
        //{
        //    IPEndPoint clientEP = new IPEndPoint(ip, pd.portNum);
        //    pd.packetNum = 0;
        //    pd.status = "request";
        //    pd.playerInfoPacket.id = idAssignIndex++;

        //    SendPacket(ref pd, clientEP);

        //    //SendPacket(ref pd, new IPEndPoint(ip, syc1Port));
        //    pd.status = "connected";
        //    clients.Add(clientEP, new Client(pd.playerInfoPacket.id, pd));
        //    BroadcastToNewClient(ref pd);
        //    //BroadcastAllPositions();
        //    SendPositionToAllClients(ref pd);
        //}

        void BroadcastToNewClient(ref PacketDatagram pd)
        {
            foreach (KeyValuePair<EndPoint, Client> client in clients)
            {
                SendPacket(ref client.Value.pd, new IPEndPoint(ip, pd.portNum)); // 기존 패킷 to New Client
            }
        }

        //void BroadcastAllPositions()
        //{
        //    foreach (KeyValuePair<EndPoint, Client> p1 in clients)
        //    {
        //        foreach (KeyValuePair<EndPoint, Client> p2 in clients)
        //        {
        //            SendPacket(ref p1.Value.pd, p2.Key);
        //        }
        //    }
        //}

        void SendPositionToAllClients(ref PacketDatagram pd)
        {
            foreach (KeyValuePair<EndPoint, Client> p in clients)
            {
                SendPacket(ref pd, p.Key);
            }
        }

        void DisconnectClient(ref PacketDatagram pd)
        {
            IPEndPoint clientEP = new IPEndPoint(ip, pd.portNum);
            Console.WriteLine($"id:{pd.playerInfoPacket.id}, disconnect client");
            if (clients.ContainsKey(clientEP))
                clients.Remove(clientEP);
            Broadcast(ref pd);
        }
        void Broadcast(ref PacketDatagram pd)
        {
            foreach (KeyValuePair<EndPoint, Client> p in clients)
                SendPacket(ref pd, p.Key);
        }

        void HandleUserMoveInput(int client, ref PacketDatagram pd, int seqNumber)
        {
            IPEndPoint clientEP = new IPEndPoint(ip, client);
            if (!clients.ContainsKey(clientEP) || clients[clientEP].lastSeqNumber > seqNumber)
                return;

            if (!clients[clientEP].history.ContainsKey(seqNumber))
            {
                clients[clientEP].UpdateStateHistory(seqNumber);
                clients[clientEP].lastSeqNumber = seqNumber;
            }
            UpdatePosition(clientEP, ref pd);
            /* so that clients see newly connected clients */
            SendPositionToAllClients(ref pd);
        }

        void UpdatePosition(EndPoint addr, ref PacketDatagram pd)
        {
            //Debug.Log($"packetId: {pd.playerInfoPacket.id}" +
            //          $"position: {pd.playerPosPacket.toString()}");
            clients[addr].pd = pd;
            clients[addr].pos.x = pd.playerPosPacket.x;
            clients[addr].pos.y = pd.playerPosPacket.y;
            clients[addr].pos.z = pd.playerPosPacket.z;
            clients[addr].cam.x = pd.playerCamPacket.x;
            clients[addr].cam.y = pd.playerCamPacket.y;
            clients[addr].cam.z = pd.playerCamPacket.z;
            clients[addr].cam.w = pd.playerCamPacket.w;
        }

        private void SendPacket(ref PacketDatagram pd, EndPoint addr)
        {
            byte[] packet = PacketSerializer.Serializer(pd);
            udp.SendTo(packet, addr);
        }

        EndPoint ReturnEndPoint()
        {
            int minWeight = 999;
            EndPoint minSycEP = sycServer1EP;
            foreach (KeyValuePair<EndPoint, int> w in weights)
            {
                if (minWeight > w.Value)
                {
                    minWeight = w.Value;
                    minSycEP = w.Key;
                }
            }
            return minSycEP;
        }

        void CalcWeight(int addOrSub, EndPoint SycEp, ref PacketDatagram pd)
        {
            int groupId = pd.playerInfoPacket.group;
            int weight = groupId == 1 ? group1Weight : groupId == 2 ? group2Weight : group3Weight;
            if (addOrSub == 1)
                weights[SycEp] += weight;
            else if (addOrSub == -1)
                weights[SycEp] -= weight;
        }
    }
}