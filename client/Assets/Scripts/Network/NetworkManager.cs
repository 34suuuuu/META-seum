using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using StarterAssets.Packet;
using UnityEngine;

namespace StarterAssets
{
    public class StateHistory
    {
        public Vector3 pos;
        public Quaternion cam;

        public StateHistory(Vector3 _pos, Quaternion _cam)
        {
            pos = _pos;
            cam = _cam;
        }
    }
    
    public class NetworkManager: MonoBehaviour
    {
        [SerializeField] private string serverIp = "127.0.0.1";
        [SerializeField] private int port = 8080;

        public int id { get; private set; }
        public string playerName;
        public int packetNumber { get; private set; }
        public Dictionary<int, StateHistory> histories;
        [HideInInspector] public Vector3 desiredPos;
        [HideInInspector] public Quaternion desiredCam;

        private Dictionary<int, GameObject> _otherPlayers;
        private IPEndPoint _endPoint;
        private Socket _udp;

        private GameObject _playerObject;
        
        private void Awake()
        {
            if (serverIp == "")
                Debug.LogError("Server IP is required");
            if (port == -1)
                Debug.LogError("Server port is required");

            id = -1;
            packetNumber = 0;
            histories = new Dictionary<int, StateHistory>();
            desiredPos = transform.position;
            desiredCam = transform.rotation;
            
            _otherPlayers = new Dictionary<int, GameObject>();
            _endPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
            _udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udp.Blocking = false;
            
            InitialConnection();
            histories.Add(0, new StateHistory(transform.position, transform.rotation));
        }

        private void Start()
        {
            _playerObject = Resources.Load("PlayerObject") as GameObject;
        }

        private void Update()
        {
            if (_udp.Available != 0)
            {
                byte[] buf = new byte[1024];
                _udp.Receive(buf);

                PacketDatagram pd = (PacketDatagram) PacketSerializer.Deserializer(buf);
                int packetId = pd.playerInfoPacket.id;

                if (pd.status == "request")
                {
                    id = packetId;
                    return;
                } 
                else if (pd.status == "quit" && _otherPlayers.ContainsKey(packetId))
                {
                    Destroy(_otherPlayers[packetId]);
                    _otherPlayers.Remove(packetId);
                    return;
                }

                int seqNum = pd.packetNum;
                if (packetId == -1 || seqNum == -1)
                    return;
                
                Vector3 packetPos = pd.playerPosPacket.toVector3();
                Quaternion packetCam = pd.playerCamPacket.toQuaternion();

                if (packetId == id)//&& histories.ContainsKey(seqNum) 
                                   //&& !histories[seqNum].Equals(new StateHistory(packetPos, packetCam)))
                {
                    Debug.Log(pd.packetNum);

                } else if (_otherPlayers.ContainsKey(packetId))
                {
                    UpdateOtherPlayer(packetId, packetPos, packetCam);
                } else if (!packetId.Equals(id))
                {
                    AddOtherPlayer(packetId, packetPos, packetCam);
                }

            }
        }

        private void OnApplicationQuit()
        {
            PacketDatagram pd = new PacketDatagram();

            pd.packetNum = -999;
            pd.status = "quit";
            pd.playerPosPacket = new PlayerPosPacket();
            pd.playerCamPacket = new PlayerCamPacket();
            pd.playerInfoPacket = new PlayerInfoPacket();
            pd.playerInfoPacket.id = id;
            
            SendPacket(pd);
            
            _udp.Close();
        }

        private void InitialConnection()
        {
            Vector3 pos = transform.position;
            Quaternion cam = transform.rotation;
            
            PacketDatagram pd = new PacketDatagram();

            pd.packetNum = -999;
            pd.status = "request";
            pd.playerPosPacket = new PlayerPosPacket(pos);
            pd.playerCamPacket = new PlayerCamPacket(cam);
            pd.playerInfoPacket = new PlayerInfoPacket();

            byte[] packet = PacketSerializer.Serializer(pd);
            _udp.SendTo(packet, _endPoint);
        }

        public void SendPacket(PacketDatagram pd)
        {
            if (id == -1)
            {
                Debug.LogError("NOT connected to server");
                InitialConnection();
                return;
            }

            pd.packetNum = packetNumber;
            
            UpdateHistory();
            byte[] packet = PacketSerializer.Serializer(pd);
            _udp.SendTo(packet, _endPoint);
        }

        private void UpdateHistory()
        {
            histories.Add(++packetNumber, new StateHistory(desiredPos, desiredCam));
            bool suc = histories.Remove(packetNumber - 51);
        }

        private void AddOtherPlayer(int packetId, Vector3 pos, Quaternion cam)
        {
            GameObject otherPlayer = MonoBehaviour.Instantiate(_playerObject);
            otherPlayer.name = $"ID{packetId}";
            otherPlayer.transform.position = pos;
            otherPlayer.transform.rotation = cam;
            _otherPlayers.Add(packetId, otherPlayer);
        }

        private void UpdateOtherPlayer(int packetId, Vector3 pos, Quaternion cam)
        {
            GameObject otherPlayer = _otherPlayers[packetId];
            Vector3 beforePos = otherPlayer.transform.position;

            float moveSpeed = 4.0f;
            
            otherPlayer.transform.position = Vector3.Lerp(beforePos, pos, moveSpeed * Time.deltaTime);
            otherPlayer.transform.rotation = cam;
        }
    }
}