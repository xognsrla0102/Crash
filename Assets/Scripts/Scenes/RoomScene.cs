using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RoomScene : MonoBehaviour
{
    [Header("방 버튼들")]
    [SerializeField] private Button lobbyBtn;
    [SerializeField] private Button roomOptionBtn;

    [SerializeField] private Button gameStartBtn;
    [SerializeField] private Button gameReadyBtn;
    [SerializeField] private Button changeMapBtn;

    [Header("인풋필드, 텍스트들")]
    [SerializeField] private InputField chatInputField;

    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI roomNameText;

    [Header("슬롯들")]
    public UserSlot[] userSlots;
    [SerializeField] private MapSlot mapSlot;

    private InputFieldUtility inputFieldUtility;

    public void Start()
    {
        inputFieldUtility = GetComponent<InputFieldUtility>();
        inputFieldUtility.EnterAction = () =>
        {
            if (string.IsNullOrWhiteSpace(chatInputField.text) == false)
            {
                // 채팅 보낸 후 인풋필드 비움
                NetworkManager.Instance.SendChat(chatInputField.text);
                chatInputField.text = string.Empty;

                // 비활성화된 인풋필드 다시 활성화
                chatInputField.ActivateInputField();
            }
        };

        lobbyBtn.onClick.AddListener(OnClickLobbyBtn);
        gameStartBtn.onClick.AddListener(OnClickGameStartBtn);
        roomOptionBtn.onClick.AddListener(OnClickRoomOptionBtn);

        chatInputField.ActivateInputField();

        UpdateRoomAfterUpdateCustomProperties(true);
    }

    private void OnDestroy()
    {
        lobbyBtn.onClick.RemoveAllListeners();

        gameStartBtn.onClick.RemoveAllListeners();
        gameReadyBtn.onClick.RemoveAllListeners();

        changeMapBtn.onClick.RemoveAllListeners();

        roomOptionBtn.onClick.RemoveAllListeners();
    }

    public void UpdateRoomAfterUpdateCustomProperties(bool isInitRoom = false)
    {
        float propertyDelayTime = 0.3f;

        if (isInitRoom)
        {
            MyRoomManager.InitRoomManager();
            Invoke("InitRoom", propertyDelayTime);
        }
        else
        {
            MyRoomManager.SetRoomManager();
            Invoke("UpdateRoom", propertyDelayTime);
        }
    }

    private void InitRoom()
    {
        // 잠긴 슬롯 세팅
        int[] lockedSlots = PhotonNetwork.CurrentRoom.CustomProperties[SRoomPropertyKey.LOCKED_SLOTS] as int[];
        foreach (var lockedSlotUserNum in lockedSlots)
        {
            userSlots[lockedSlotUserNum].IsLocked = true;
        }

        // 유저 번호 세팅
        userSlots[0].userNum = UserManager.myUserNum;
        for (int slotIdx = 1, userNum = 0; slotIdx < userSlots.Length; slotIdx++)
        {
            if (userSlots[slotIdx].IsLocked)
            {
                userSlots[slotIdx].userNum = -1;
                continue;
            }

            if (userNum == UserManager.myUserNum)
            {
                userNum++;
            }

            userSlots[slotIdx].userNum = userNum;
            userNum++;
        }

        UpdateRoom();
    }

    private void UpdateRoom()
    {
        #region 슬롯 세팅

        mapSlot.InitSlot(MyRoomManager.mapName);

        // 유저 번호에 맞는 색상 세팅
        Player[] users = PhotonNetwork.PlayerList;
        EUserColorType[] userColorTypes = new EUserColorType[userSlots.Length];
        for (int userNum = 0; userNum < users.Length; userNum++)
        {
            userColorTypes[userNum] = Utility.StringToEnum<EUserColorType>($"{users[userNum].CustomProperties[SPlayerPropertyKey.COLOR_TYPE]}");
        }

        // 유저 번호에 맞게 슬롯 세팅
        for (int i = 0; i < userSlots.Length; i++)
        {
            int nowUserNum = userSlots[i].userNum;
            if (nowUserNum != -1)
            {
                userSlots[i].InitSlot(users[nowUserNum], userColorTypes[nowUserNum]);
            }
            else
            {
                userSlots[i].InitEmptySlot();
            }
        }
        #endregion

        roomNameText.text = MyRoomManager.roomName;

        #region 자신이 방장인지 아닌지에 따라 보이는 버튼 구분
        gameStartBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        gameReadyBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient == false);

        changeMapBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        roomOptionBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        #endregion
    }

    private void OnClickLobbyBtn() => NetworkManager.Instance.LeaveRoom();

    private void OnClickGameStartBtn()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            Debug.Assert(false, "마스터 클라이언트가 아닙니다");
            return;
        }

        // 게임이 시작되므로 참여 못 하게 막음..
        //PhotonNetwork.CurrentRoom.IsOpen = false;

        // 룸 상태 변경해야 함
    }

    private void OnClickRoomOptionBtn() => Popup.CreateSpecialPopup(EPopupType.ROOM_OPTION_POPUP);
}
