using UnityEngine;
using TMPro;

public enum EUserColorType
{
    RED,
    YELLOW,
    GREEN,
    BLUE,
    NONE
}

public class UserSlot : MonoBehaviour
{
    [SerializeField] private Transform modelParent;
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private GameObject userChatBox;

    private EUserColorType userColorType;

    private void Awake()
    {
        userChatBox.SetActive(false);
    }

    public void InitEmptySlot()
    {
        userNameText.text = "Empty";

        // 모델링 비활성화
        modelParent.GetChild((int)userColorType).gameObject.SetActive(false);
        userColorType = EUserColorType.NONE;
    }

    public void InitSlot(string userName, EUserColorType userColorType)
    {
        userNameText.text = userName;
        this.userColorType = userColorType;

        // ColorType에 해당하는 모델링만 활성화
        int userColorNum = (int)userColorType;
        for (int i = 0; i < modelParent.childCount; i++)
        {
            modelParent.GetChild(i).gameObject.SetActive(i == userColorNum);
        }
    }

    public void ActiveUserChatBox(string msg)
    {
        // userChatBox.
    }
}
