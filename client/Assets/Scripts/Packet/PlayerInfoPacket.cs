using System;

namespace StarterAssets.Packet
{
    [Serializable]
    public class PlayerInfoPacket
    {
        public PlayerInfoPacket()
        {
            
        }
        
        public int id { get; set; }
        public string playerName { get; set; }
        public int group { get; set; }
        public int roomNum { get; set; }
    }
}