using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game.Scripts.Packet
{
    [Serializable]
    public class ClientPacket
    {
        public ClientPacket(PacketType _packetType,
            SerializableVector3 _playerPosition,
            int _packetNum,
            string _id)
        {
            packetType = _packetType;
            playerPosition = _playerPosition;
            packetNum = _packetNum;
            id = _id;
        }

        public PacketType packetType { get; set; }
        public SerializableVector3 playerPosition { get; set; }
        public int packetNum { get; set; }
        public string id { get; set; }
        
        public static byte[] Serialize(ClientPacket obj)
        {
            if(obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static ClientPacket Deserialize(byte[] packet)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(packet, 0, packet.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            ClientPacket obj = (ClientPacket) binForm.Deserialize(memStream);

            return obj;
        }
    }
}