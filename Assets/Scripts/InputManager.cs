using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEditor;

public class InputManager : MonoBehaviour
{
    private bool usingGamepad;
    private PlayerInput playerInput;
    private EventSystem eventSystem;
    private GameObject firstSelected;

    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        playerInput = gameObject.GetComponent<PlayerInput>();
        playerInput.onControlsChanged += ChangeControls;

        eventSystem = gameObject.GetComponent<EventSystem>();
        firstSelected = eventSystem.firstSelectedGameObject;
    }

    private void OnDestroy()
    {
        playerInput.onControlsChanged -= ChangeControls;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void ChangeControls(PlayerInput pi)
    {
        usingGamepad = pi.currentControlScheme == "Gamepad";

        if (usingGamepad)
        {
            eventSystem.SetSelectedGameObject(firstSelected);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else Cursor.lockState = CursorLockMode.Confined;

        Cursor.visible = !usingGamepad;
    }

    public bool IsUsingGamepad() { return usingGamepad; }
}
