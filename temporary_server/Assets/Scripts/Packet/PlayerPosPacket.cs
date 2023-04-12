using System;
using UnityEngine;

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
            x = playerPos.x;
            y = playerPos.y;
            z = playerPos.z;
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