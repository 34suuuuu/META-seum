using System;
using UnityEngine;

namespace Game.Scripts.Packet
{
    [Serializable]
    public class SerializableVector3
    {
        public SerializableVector3()
        {
            
        }
        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
        public float x, y, z;

        public Vector3 toVector3()
        {
            Vector3 newVector = new Vector3(x, y, z);
            return newVector;
        }
    }
}