using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class RandomMover : MonoBehaviour
{
    private int count;
    private float movingRatio;
    // Start is called before the first frame update
    void Start()
    {
        count = Random.Range(40, 110);
        movingRatio = StartScript.playerMovingrateStatic;
    }

    // Update is called once per frame
    void Update()
    {
        if (count > 0)
        {
            transform.position += (0.01f * transform.forward);
            count--;
        }
        else if (count < 0)
        {
            count++;
        }
        else
        {
            if (Random.Range(0f, 1f) < movingRatio)
            {
                Vector3 angle = new Vector3(0f, Random.Range(0, 360), 0f);
                transform.Rotate(angle);
                count = Random.Range(40, 110);
            }
            else
            {
                count = -Random.Range(110, 300);
            }
        }
    }
}
