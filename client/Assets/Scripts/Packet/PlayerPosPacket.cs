using System;
using System.Numerics;

namespace StarterAssets.Packet
{
    [Serializable]
    public class PlayerPosPacket
    {
        public PlayerPosPacket()
        {
            
        }
        public PlayerPosPacket(Vector3 playerPos)
        {
            x = playerPos.X;
            y = playerPos.Y;
            z = playerPos.Z;
        }
        
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Vector3 toVector3()
        {
            return new Vector3(x, y, z);
        }

        public string toString()
        {
            return $"x: {x} y: {y} z: {z}";
        }
    }
}