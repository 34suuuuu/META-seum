using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInfo : MonoBehaviour
{
    public GameObject fpsInfo;
    public GameObject testInfo;
    
    public void ToggleFPSInfo()
    {
        fpsInfo.SetActive(!fpsInfo.activeInHierarchy);
    }
    
    public void ToggleTestInfo()
    {
        testInfo.SetActive(!testInfo.activeInHierarchy);
    }
}
