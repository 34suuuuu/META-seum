using System;

namespace Game.Scripts.Packet
{
    [Serializable]
    public enum PacketType
    {
        InitializePacket,
        PlayerControlPacket,
        QuitPacket
    }
}