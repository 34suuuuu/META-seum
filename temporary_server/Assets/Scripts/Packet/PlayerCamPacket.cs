using System;
using UnityEngine;

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
            x = playerCam.x;
            y = playerCam.y;
            z = playerCam.z;
            w = playerCam.w;
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