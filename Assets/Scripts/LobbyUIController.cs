using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Arrowfist.Managers;

public class LobbyUIController : MonoBehaviour
{
    [Header("Welcome Screen")]
    [SerializeField] private CanvasGroup welcomeScreen;
    [SerializeField] private Button returnBtn;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button findLobbyBtn;

    [Header("Lobby Params Screen")]
    [SerializeField] private CanvasGroup lobbyParamsScreen;
    [SerializeField] private Button changeLobbyNameBtn;
    [SerializeField] private Button changeLobbyPlayersBtn;
    [SerializeField] private Button changeLobbyPrivacyBtn;
    [SerializeField] private Button saveParamsBtn;
    [SerializeField] private InputWindowUI inputWindow;

    [Header("Search Lobbies Screen")]
    [SerializeField] private CanvasGroup searchLobbiesScreen;
    [SerializeField] private Button refreshBtn;
    [SerializeField] private GameObject lobbyBoxUI;

    [Header("Wait Screen")]
    [SerializeField] private CanvasGroup waitScreen;
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private Button startGameBtn;
    [SerializeField] private GameObject playerBoxUI;

    private CanvasGroup currScreen;
    private TextMeshProUGUI lobbyNameBtnText;
    private TextMeshProUGUI lobbyPlayersBtnText;
    private TextMeshProUGUI lobbyPrivacyBtnText;

    private bool isPrivate = false;
    private int currNumPlayers = 2;
    private const int MinNumPlayers = 2;
    private const int MaxNumPlayers = 4;

    private void Awake()
    {
        lobbyNameBtnText = changeLobbyNameBtn.GetComponentInChildren<TextMeshProUGUI>();
        lobbyPlayersBtnText = changeLobbyPlayersBtn.GetComponentInChildren<TextMeshProUGUI>();
        lobbyPrivacyBtnText = changeLobbyPrivacyBtn.GetComponentInChildren<TextMeshProUGUI>();

        ChangeScreen(welcomeScreen);
        EnableScreen(lobbyParamsScreen, false);
        EnableScreen(searchLobbiesScreen, false);
        EnableScreen(waitScreen, false);

        returnBtn.onClick.AddListener(ReturnClick);
        createLobbyBtn.onClick.AddListener(CreateLobbyClick);
        findLobbyBtn.onClick.AddListener(SearchForLobbiesClick);
        changeLobbyNameBtn.onClick.AddListener(ChangeLobbyNameClick);
        changeLobbyPlayersBtn.onClick.AddListener(ChangeLobbyPlayersClick);
        changeLobbyPrivacyBtn.onClick.AddListener(ChangeLobbyPrivacyClick);
        saveParamsBtn.onClick.AddListener(SaveParamsClick);
        refreshBtn.onClick.AddListener(RefreshClick);
        startGameBtn.onClick.AddListener(StartGameClick);

        EventManager.Instance.Subscribe(EventIds.OnLobbyCreated, OnLobbyCreated);
        EventManager.Instance.Subscribe(EventIds.OnLobbyJoined, OnLobbyJoined);
        EventManager.Instance.Subscribe(EventIds.OnLobbyListUpdated, OnLobbyListUpdated);
        EventManager.Instance.Subscribe(EventIds.OnLobbyJoinedUpdated, OnLobbyJoinedUpdated);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(EventIds.OnLobbyCreated, OnLobbyCreated);
        EventManager.Instance.Unsubscribe(EventIds.OnLobbyJoined, OnLobbyJoined);
        EventManager.Instance.Unsubscribe(EventIds.OnLobbyListUpdated, OnLobbyListUpdated);
        EventManager.Instance.Unsubscribe(EventIds.OnLobbyJoinedUpdated, OnLobbyJoinedUpdated);
    }

    private void CreateLobbyClick()
    {
        ChangeScreen(lobbyParamsScreen);
    }

    private void ChangeLobbyNameClick()
    {
        inputWindow.Show("Enter lobby name", 15);
        inputWindow.onOk.AddListener(OnLobbyNameChanged);
    }

    private void OnLobbyNameChanged(string newName)
    {
        lobbyNameBtnText.text = newName;
        inputWindow.onOk.RemoveListener(OnLobbyNameChanged);
    }

    private void ChangeLobbyPlayersClick()
    {
        currNumPlayers++;
        if (currNumPlayers > MaxNumPlayers) currNumPlayers = MinNumPlayers;
        lobbyPlayersBtnText.text = currNumPlayers.ToString();
    }

    private void ChangeLobbyPrivacyClick()
    {
        isPrivate = !isPrivate;
        lobbyPrivacyBtnText.text = isPrivate ? "Private" : "Public"; 
    }

    private void SaveParamsClick()
    {
        lobbyName.text = lobbyNameBtnText.text;
        LobbyManager.Instance.CreateLobby(lobbyNameBtnText.text, currNumPlayers, isPrivate);
    }

    private void OnLobbyCreated(EventData eventData)
    {
        ChangeScreen(waitScreen);
        startGameBtn.gameObject.SetActive(true);
    }

    private void SearchForLobbiesClick()
    {
        ChangeScreen(searchLobbiesScreen);
    }

    private void OnLobbyJoined(EventData eventData)
    {
        ChangeScreen(waitScreen);
        startGameBtn.gameObject.SetActive(false);
    }

    private void ReturnClick()
    {
        if (searchLobbiesScreen.alpha == 1) ChangeScreen(welcomeScreen);
        if (lobbyParamsScreen.alpha == 1) ChangeScreen(welcomeScreen);

        if (waitScreen.alpha == 1)
        {
            ChangeScreen(LobbyManager.Instance.IsLobbyHost() ? welcomeScreen : searchLobbiesScreen);
            LobbyManager.Instance.LeaveLobby();
        }
    }

    private void RefreshClick()
    {
        LobbyManager.Instance.RefreshLobbyList();
    }

    private void StartGameClick()
    {
        LobbyManager.Instance.StartGame();
    }

    private void OnLobbyListUpdated(EventData eventData)
    {
        if (searchLobbiesScreen.alpha != 1) return;

        foreach (Transform child in lobbyBoxUI.transform.parent)
            if (child != lobbyBoxUI.transform) Destroy(child.gameObject);

        List<Lobby> lobbies = ((OnLobbyListRefreshedEventData)eventData).Lobbies;
        for (int i = 0; i < lobbies.Count; i++)
        {
            LobbyBoxUI box = Instantiate(lobbyBoxUI, lobbyBoxUI.transform.parent).GetComponent<LobbyBoxUI>();
            box.gameObject.SetActive(true);
            box.SetInfo(lobbies[i]);
        }
    }

    private void OnLobbyJoinedUpdated(EventData eventData)
    {
        if (waitScreen.alpha != 1) return;

        List<Player> players = ((OnLobbyJoinedUpdatedEventData)eventData).Lobby.Players;
        string keyPlayerName = LobbyManager.KeyPlayerName;

        foreach (Transform child in playerBoxUI.transform.parent)
            if (child != playerBoxUI.transform) Destroy(child.gameObject);

        for (int i = 0; i < players.Count; i++)
        {
            PlayerBoxUI box = Instantiate(playerBoxUI, playerBoxUI.transform.parent).GetComponent<PlayerBoxUI>();
            box.gameObject.SetActive(true);
            box.SetInfo(players[i].Data[keyPlayerName].Value);
        }
    }

    private void ChangeScreen(CanvasGroup newScreen)
    {
        if (currScreen != null) EnableScreen(currScreen, false);
        EnableScreen(newScreen, true);
        currScreen = newScreen;

        returnBtn.gameObject.SetActive(currScreen != welcomeScreen);
    }

    private void EnableScreen(CanvasGroup screen, bool state)
    {
        screen.alpha = state ? 1 : 0;
        screen.interactable = state;
        screen.blocksRaycasts = state;
    }
}
