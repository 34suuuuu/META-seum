using System;

namespace SycServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            UDPSyc1Server syc1Server = new UDPSyc1Server(6060);
            syc1Server.Start();

            Console.ReadLine();
        }
    }
}
