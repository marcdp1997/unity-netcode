using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Scene { Menu, Game }

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void LoadScene(Scene scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)scene);
    }
}
