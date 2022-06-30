using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;

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
    [HideInInspector] public bool isLocked;

    [SerializeField] private bool isMyUserSlot;

    [SerializeField] private Transform modelParent;
    [SerializeField] private ChatBox userChatBox;

    [SerializeField] private TextMeshProUGUI userNameText;

    [Header("방장만 컨트롤 가능한 오브젝트들")]
    [SerializeField] private GameObject masterText;
    [SerializeField] private GameObject xText;
    [SerializeField] private GameObject lockedImg;

    private Button userSlotBtn;
    private Player userInfo;

    private void Awake()
    {
        if (isMyUserSlot == false)
        {
            userSlotBtn = GetComponent<Button>();
            userSlotBtn.onClick.AddListener(OnClickUserSlotBtn);

            xText.SetActive(false);
            lockedImg.SetActive(false);
        }

        masterText.SetActive(false);
    }

    private void OnDestroy()
    {
        if (isMyUserSlot == false)
        {
            userSlotBtn.onClick.RemoveAllListeners();
        }
    }

    public void InitEmptySlot()
    {
        userColorType = EUserColorType.NONE;

        userNameText.text = "Empty";
        masterText.SetActive(false);

        userInfo = null;

        // 모델링 비활성화
        for (int i = 0; i < modelParent.childCount; i++)
        {
            modelParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void InitSlot(Player user, EUserColorType userColorType)
    {
        userInfo = user;

        // 해당 슬롯의 유저가 방장 유저라면 마스터 텍스트 활성화
        bool isMasterUserSlot = PhotonNetwork.MasterClient.UserId.Equals(userInfo.UserId);
        masterText.SetActive(isMasterUserSlot);

        userNameText.text = userInfo.NickName;
        this.userColorType = userColorType;

        // ColorType에 해당하는 모델링만 활성화
        int userColorNum = (int)userColorType - 1;
        for (int i = 0; i < modelParent.childCount; i++)
        {
            modelParent.GetChild(i).gameObject.SetActive(i == userColorNum);
        }
    }

    public void SetLockSlot(bool isLocked)
    {
        this.isLocked = isLocked;
        xText.SetActive(isLocked);
        lockedImg.SetActive(isLocked);
    }

    public void ShowChatEffect(string msg) => userChatBox.ShowChatEffect(msg);

    private void OnClickUserSlotBtn()
    {
        // 방장이 아닌 경우 무시
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }

        // 유저가 비어있는 슬롯인 경우
        if (userNameText.text.Equals("Empty"))
        {
            // 현재 잠김 상태를 반전하여 세팅
            SetLockSlot(!isLocked);

            // 방 상태 갱신
            if (isLocked)
            {
                PhotonNetwork.CurrentRoom.MaxPlayers--;
            }
            else
            {
                PhotonNetwork.CurrentRoom.MaxPlayers++;
            }
        }
        else
        {
            // 유저 강퇴 팝업 생성
            YesNoPopup popup = Popup.CreateNormalPopup("Kick User Popup", $"Do you want to kick [\"{userNameText.text}\"]?", EPopupType.YES_NO_POPUP) as YesNoPopup;
            popup.SetYesBtnAction(() =>
            {
                NetworkManager.Instance.KickUser(userInfo.UserId);
                popup.ClosePopup();
            });
        }
    }
}
