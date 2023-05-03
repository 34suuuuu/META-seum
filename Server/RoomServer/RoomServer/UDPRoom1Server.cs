using System;
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

        private void HandlePacket(ref PacketDatagram packet, IPEndPoint remoteEP)
        {
            Console.WriteLine("Received packet from {0}:{1}", remoteEP.Address, remoteEP.Port);
            Console.WriteLine("user id :{0}, user name :{1}, group id :{2}, source :{3}", packet.playerInfoPacket.id, packet.playerInfoPacket.playerName, packet.playerInfoPacket.group, packet.source);
            byte[] serializedPacket;
            if (packet.source.Equals("client") && packet.dest.Equals("server"))
            {
                packet.source = "roomServer";
                packet.dest = "sycServer";
                serializedPacket = PacketSerializer.Serializer(packet);
                server.SendTo(serializedPacket, sycServer1EP);
                Console.WriteLine("Send Packet to Syc1!");
            }
            else if (packet.source.Equals("sycServer") && packet.dest.Equals("roomServer"))
            {
                packet.source = "roomServer";
                packet.dest = "server";
                serializedPacket = PacketSerializer.Serializer(packet);
                server.SendTo(serializedPacket, new IPEndPoint(serverIP, 8080));
                Console.WriteLine("Send Packet to Server!");

            }
        }
    }
}
