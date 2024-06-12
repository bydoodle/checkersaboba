using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinPanel : NetworkBehaviour
{
    [SerializeField] private Button joinButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private TMP_InputField ipInputField;

    private void Start()
    {
        if (IsOwner) return;

        joinButton.onClick.AddListener(JoinGame);
        hostButton.onClick.AddListener(HostGame);
    }

    private void JoinGame()
    {
        var connectionInfo = ipInputField.text.Split(':');
        ((UnityTransport)NetworkManager.NetworkConfig.NetworkTransport)
                .SetConnectionData(connectionInfo[0], ushort.Parse(connectionInfo[1]), connectionInfo[0]);

        NetworkManager.StartClient();
    }

    private void HostGame()
    {
        NetworkManager.StartHost();
        NetworkManager.SceneManager.LoadScene("SampleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
