using System;
using UnityEngine;
public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField] private Login LoginPanel;
    [SerializeField] private Lobby LobbyPanel;

    public void EnterLobby()
    {
        LoginPanel.gameObject.SetActive(false);
        LobbyPanel.gameObject.SetActive(true);
    }
}
