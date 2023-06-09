using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;
using Quaternion = UnityEngine.Quaternion;

namespace StarterAssets
{
    public class CapsuleGenerator : MonoBehaviour
    {
        private int playerNum;
        private int seed;
        private int startId;
    
        private GameObject go;
    
        void Awake() 
        {
            Application.targetFrameRate = 60;
        }
        // Start is called before the first frame update
        void Start()
        {
            seed = StartScript.seedStatic;
            playerNum = StartScript.playerNumStatic;
            startId = StartScript.startIdStatic;
            Random.InitState(seed);
            for (int i = startId; i < startId + playerNum; i++)
            {
                go = Resources.Load("Capsule") as GameObject;
                Vector3 randPos = new Vector3(
                    Random.Range(-10f, 20f),
                    1,
                    Random.Range(0f, 25f));
                Vector3 angle = new Vector3(0f, Random.Range(0, 360), 0f);
                
                GameObject gv = Instantiate(go, randPos, Quaternion.Euler(angle));
                gv.GetComponent<NetworkManager>().id = i;
            }
        }
    }
}
