using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text;
using StarterAssets.Packet;

public class Server : MonoBehaviour
{
    #region "Inspector Members"
    [SerializeField] int port = 8080;
    [Tooltip("Distance to move at each move input (must match with client)")]
    [SerializeField] float moveDistance = 1f;
    [Tooltip("Number of frames to wait until next processing")]
    [SerializeField] int frameWait = 5;
    #endregion

    #region "Private Members"
    Socket udp;
    int idAssignIndex = 0;
    Dictionary<EndPoint, Client> clients;
    #endregion

    void Start()
    {
        clients = new Dictionary<EndPoint, Client>();

        IPHostEntry host = Dns.Resolve(Dns.GetHostName());
        IPAddress ip = host.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ip, port);

        Debug.Log("Server IP Address: " + ip);
        Debug.Log("Port: " + port);
        udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        udp.Bind(endPoint);
        udp.Blocking = false;
    }

    void Update()
    {
        if(Time.frameCount % frameWait == 0 && udp.Available != 0)
        {
            byte[] packet = new byte[1024];
            EndPoint sender = new IPEndPoint(IPAddress.Any, port);

            int rec = udp.ReceiveFrom(packet, ref sender);

            PacketDatagram pd = (PacketDatagram) PacketSerializer.Deserializer(packet);

            if (pd.status == "request")
            {
                HandleNewClient(sender, pd);
            } else if (pd.status == "quit")
            {
                DisconnectClient(sender, pd);
            }else if (pd.status == "connected")
            {
                int packetId = pd.playerInfoPacket.id;
                int seqNum = pd.packetNum;
                if (packetId == -1 || seqNum == -1)
                    return;
                
                HandleUserMoveInput(sender, pd, seqNum);
            }
        }
    }

    void HandleNewClient(EndPoint addr, PacketDatagram pd)
    {
        Debug.Log(pd.toString());
        pd.packetNum = 0;
        pd.status = "request";
        pd.playerInfoPacket.id = idAssignIndex++;
        
        Debug.Log($"Handling new client with id [{pd.playerInfoPacket.id}] ");
        
        SendPacket(pd, addr);

        pd.status = "connected";
        clients.Add(addr, new Client(pd.playerInfoPacket.id, pd));
        SendPositionToAllClients(pd);
    }

    void DisconnectClient(EndPoint sender, PacketDatagram pd)
    {
        if(clients.ContainsKey(sender))
            clients.Remove(sender);
        Broadcast(pd);
    }

    void Broadcast(PacketDatagram pd)
    {
        foreach (KeyValuePair<EndPoint, Client> p in clients)
            SendPacket(pd, p.Key);
    }

    void HandleUserMoveInput(EndPoint client, PacketDatagram pd, int seqNumber)
    {
        if(!clients.ContainsKey(client) || clients[client].lastSeqNumber > seqNumber)
            return;

        if(!clients[client].history.ContainsKey(seqNumber))
        {
            clients[client].UpdateStateHistory(seqNumber);
            clients[client].lastSeqNumber = seqNumber;
        }
        UpdatePosition(client, pd);
        /* so that clients see newly connected clients */
        SendPositionToAllClients(pd);
    }

    void UpdatePosition(EndPoint addr, PacketDatagram pd)
    {
        Debug.Log($"packetId: {pd.playerInfoPacket.id}" +
                  $"position: {pd.playerPosPacket.toString()}");
        clients[addr].pd = pd;
        clients[addr].pos = pd.playerPosPacket.toVector3();
        clients[addr].cam = pd.playerCamPacket.toQuaternion();
    }

    void SendPositionToAllClients(PacketDatagram pd)
    {
        foreach (KeyValuePair<EndPoint, Client> p in clients)
        {
            SendPacket(pd, p.Key);
        }
    }
    
    void SendPacket(PacketDatagram pd, EndPoint addr)
    {
        byte[] packet = PacketSerializer.Serializer(pd);
        udp.SendTo(packet, addr);
    }
}
