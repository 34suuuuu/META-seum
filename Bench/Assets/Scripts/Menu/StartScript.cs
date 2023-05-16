using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    public static int playerNumStatic;
    public static float playerMovingrateStatic;
    public static string ipAddressStatic;
    public static int portStatic;
    public static int groupIdStatic;
    public static int seedStatic;

    public InputField playerNumInput;
    public InputField playerMovingrateInput;
    public InputField ipAddressInput;
    public InputField portInput;
    public InputField groupId;
    public InputField seed;
    
    // Update is called once per frame
    public void StartGame()
    {
        playerNumStatic = int.Parse(playerNumInput.text);
        playerMovingrateStatic = float.Parse(playerMovingrateInput.text);
        ipAddressStatic = ipAddressInput.text;
        portStatic = int.Parse(portInput.text);
        groupIdStatic = int.Parse(groupId.text);
        seedStatic = int.Parse(seed.text);

        SceneManager.LoadScene(1);
    }

}       
