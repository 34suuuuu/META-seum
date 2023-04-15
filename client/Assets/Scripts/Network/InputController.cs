using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using StarterAssets.Packet;

public class InputController : MonoBehaviour
{
    NetworkManager networkManager;
    Vector3 playerPos = new Vector3();
    PacketDatagram userDatagram;
    // Start is called before the first frame update
    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        userDatagram = new PacketDatagram();
    }

    // Update is called once per frame
    void Update()
    {
        if (networkManager.id != -1)
        {
            if ((playerPos - transform.position).sqrMagnitude > 0.0001) 
            {
                Vector3 currentPos = transform.position;
                Quaternion currentCam = transform.rotation;

                userDatagram.status = "connected";
                userDatagram.playerPosPacket = new PlayerPosPacket(NetworkUtility.ChangeVector3Package(currentPos));
                userDatagram.playerCamPacket = new PlayerCamPacket(NetworkUtility.ChangeQuaternionPackage(currentCam));
                userDatagram.playerInfoPacket = new PlayerInfoPacket();
                userDatagram.playerInfoPacket.id = networkManager.id;
                
                networkManager.SendPacket(userDatagram);
                playerPos = transform.position;
            }
        }
    }
}
