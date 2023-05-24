using System;

namespace Server
{
    class Server
    {
        public static void Main(string[] args)
        {
            UDPServer server = new UDPServer();
            UDPRoom1Server room1Server = new UDPRoom1Server();
            UDPSyc1Server syc1Server = new UDPSyc1Server();
            UDPSyc2Server syc2Server = new UDPSyc2Server();
            UDPSyc3Server syc3Server = new UDPSyc3Server();

            server.Start();
            room1Server.Start();
            syc1Server.Start();
            syc2Server.Start();
            syc3Server.Start();
            

            Console.ReadLine();
        }
    }
}
