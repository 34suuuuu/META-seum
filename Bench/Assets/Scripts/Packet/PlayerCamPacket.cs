using System;
using System.Numerics;

namespace StarterAssets.Packet
{
    [Serializable]
    public class PlayerCamPacket
    {
        public PlayerCamPacket()
        {
            
        }
        public PlayerCamPacket(Quaternion playerCam)
        {
            x = playerCam.X;
            y = playerCam.Y;
            z = playerCam.Z;
            w = playerCam.W;
        }
        
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        public Quaternion toQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }

        public string toString()
        {
            return $"x: {x} y: {y} z: {z} w: {w}";
        }
    }
}