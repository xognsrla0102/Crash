using UnityEngine;
using TMPro;

public enum EUserColorType
{
    RED,
    YELLOW,
    GREEN,
    BLUE
}

public class UserSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private GameObject userChatBox;

    private EUserColorType userColorType;

    public void InitSlot(string userName, EUserColorType userColorType)
    {
        userNameText.text = userName;

        this.userColorType = userColorType;
    }

    public void ActiveUserChatBox(string msg)
    {
        // userChatBox.
    }
}
