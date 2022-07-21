using TMPro;
using UnityEngine;

public class InGameUserSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userNameText;

    public void InitUserSlot(string userName)
    {
        userNameText.text = userName;
    }
}
