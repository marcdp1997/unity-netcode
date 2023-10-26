using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputWindowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button saveBtn;
    [SerializeField] private Button cancelBtn;

    public UnityEvent<string> onSave = new UnityEvent<string>();
    public UnityEvent onCancel = new UnityEvent();

    private void Awake()
    {
        Hide();

        saveBtn.onClick.AddListener(SaveClick);
        cancelBtn.onClick.AddListener(CancelClick);
    }

    public void Show(string newTitle)
    {
        title.text = newTitle;
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SaveClick()
    {
        onSave?.Invoke(inputField.text);
        Hide();
    }

    private void CancelClick()
    {
        onCancel?.Invoke();
        Hide();
    }
}
