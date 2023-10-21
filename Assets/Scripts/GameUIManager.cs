using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup lobbyCG;
    [SerializeField] private CanvasGroup winCG;
    [SerializeField] private CanvasGroup loseCG;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private List<LobbyUI> lobbiesUI;

    private const float ReturnMenuDelayTime = 3;
    public static GameUIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        EnableScreen(false, lobbyCG);
        Invoke(nameof(EnableLobbyScreen), 0.5f);

        EnableScreen(false, winCG);
        EnableScreen(false, loseCG);

        createLobbyBtn.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CreateLobby();
            DisableLobbyScreen();
        });
    }

    private void EnableLobbyScreen()
    {
        EnableScreen(true, lobbyCG);
    }

    public void DisableLobbyScreen()
    {
        EnableScreen(false, lobbyCG);
    }

    public void EnableWinScreen()
    {
        EnableScreen(true, winCG);
        Invoke(nameof(GoBackToMenu), ReturnMenuDelayTime);
    }

    public void EnableLoseScreen()
    {
        EnableScreen(true, loseCG);
        Invoke(nameof(GoBackToMenu), ReturnMenuDelayTime);
    }

    private void GoBackToMenu()
    {
        SceneManager.Instance.LoadScene(Scene.MainMenu);
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

    private void EnableScreen(bool state, CanvasGroup screen)
    {
        if (state)
        {
            screen.alpha = 1;
            screen.interactable = true;
            screen.blocksRaycasts = true;
        }
        else
        {
            screen.alpha = 0;
            screen.interactable = false;
            screen.blocksRaycasts = false;
        }
    }
}
