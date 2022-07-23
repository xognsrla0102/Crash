using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUserSlot : MonoBehaviour
{
    [SerializeField] private RawImage userProfileImage;
    [SerializeField] private TextMeshProUGUI userNameText;

    public void InitUserSlot(string userName)
    {
        userProfileImage.color = new Color(1f, 1f, 1f);
        userNameText.text = userName;
    }

    public void GameOver()
    {
        userProfileImage.color = new Color(0.2f, 0.2f, 0.2f);
    }
}
