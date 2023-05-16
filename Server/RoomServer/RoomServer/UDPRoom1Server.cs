using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using StarterAssets.Packet;

namespace RoomServer
{
    public class UDPRoom1Server
    {
        private int port, sycPort1;
        private Socket server;
        private IPAddress serverIP;
        private IPEndPoint sycServer1EP;

        LoadBalancer loadBalancer = new LoadBalancer();
  

        public UDPRoom1Server(int port, int sycPort1)
        {
            this.port = port;
            this.sycPort1 = sycPort1;
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            BeginReceive();
        }

        public void Start()
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
            serverIP = IPAddress.Parse("127.0.0.1");
            sycServer1EP = new IPEndPoint(serverIP, sycPort1);
            server.Bind(localEP);

            loadBalancer.AddServer(new ConnectedServer(new IPEndPoint(serverIP, 6060), 13));
            loadBalancer.AddServer(new ConnectedServer(new IPEndPoint(serverIP, 1111), 29));

            Console.WriteLine("Room1 Server Start!");
            
        }

        private void BeginReceive()
        {
            byte[] buffer = new byte[1024];
            EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
            server.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref clientEP, new AsyncCallback(OnReceive), buffer);
            
        }

        private void OnReceive(IAsyncResult ar)
        {
            byte[] buffer = (byte[])ar.AsyncState;
            EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
            int bytesRead = server.EndReceiveFrom(ar, ref clientEP);

            PacketDatagram packet = PacketSerializer.Deserializer(buffer) as PacketDatagram;
            if (packet != null)
            {
                HandlePacket(ref packet, (IPEndPoint)clientEP);
            }
            BeginReceive();
        }

        //private void HandlePacket(ref PacketDatagram packet, IPEndPoint remoteEP)
        //{
        //    Console.WriteLine("Received packet from {0}:{1}", remoteEP.Address, remoteEP.Port);
        //    Console.WriteLine("user id :{0}, user name :{1}, group id :{2}, source :{3}", packet.playerInfoPacket.id, packet.playerInfoPacket.playerName, packet.playerInfoPacket.group, packet.source);
        //    byte[] serializedPacket;
        //    if (packet.source.Equals("client") && packet.dest.Equals("server"))
        //    {
        //        packet.source = "roomServer";
        //        packet.dest = "sycServer";
        //        serializedPacket = PacketSerializer.Serializer(packet);
        //        server.SendTo(serializedPacket, sycServer1EP);
        //        Console.WriteLine("Send Packet to Syc1!");
        //    }
        //    else if (packet.source.Equals("sycServer") && packet.dest.Equals("roomServer"))
        //    {
        //        packet.source = "roomServer";
        //        packet.dest = "server";
        //        serializedPacket = PacketSerializer.Serializer(packet);
        //        server.SendTo(serializedPacket, new IPEndPoint(serverIP, 8080));
        //        Console.WriteLine("Send Packet to Server!");

        //    }
        //}

        private void HandlePacket(ref PacketDatagram packet, IPEndPoint remoteEP)
        {
            Console.WriteLine("Received packet from {0}:{1}", remoteEP.Address, remoteEP.Port);
            Console.WriteLine("user id :{0}, user name :{1}, group id :{2}, source :{3}", packet.playerInfoPacket.id, packet.playerInfoPacket.playerName, packet.playerInfoPacket.group, packet.source);
            byte[] serializedPacket;
            IPEndPoint newEP;

            //loadBalancer.print();
            //Console.WriteLine(loadBalancer.GetMinWeightServer());
            Console.WriteLine("before weight: {0}", loadBalancer.GetMinWeight());

            if (packet.source.Equals("client") && packet.dest.Equals("server"))
            {
                packet.source = "roomServer";
                packet.dest = "sycServer";
                packet.playerInfoPacket.group = 1; // 임의로 추가해봄
                serializedPacket = PacketSerializer.Serializer(packet);

                if (loadBalancer.servers.Count == 0) //sync server가 하나도 없으면
                {

                    newEP = new IPEndPoint(IPAddress.Any, 6060);
                    loadBalancer.AddServer(new ConnectedServer(newEP, 0));

                    server.SendTo(serializedPacket, newEP);
                    Console.WriteLine("Send Packet to new Syc1!");

                }
                else
                {
                    newEP = loadBalancer.GetMinWeightServer();

                    server.SendTo(serializedPacket, newEP);
                    Console.WriteLine("Send Packet to minimum Syc1!");
                }
            

            }
            else if (packet.source.Equals("sycServer") && packet.dest.Equals("roomServer"))
            {
                packet.source = "roomServer";
                packet.dest = "server";
                serializedPacket = PacketSerializer.Serializer(packet);
                server.SendTo(serializedPacket, new IPEndPoint(serverIP, 8080));
                Console.WriteLine("Send Packet to Server!");

            }

            loadBalancer.calcWeight(ref packet); ;
            Console.WriteLine("after weight: {0}\n", loadBalancer.GetMinWeight());
        }

    }

    public class ConnectedServer
    {
        public IPEndPoint EndPoint { get; set; }
        public int weight { get; set; }

        ConnectedServer() { }

        public ConnectedServer(IPEndPoint EndPoint, int weight)
        {
            this.EndPoint = EndPoint;
            this.weight = weight;
        }
    }


    public class LoadBalancer
    {
        public List<ConnectedServer> serverGroups;
        public bool flag = true;

        public LoadBalancer() { }
 
        public LoadBalancer(List<ConnectedServer> serverGroups)
        {
            this.serverGroups = serverGroups;
        }

        public SortedSet<ConnectedServer> servers = new SortedSet<ConnectedServer>(
            Comparer<ConnectedServer>.Create((x, y) =>
            {
                if (x.weight == y.weight) return x.EndPoint.ToString().CompareTo(y.EndPoint.ToString());
                return x.weight.CompareTo(y.weight);
            }));

        public void AddServer(ConnectedServer connectedServer)
        {
            servers.Add(connectedServer);

        }

        // 최소 가중치 서버의 IPEndPoint 리턴
        public IPEndPoint GetMinWeightServer()
        {
            if (servers.Count == 0) throw new InvalidOperationException("No servers available");
            return servers.Min.EndPoint;
        }

        public int GetMinWeight()
        {
            foreach (var ConnectedServer in servers)
            {
                if (ConnectedServer.EndPoint == servers.Min.EndPoint) return ConnectedServer.weight;
            }
            return -1;
        }


        public void calcWeight(ref PacketDatagram packet)
        {
            int weight = packet.playerInfoPacket.group == 1 ? 5 : packet.playerInfoPacket.group == 2 ? 3 : 1;
            //int weight = 3;
            if (packet.source.Equals("roomServer") && packet.dest.Equals("server"))
            {
                servers.Min.weight += weight;
            }
            else if (packet.source.Equals("roomServer") && packet.dest.Equals("sycServer"))
            {
                servers.Min.weight -= weight;
            }
        }


        public void print()
        {
            foreach(var ConnectedServer in servers)
            {
                Console.WriteLine("IPEndPoint: {0}, Weight: {1}", ConnectedServer.EndPoint, ConnectedServer.weight);

            }
        }

    }
  
}
