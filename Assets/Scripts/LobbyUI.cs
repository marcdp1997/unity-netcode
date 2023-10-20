using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button joinButton;
    [SerializeField] private TextMeshProUGUI nameText;

    private string lobbyId;

    private void Awake()
    {
        joinButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinLobby(lobbyId);
            GameUIManager.Instance.DisableLobbyScreen();
        });
    }

    public void SetInfo(Lobby lobby)
    {
        lobbyId = lobby.Id;
        nameText.text = name;
    }
}