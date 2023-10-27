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
    }

    private void AuthenticateClick()
    {
        inputWindow.Show("Enter your name", 15);
        inputWindow.onOk.AddListener(InputWindowSaveClick);
    }

    private void InputWindowSaveClick(string playerName)
    {
        Authenticate(playerName);
        inputWindow.onOk.RemoveListener(InputWindowSaveClick);
    }

    private void Authenticate(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
            playerName = "Player" + Random.Range(10, 100);

        LobbyManager.Instance.Authenticate(playerName);
        SceneLoaderManager.Instance.LoadSceneAsync(ProjectScenes.Lobby);
    }
}
