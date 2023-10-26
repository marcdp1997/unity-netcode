using TMPro;
using UnityEngine;

public class PlayerBoxUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    public void SetInfo(string name)
    {
        nameText.text = name;
    }
}
