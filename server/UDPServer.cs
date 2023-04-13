using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class UDPServer // udp 소켓 생성 및 수신한 패킷 처리
    {
        private int port;
        private Socket server;

        public UDPServer(int port)
        {
            this.port = port;
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        }

        public void Start() // 새로운 소켓 생성, 로컬엔드포인트에 바인딩 -> 수신대기 상태
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
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

                ClientPacket packet = ClientPacket.Deserialize(buffer); // 수신한 패킷을 역직렬화
                HandlePacket(packet, (IPEndPoint)clientEP);

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

        private void HandlePacket(ClientPacket packet, IPEndPoint remoteEP) // 수신한 패킷을 출력 & 추후 처리
        {
            // Extract the packet information and display it
            Console.WriteLine("Received packet from {0}:{1}", remoteEP.Address, remoteEP.Port);
            //Console.WriteLine("Packet type: {0}", packet.PacketType);
            //Console.WriteLine("Input action asset: {0}", packet.InputActionAsset);
            //Console.WriteLine("Packet number: {0}", packet.PacketNum);
            //Console.WriteLine("ID: {0}", packet.ID);

            // Do something with the packet data
            // ...
        }
        //public void CheckPacket(byte[] byte_)
        //{
            
        //}
    }
}
