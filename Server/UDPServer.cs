using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using StarterAssets.Packet;

namespace Server
{
    public class UDPServer // udp 소켓 생성 및 수신한 패킷 처리
    {
        private int port, roomPort1;
        private Socket server;
        private IPAddress serverIP;
        private IPEndPoint roomServer1EP;
        public UDPServer(int port, int roomPort1)
        {
            this.port = port;
            this.roomPort1 = roomPort1;
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        }

        public void Start() // 새로운 소켓 생성, 로컬엔드포인트에 바인딩 -> 수신대기 상태
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
            serverIP = IPAddress.Parse("127.0.0.1");
            roomServer1EP = new IPEndPoint(serverIP, roomPort1);
            server.Bind(localEP);
            Console.WriteLine("Server Start!");
            BeginReceive(); // 비동기적으로 패킷 수신
        }

        private void BeginReceive()
        {
            byte[] buffer = new byte[1024]; // 새로운 바이트배열 버퍼
            EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
            server.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref clientEP, new AsyncCallback(OnReceive), buffer);
            // 비동기적으로 수신하기 위해 beginreceivefrom 사용
        }

        private void OnReceive(IAsyncResult ar) // 비동기적으로 수신된 패킷을 처리하는 콜백 함수, ar = BeginReceiveFrom() 호출로 부터 반환된 객체
        {
            try
            {
                byte[] buffer = (byte[])ar.AsyncState;
                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                int bytesRead = server.EndReceiveFrom(ar, ref clientEP);

                PacketDatagram packet = PacketSerializer.Deserializer(buffer) as PacketDatagram; // 수신한 패킷을 역직렬화
                
                if (packet != null)
                {
                    HandlePacket(ref packet, (IPEndPoint)clientEP);
                }
                else
                {
                    Console.WriteLine("TypeCast Error!");
                }
                BeginReceive(); // 다음 패킷을 수신하기 위해
            }
            catch (SocketException ex) // 소켓에러 발생했을때
            {
                Console.WriteLine("SocketException: {0}", ex.Message);

            }
            catch (ObjectDisposedException ex) // 소켓이 없는데 실행하려고 할때 예외
            {
                Console.WriteLine("ObjectDisposedException: {0}", ex.Message);
            }
            catch (Exception ex) // 프로그램 실행 중 발생하는 에러
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
        }

        private void HandlePacket(ref PacketDatagram packet, IPEndPoint remoteEP) // 수신한 패킷을 출력 & 추후 처리
            //패킷을 받아서 패킷에 있는 그룹id를 통해 가중치를 계산하고, 위치 동기화 시키는 서버에 넘겨줌
            //서버 정보도 저장해야함
        {
            Console.WriteLine("Received packet from {0}:{1}", remoteEP.Address, remoteEP.Port);
            Console.WriteLine("user id :{0}, user name :{1}, group id :{2}, source :{3}", packet.playerInfoPacket.id, packet.playerInfoPacket.playerName, packet.playerInfoPacket.group, packet.source);
            //int groupWeight = packet.playerInfoPacket.group == 1 ? 5 : packet.playerInfoPacket.group == 2 ? 3 : 1;
            // room id가 있으면 room id로 함
            // room id가 없어서 일단 group id를 보고 이동
            byte[] serializedPacket;
            //server.SendTo(groupPacket, remoteEP);
            if (packet.source.Equals("client") && packet.dest.Equals("server"))
            {
                packet.portNum = remoteEP.Port;
                serializedPacket = PacketSerializer.Serializer(packet);
                server.SendTo(serializedPacket, roomServer1EP);
                Console.WriteLine("Send Packet to Room1!");
            }
            else if (packet.source.Equals("roomServer") && packet.dest.Equals("server"))
            {
                serializedPacket = PacketSerializer.Serializer(packet);
                server.SendTo(serializedPacket, new IPEndPoint(serverIP, packet.portNum));
                Console.WriteLine("Send Packet to Client!");

            }
            
        }
       
        //public void CheckPacket(byte[] byte_)
        //{
            
        //}
    }


}
