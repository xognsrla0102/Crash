﻿using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
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
    private bool isLocked;
    public bool IsLocked
    {
        get => isLocked;
        set
        {
            isLocked = value;

            if (isMyUserSlot == false)
            {
                xText.SetActive(isLocked);
                lockedImg.SetActive(isLocked);
            }
        }
    }

    [HideInInspector] public EUserColorType userColorType;
    [HideInInspector] public Button userSlotBtn;
    [HideInInspector] public Player userInfo;
    [HideInInspector] public int slotUserNum;

    [SerializeField] private bool isMyUserSlot;
    [SerializeField] private Transform modelParent;
    [SerializeField] private ChatBox userChatBox;
    [SerializeField] private TextMeshProUGUI userNameText;

    [Header("방장만 컨트롤 가능한 오브젝트들")]
    public Button makeMasterBtn;
    [SerializeField] private GameObject masterText;
    [SerializeField] private GameObject xText;
    [SerializeField] private GameObject lockedImg;

    public bool IsEmptySlot => userInfo == null;

    private void Awake()
    {
        if (isMyUserSlot == false)
        {
            userSlotBtn = GetComponent<Button>();

            UserSlot[] userSlots = FindObjectOfType<RoomScene>().userSlots;
            userSlotBtn.onClick.AddListener(() => OnClickUserSlotBtn(userSlots));

            makeMasterBtn.onClick.AddListener(OnClickMakeMasterBtn);

            xText.SetActive(false);
            lockedImg.SetActive(false);
            makeMasterBtn.gameObject.SetActive(false);
        }

        masterText.SetActive(false);
    }

    private void OnDestroy()
    {
        if (isMyUserSlot == false)
        {
            userSlotBtn.onClick.RemoveAllListeners();
            makeMasterBtn.onClick.RemoveAllListeners();
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

        if (isMyUserSlot == false)
        {
            userSlotBtn.enabled = PhotonNetwork.IsMasterClient;
            makeMasterBtn.gameObject.SetActive(false);
        }
    }

    public void InitSlot(Player user, EUserColorType userColorType)
    {
        print($"user:{user.NickName}, color:{userColorType}");
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

        if (isMyUserSlot == false)
        {
            userSlotBtn.enabled = PhotonNetwork.IsMasterClient;
            makeMasterBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        }
    }

    public void ShowChatEffect(string msg) => userChatBox.ShowChatEffect(msg);

    private void OnClickUserSlotBtn(UserSlot[] userSlots)
    {
        Debug.Assert(PhotonNetwork.IsMasterClient, "방장이 아닙니다.");

        if (IsEmptySlot)
        {
            string lockedSlotNums = $"{PhotonNetwork.CurrentRoom.CustomProperties[SRoomPropertyKey.LOCKED_SLOT_NUMS]}";

            int lockedSlotCnt = 0;
            foreach (var userSlot in userSlots)
            {
                if (userSlot.isLocked)
                {
                    lockedSlotCnt++;
                }
            }

            // 잠겨 있으면 해제
            if (IsLocked)
            {
                lockedSlotNums = lockedSlotNums.Replace($"{slotUserNum},", "");
                lockedSlotCnt--;
            }
            // 안 잠겨있으면 잠금
            else
            {
                lockedSlotNums += $"{slotUserNum},";
                lockedSlotCnt++;
            }

            PhotonNetwork.CurrentRoom.MaxPlayers = (byte)(4 - lockedSlotCnt);
            NetworkManager.Instance.SetRoomProperties(SRoomPropertyKey.LOCKED_SLOT_NUMS, lockedSlotNums);
        }
        else
        {
            // 유저 강퇴 팝업 생성
            YesNoPopup popup = Popup.CreateNormalPopup("Kick User", $"Do you want to <color=red>kick</color> [\"{userNameText.text}\"]?", EPopupType.YES_NO_POPUP) as YesNoPopup;
            popup.SetYesBtnAction(() =>
            {
                NetworkManager.Instance.KickUser(userInfo.UserId);
                popup.ClosePopup();
            });
        }
    }

    private void OnClickMakeMasterBtn()
    {
        Debug.Assert(PhotonNetwork.IsMasterClient, "방장이 아닙니다.");

        YesNoPopup popup = Popup.CreateNormalPopup("Make Master", $"Do you want to make [\"{userNameText.text}\"] as <color=green>a Master</color>?", EPopupType.YES_NO_POPUP) as YesNoPopup;
        popup.SetYesBtnAction(() =>
        {
            NetworkManager.Instance.SetMasterClient(userInfo);
            popup.ClosePopup();
        });
    }
}
