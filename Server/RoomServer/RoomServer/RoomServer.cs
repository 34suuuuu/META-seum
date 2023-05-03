using System;

namespace RoomServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            UDPRoom1Server room1Server = new UDPRoom1Server(5050, 6060);
            room1Server.Start();

            Console.ReadLine();
        }
    }
}
