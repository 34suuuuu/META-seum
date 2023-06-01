using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace StarterAssets.Packet
{
    public class PacketSerializer
    {
        public static byte[] Serializer(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Object Deserializer(byte[] packet)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            ms.Write(packet, 0, packet.Length);
            ms.Seek(0, SeekOrigin.Begin);
            return bf.Deserialize(ms);
        }
    }
}