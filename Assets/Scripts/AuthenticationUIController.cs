using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuthenticationUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup authenticateScreen;
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private Button authenticateBtn;

    private void Awake()
    {
        authenticateBtn.onClick.AddListener(AuthenticateClick);
    }

    private void AuthenticateClick()
    {
        if (string.IsNullOrEmpty(playerName.text))
            playerName.text = "Player" + Random.Range(10, 100);

        LobbyManager.Instance.Authenticate(playerName.text);
        SceneLoaderManager.Instance.LoadSceneAsync(ProjectScenes.Lobby);
    }
}
