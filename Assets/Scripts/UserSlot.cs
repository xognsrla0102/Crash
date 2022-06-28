using UnityEngine;
using TMPro;

public enum EUserColorType
{
    NONE,
    RED,
    YELLOW,
    GREEN,
    BLUE,
    NUMS
}

public class UserSlot : MonoBehaviour
{
    [HideInInspector] public EUserColorType userColorType;

    [SerializeField] private Transform modelParent;
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private ChatBox userChatBox;

    public void InitEmptySlot()
    {
        userNameText.text = "Empty";
        userColorType = EUserColorType.NONE;

        // 모델링 비활성화
        for (int i = 0; i < modelParent.childCount; i++)
        {
            modelParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void InitSlot(string userName, EUserColorType userColorType)
    {
        userNameText.text = userName;
        this.userColorType = userColorType;

        // ColorType에 해당하는 모델링만 활성화
        int userColorNum = (int)userColorType - 1;
        for (int i = 0; i < modelParent.childCount; i++)
        {
            modelParent.GetChild(i).gameObject.SetActive(i == userColorNum);
        }
    }

    public void ShowChatEffect(string msg) => userChatBox.ShowChatEffect(msg);
}
