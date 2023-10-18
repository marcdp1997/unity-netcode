using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup lobbyUI;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button joinLobbyBtn;

    private void Awake()
    {
        lobbyUI.alpha = 1;

        createLobbyBtn.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CreateLobby();
            lobbyUI.alpha = 0;
        });

        joinLobbyBtn.onClick.AddListener(() =>
        {
            LobbyManager.Instance.QuickJoinLobby();
            lobbyUI.alpha = 0;
        });
    }
}
