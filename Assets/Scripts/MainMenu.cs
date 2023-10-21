using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private bool pressed;

    private void Awake()
    {
        if (NetworkManager.Singleton != null) 
            Destroy(NetworkManager.Singleton.gameObject);
    }

    private void Update()
    {
        if (Input.anyKey && !pressed)
        {
            pressed = true;
            SceneManager.Instance.LoadScene(Scene.Game);
        }
    }
}
