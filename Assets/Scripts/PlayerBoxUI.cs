using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Arrowfist.Managers;
using Unity.Services.Authentication;

public class PlayerBoxUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button removeBtn;

    private string playerId;

    private void Awake()
    {
        removeBtn.onClick.AddListener(RemoveClick);
    }

    public void SetInfo(string name, string playerId)
    {
        nameText.text = name;
        this.playerId = playerId;
        removeBtn.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost() && playerId != AuthenticationService.Instance.PlayerId);
    }

    public void RemoveClick()
    {
        LobbyManager.Instance.KickPlayer(playerId);
    }
}
