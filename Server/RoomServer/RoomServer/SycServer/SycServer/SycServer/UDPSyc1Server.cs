using System;
using System.Net;
using System.Net.Sockets;
using StarterAssets.Packet;


namespace SycServer
{
    public class UDPSyc1Server
    {
        private int port;
        private Socket server;
        private IPAddress serverIP;

        public UDPSyc1Server(int port)
        {
            this.port = port;
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            BeginReceive();
        }

        public void Start()
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
            serverIP = IPAddress.Parse("127.0.0.1");
            server.Bind(localEP);
            Console.WriteLine("Syc1 Server Start!");

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
            byte[] serializedPacket;
            packet.source = "sycServer";
            packet.dest = "roomServer";
            Console.WriteLine("Received packet from {0}:{1}", remoteEP.Address, remoteEP.Port);
            Console.WriteLine(packet.packetNum);
            serializedPacket = PacketSerializer.Serializer(packet);
            server.SendTo(serializedPacket, new IPEndPoint(serverIP, 5050));
            Console.WriteLine("Send Packet to RoomServer!");
        }
    }
}
