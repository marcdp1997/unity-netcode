using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup lobbyCG;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private List<LobbyUI> lobbiesUI;

    public static GameUIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    
        lobbyCG.alpha = 1;

        createLobbyBtn.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CreateLobby();
            DisableLobbyScreen();
        });
    }

    public void DisableLobbyScreen()
    {
        lobbyCG.alpha = 0;
    }

    public void ShowLobbies(List<Lobby> lobbies)
    {
        // Disable all lobbiesUI
        for (int i = 0; i < lobbiesUI.Count; i++)
            lobbiesUI[i].gameObject.SetActive(false);

        // Activate the ones we want
        for (int i = 0; i < lobbies.Count; i++)
        {
            lobbiesUI[i].gameObject.SetActive(true);
            lobbiesUI[i].SetInfo(lobbies[i]);
        }
    }
}
