using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Text playerNumInput;
    public Text playerMovingrateInput;
    public Text ipAddressInput;
    public Text groupId;
    public Text seed;
    public Text roomId;
    // Start is called before the first frame update
    void Start()
    {
        playerNumInput.text = StartScript.playerNumStatic.ToString();
        playerMovingrateInput.text = StartScript.playerMovingrateStatic.ToString();
        ipAddressInput.text = StartScript.ipAddressStatic;
        groupId.text = StartScript.groupIdStatic.ToString();
        seed.text = StartScript.seedStatic.ToString();
        roomId.text = StartScript.roomIdStatic.ToString();
    }
}
