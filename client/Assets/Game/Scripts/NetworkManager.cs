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
            ClientPacket clientPacket = new ClientPacket(PacketType.InitializePacket, null, -1, null);
            byte[] packet = ClientPacket.Serialize(clientPacket);
            _udp.SendTo(packet, _endPoint);
        }

        void SendPacket(InputActionAsset move)
        {
            if (id == null || id == "")
            {
                Debug.LogError("NOT Connected to server!");
                InitializeServer();
                return;
            }

            ClientPacket clientPacket = new ClientPacket(PacketType.PlayerControlPacket, move, packetNumber, id);
            byte[] arr = ClientPacket.Serialize(clientPacket);
            _udp.SendTo(arr, _endPoint);
        }

        void OnQuit()
        {
            ClientPacket clientPacket = new ClientPacket(PacketType.QuitPacket, null, -1, null);
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
                
            }
        }
    }
}