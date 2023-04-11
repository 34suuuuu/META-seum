using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.InputSystem;

namespace Game.Scripts.Packet
{
    public class ClientPacket
    {
        public ClientPacket(PacketType packetType,
            InputActionAsset inputActionAsset,
            int packetNum,
            string id)
        {
            _packetType = packetType;
            _inputActionAsset = inputActionAsset;
            _packetNum = packetNum;
            _id = id;
        }

        private PacketType _packetType { get; set; }
        private InputActionAsset _inputActionAsset { get; set; }
        private int _packetNum { get; set; }
        private string _id { get; set; }
        
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