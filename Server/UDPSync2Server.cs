using System;
using System.Net;
using System.Net.Sockets;
using StarterAssets.Packet;


namespace Server
{
    public class UDPSync2Server
    {
        private int port = 6062;
        private int room1Port = 5051;
        private Socket udp;
        private IPAddress ip;
        private IPEndPoint room1ServerEP;
        public UDPSync2Server()
        {
            udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            BeginReceive();
        }

        public void Start()
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
            ip = IPAddress.Parse("127.0.0.1");
            room1ServerEP = new IPEndPoint(ip, room1Port);
            udp.Bind(localEP);
            Console.WriteLine("Sync2 Server Start!");

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
                HandlePacket(ref packet, (IPEndPoint)clientEP);
            }
            BeginReceive();
        }

        private void HandlePacket(ref PacketDatagram packet, IPEndPoint remoteEP)
        {
            if (packet.status.Equals("request"))
            {
                SendPacket(ref packet, new IPEndPoint(ip, packet.portNum));
                packet.status = "connected";

            }
            packet.source = "server";
            packet.dest = "client";
            SendPacket(ref packet, room1ServerEP); // 나중에 고쳐야됨
            Console.WriteLine("Sync2 ------ Send Packet to RoomServer!\n");
        }

        private void SendPacket(ref PacketDatagram pd, EndPoint addr)
        {
            byte[] packet = PacketSerializer.Serializer(pd);
            udp.SendTo(packet, addr);
        }
    }
}

