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
        private int seed;
        private int playerNum;
    
        private GameObject go;
        public static float elapsedTimeSum;
        public static int elapsedTimeCount;
        public static float realTime;
        public static float minLatency;
        public static float maxLatency;
    
        // Start is called before the first frame update
        void Start()
        {
            seed = StartScript.seedStatic;
            playerNum = StartScript.playerNumStatic;
            maxLatency = 0;
            minLatency = int.MaxValue;
            Random.InitState(seed);
            for (int i = 0; i < playerNum; i++)
            {
                go = Resources.Load("Capsule") as GameObject;
                Vector3 randPos = new Vector3(
                    Random.Range(-10f, 20f),
                    1,
                    Random.Range(0f, 25f));
                Vector3 angle = new Vector3(0f, Random.Range(0, 360), 0f);
                
                GameObject gv = Instantiate(go, randPos, Quaternion.Euler(angle));
                gv.GetComponent<NetworkManager>().id = i + 1;
            }
        }

        private void Update()
        {
            //Debug.Log(elapsedTimeSum / elapsedTimeCount * 1000 + "ms");
        }
    }
}
