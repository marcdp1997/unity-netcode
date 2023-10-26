using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBoxUI : MonoBehaviour
{
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI playersText;

    private string lobbyId;

    private void Awake()
    {
        joinLobbyButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinLobbyById(lobbyId);
        });
    }

    public void SetInfo(Lobby lobby)
    {
        lobbyId = lobby.Id;
        nameText.text = lobby.Name;
        playersText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
    }
}
