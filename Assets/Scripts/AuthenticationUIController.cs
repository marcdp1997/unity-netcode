using UnityEngine;
using UnityEngine.UI;

public class AuthenticationUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup authenticateScreen;
    [SerializeField] private Button authenticateBtn;
    [SerializeField] private InputWindowUI inputWindow;

    private void Awake()
    {
        authenticateBtn.onClick.AddListener(AuthenticateClick);
        inputWindow.onOk.AddListener(InputWindowSaveClick);
    }

    private void AuthenticateClick()
    {
        inputWindow.Show("Enter your name", 15);
    }

    private void InputWindowSaveClick(string playerName)
    {
        Authenticate(playerName);
    }

    private void Authenticate(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
            playerName = "Player" + Random.Range(10, 100);

        LobbyManager.Instance.Authenticate(playerName);
        SceneLoaderManager.Instance.LoadSceneAsync(ProjectScenes.Lobby);
    }
}
