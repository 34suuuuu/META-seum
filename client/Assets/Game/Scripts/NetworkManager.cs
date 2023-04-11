using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Game.Scripts.Packet;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts
{
    public class NetworkManager: MonoBehaviour
    {
        public static NetworkManager Instance;
        
        [SerializeField] private string serverIp = "127.0.0.1";
        [SerializeField] private int port = 8080;
        
        #region "Public Members"
        public string id { get; private set; }
        public int packetNumber { get; private set; }
        [HideInInspector] public Vector3 desiredPosition;
        #endregion
        
        #region "Private Members"
        Dictionary<string, GameObject> _otherClients;
        Socket _udp;
        IPEndPoint _endPoint;
        #endregion

        void Awake()
        {
            if (serverIp == "")
                Debug.LogError("Server IP Address not set");
            if (port == -1)
                Debug.LogError("Port not set");

            Instance = this;

            packetNumber = 0;
            desiredPosition = transform.position;
            _otherClients = new Dictionary<string, GameObject>();
            _endPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
            _udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udp.Blocking = false;
            
            InitializeServer();
        }

        void InitializeServer()
        {
            Vector3 initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            ClientPacket clientPacket = new ClientPacket(PacketType.InitializePacket, new SerializableVector3(initialPosition), -1, null);
            byte[] packet = ClientPacket.Serialize(clientPacket);
            ClientPacket newClientPakcet = ClientPacket.Deserialize(packet);
            Debug.Log(newClientPakcet.playerPosition.toVector3().ToString());
            _udp.SendTo(packet, _endPoint);
        }

        public void SendPacket(Vector3 move)
        {
            if (id == null || id == "")
            {
                Debug.LogError("NOT Connected to server!");
                InitializeServer();
                return;
            }

            ClientPacket clientPacket = new ClientPacket(PacketType.PlayerControlPacket, new SerializableVector3(move), packetNumber, id);
            byte[] arr = ClientPacket.Serialize(clientPacket);
            _udp.SendTo(arr, _endPoint);
        }

        void OnQuit()
        {
            ClientPacket clientPacket = new ClientPacket(PacketType.QuitPacket, new SerializableVector3(), -1, null);
            byte[] arr = ClientPacket.Serialize(clientPacket);
            _udp.SendTo(arr, _endPoint);
            _udp.Close();
        }

        void Update()
        {
            if (_udp.Available != 0)
            {
                byte[] buf = new byte[64];
                _udp.Receive(buf);

                ClientPacket clientPacket = ClientPacket.Deserialize(buf);
                string parsedId = clientPacket.id;

                if (clientPacket.packetType == PacketType.InitializePacket)
                {
                    id = parsedId;
                    Debug.Log("client ID: " + id);
                    return;
                } else if (clientPacket.packetType == PacketType.QuitPacket && _otherClients.ContainsKey(id))
                {
                    Destroy(_otherClients[parsedId]);
                    _otherClients.Remove(parsedId);
                    return;
                }

                Vector3 posInPacket = clientPacket.playerPosition.toVector3();
                if (_otherClients.ContainsKey(parsedId))
                {
                    _otherClients[parsedId].transform.position = posInPacket;
                }
                else if (!parsedId.Equals(id))
                {
                    AddOtherClient(parsedId, posInPacket);
                }
                    
            }
        }

        void AddOtherClient(string parsedId, Vector3 pos)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = parsedId;
            go.transform.position = pos;
            _otherClients.Add(parsedId, go);
        }
    }
}