using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ProjectScenes { Application, Authentication, Lobby, Game }

public class SceneLoaderManager : MonoBehaviour
{
    public static SceneLoaderManager Instance { get; private set; }

    private ProjectScenes currScene;

    private void Awake()
    {
        Instance = this;
        LoadSceneAsync(ProjectScenes.Authentication);
    }

    public void LoadSceneAsync(ProjectScenes scene)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(scene));
    }

    private IEnumerator LoadSceneAsyncCoroutine(ProjectScenes sceneToLoad)
    {
        AsyncOperation asyncOperation;

        if (currScene != ProjectScenes.Application)
        {
            asyncOperation = SceneManager.UnloadSceneAsync((int)currScene);
            while (!asyncOperation.isDone) yield return null;
        }

        asyncOperation = SceneManager.LoadSceneAsync((int)sceneToLoad, LoadSceneMode.Additive);
        while (!asyncOperation.isDone) yield return null;

        currScene = sceneToLoad;
    }

    public void LoadNetworkScene(ProjectScenes sceneToLoad)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad.ToString(), LoadSceneMode.Additive);
    }  
}
