using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInfo : MonoBehaviour
{
    public GameObject testInfo;
    
    public void ToggleTestInfo()
    {
        testInfo.SetActive(!testInfo.activeInHierarchy);
    }
}
