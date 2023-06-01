using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    // latency -> fps로 
    public Text avglatencyText;
    public Text reallatencyText;
    public Text minLatencyText;
    public Text maxLatencyText;
    
    public Text playerMovingrateInput;
    public Text ipAddressInput;
    public Text groupId;
    public Text seed;
    public Text roomId;
    // Start is called before the first frame update
    void Start()
    {
        playerMovingrateInput.text = StartScript.playerMovingrateStatic.ToString();
        ipAddressInput.text = StartScript.ipAddressStatic;
        groupId.text = StartScript.groupIdStatic.ToString();
        seed.text = StartScript.seedStatic.ToString();
        roomId.text = StartScript.roomIdStatic.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        avglatencyText.text = (CapsuleGenerator.elapsedTimeSum / CapsuleGenerator.elapsedTimeCount * 1000) + "ms";
        reallatencyText.text = (CapsuleGenerator.realTime * 1000) + "ms";
        minLatencyText.text = (CapsuleGenerator.minLatency * 1000) + "ms";
        maxLatencyText.text = (CapsuleGenerator.maxLatency * 1000) + "ms";
        // menu button, quit button
        // Camera unit
        // 
    }
}
