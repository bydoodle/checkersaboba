using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public bool startAsHost;
    private void Start()
    {
        if (startAsHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
            NetworkManager.Singleton.StartClient();
    }
}