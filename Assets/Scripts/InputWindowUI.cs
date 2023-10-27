using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputWindowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button okBtn;
    [SerializeField] private Button cancelBtn;

    public UnityEvent<string> onOk = new UnityEvent<string>();
    public UnityEvent onCancel = new UnityEvent();

    private void Awake()
    {
        okBtn.onClick.AddListener(OkClick);
        cancelBtn.onClick.AddListener(CancelClick);
    }

    public void Show(string newTitle, int newMaxCharacters)
    {
        gameObject.SetActive(true);
        title.text = newTitle;
        inputField.characterLimit = newMaxCharacters;
        inputField.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OkClick()
    {
        onOk?.Invoke(inputField.text);
        Hide();
    }

    private void CancelClick()
    {
        onCancel?.Invoke();
        Hide();
    }
}
