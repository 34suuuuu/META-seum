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
        private int startId;
    
        private GameObject go;
        public static float elapsedTimeSum;
        public static int elapsedTimeCount;
        public static float realTime;
        public static float minLatency;
        public static float maxLatency;

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        // Start is called before the first frame update
        void Start()
        {
            seed = StartScript.seedStatic;
            startId = StartScript.startIdStatic;
            maxLatency = 0;
            minLatency = int.MaxValue;
            Random.InitState(seed);
            go = Resources.Load("Capsule") as GameObject;
            Vector3 randPos = new Vector3(
                Random.Range(-10f, 20f),
                1,
                Random.Range(0f, 25f));
            Vector3 angle = new Vector3(0f, Random.Range(0, 360), 0f);
            
            GameObject gv = Instantiate(go, randPos, Quaternion.Euler(angle));
            gv.GetComponent<NetworkManager>().id = startId;
        }

        private void Update()
        {
            //Debug.Log(elapsedTimeSum / elapsedTimeCount * 1000 + "ms");
        }
    }
}
