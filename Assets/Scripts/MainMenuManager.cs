using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public TMP_Text versionTxt;
    public TMP_InputField ipInput;
    public TMP_InputField nicknameInput;
    public GameObject menuPanel;
    public GameObject connectingPanel;
    public float connectionTimeout = 5f;

    private void Start()
    {
        versionTxt.text = "v." + Application.version.ToString();
        nicknameInput.text = "Player_" + Random.Range(1000, 10000).ToString();
    }

    public void Connect()
    {
        GameData.nickname = nicknameInput.text;

        menuPanel.SetActive(false);
        connectingPanel.SetActive(true);
        Invoke(nameof(ConnectionTimeout), connectionTimeout);

        if (NetworkManager.singleton != null)
        {
            NetworkManager.singleton.networkAddress = ipInput.text;
            NetworkManager.singleton.StartClient();
        }
    }

    private void ConnectionTimeout()
    {
        menuPanel.SetActive(true);
        connectingPanel.SetActive(false);
    }
}
