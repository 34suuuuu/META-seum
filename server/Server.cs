using System;
using System.Net;
using System.Net.Sockets;
namespace Server
{
    class Server
    {
        public static void Main(string[] args)
        {
            UDPServer server = new UDPServer(5055);
            server.Start();

            Console.ReadLine();
        }
    }
}
