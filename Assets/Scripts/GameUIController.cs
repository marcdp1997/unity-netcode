using UnityEngine;
using Arrowfist.Managers;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup winCG;
    [SerializeField] private CanvasGroup loseCG;

    private const float ReturnMenuDelayTime = 3;

    private void Awake()
    {
        EnableScreen(winCG, false);
        EnableScreen(loseCG, false);

        EventManager.Instance.Subscribe(EventIds.OnGameEnded, OnGameEnded);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(EventIds.OnGameEnded, OnGameEnded);
    }

    private void OnGameEnded(EventData eventData)
    {
        if (((OnGameEndedEventData)eventData).Win) EnableScreen(winCG, true);
        else EnableScreen(loseCG, true);

        Invoke(nameof(GoBackToMenu), ReturnMenuDelayTime);
    }

    private void GoBackToMenu()
    {
        SceneLoaderManager.Instance.LoadSceneAsync(ProjectScenes.Lobby);
    }

    private void EnableScreen(CanvasGroup screen, bool state)
    {
        screen.alpha = state ? 1 : 0;
        screen.interactable = state;
        screen.blocksRaycasts = state;
    }
}
