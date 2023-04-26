using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using StarterAssets.Packet;

namespace RoomServer
{
    public class UDPRoom1Server
    {
        private int port;
        private Socket server;
        public UDPRoom1Server(int port)
        {
            this.port = port;
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            BeginReceive();
        }

        public void Start()
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
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
            packet.source = "server";
            packet.dest = "client";
            Console.WriteLine("Received packet from {0}:{1}", remoteEP.Address, remoteEP.Port);
            Console.WriteLine(packet.packetNum);
            byte[] serializedPacket = PacketSerializer.Serializer(packet);
            server.SendTo(serializedPacket, remoteEP); // 8080 server
            Console.WriteLine("Send Packet to UDPServer!");
        }
    }
}
