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
        inputField.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;

        inputField.onValidateInput = (string text, int charIndex, char c) =>
        {
            return ValidateChar(text, charIndex, c);
        };
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

    private char ValidateChar(string text, int charIndex, char c)
    {
        return (char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)) ? c : '\0';
    }
}
