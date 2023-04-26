using System;

namespace StarterAssets.Packet
{
    [Serializable]
    public class PacketDatagram
    {
        public PacketDatagram()
        {
            
        }
        public string source { get; set; }
        public string dest { get; set; }
        public int packetNum { get; set; }
        public string status { get; set; }
        
        public PlayerCamPacket playerCamPacket { get; set; }
        public PlayerInfoPacket playerInfoPacket { get; set; }
        public PlayerPosPacket playerPosPacket { get; set; }

        public string toString()
        {
            return $"status: {status}" +
                   $"info: {playerInfoPacket}" +
                   $"pos: {playerPosPacket}" +
                   $"cam: {playerCamPacket}";
        }
    }
}