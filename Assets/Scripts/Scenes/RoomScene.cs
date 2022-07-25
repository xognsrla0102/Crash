using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // 유저 입력을 TMP로 받다보면 간혹적으로 동적으로 폰트 생성이 안되서 제대로 안 나오는 문제가 있음
    [SerializeField] private Text roomNameText;

    [Header("슬롯들")]
    public UserSlot[] userSlots;
    [SerializeField] private MapSlot mapSlot;

    [HideInInspector] public bool isDoneInitRoom;

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
        gameReadyBtn.onClick.AddListener(OnClickGameReadyBtn);
        // changeMapBtn.onClick.AddListener();
        roomOptionBtn.onClick.AddListener(OnClickRoomOptionBtn);

        chatInputField.ActivateInputField();

        InitRoomAfterUpdateCustomProperties();
    }

    private void OnDestroy()
    {
        lobbyBtn.onClick.RemoveAllListeners();

        gameStartBtn.onClick.RemoveAllListeners();
        gameReadyBtn.onClick.RemoveAllListeners();

        changeMapBtn.onClick.RemoveAllListeners();

        roomOptionBtn.onClick.RemoveAllListeners();
    }

    private void InitRoomAfterUpdateCustomProperties()
    {
        float propertyDelayTime = 0.3f;
        MyRoomManager.SetRoomManager();
        Invoke("InitRoom", propertyDelayTime);
    }

    public void UpdateRoomAfterUpdateCustomProperties()
    {
        float propertyDelayTime = 0.3f;
        MyRoomManager.SetRoomManager();
        Invoke("UpdateRoom", propertyDelayTime);
    }

    private void InitRoom()
    {
        UserManager.Instance.InitUserManager();

        #region 슬롯 유저 번호 세팅 (자신의 유저 번호가 1일 경우 1 0 2 3 으로 슬롯 유저 번호 세팅)
        userSlots[0].slotUserNum = UserManager.Instance.slotUserNum;
        for (int slotIdx = 1, userNum = 0; slotIdx < userSlots.Length; slotIdx++, userNum++)
        {
            if (userNum == UserManager.Instance.slotUserNum)
            {
                userNum++;
            }
            userSlots[slotIdx].slotUserNum = userNum;
        }
        #endregion

        UpdateRoom();

        isDoneInitRoom = true;
    }

    private void UpdateRoom()
    {
        #region 유저 슬롯 세팅
        for (int i = 0; i < userSlots.Length; i++)
        {
            userSlots[i].InitEmptySlot();
        }

        // 잠금 슬롯 설정
        string[] lockedSlotNums = $"{PhotonNetwork.CurrentRoom.CustomProperties[SRoomPropertyKey.LOCKED_SLOT_NUMS]}".Split(',');
        foreach (var lockSlotNumText in lockedSlotNums)
        {
            int lockedSlotUserNum;
            if (int.TryParse(lockSlotNumText, out lockedSlotUserNum) == false)
            {
                continue;
            }

            foreach (var userSlot in userSlots)
            {
                if (userSlot.slotUserNum == lockedSlotUserNum)
                {
                    userSlot.IsLocked = true;
                    break;
                }
            }
        }

        // 레디 슬롯 설정
        string[] readySlotNums = $"{PhotonNetwork.CurrentRoom.CustomProperties[SRoomPropertyKey.READY_SLOT_NUMS]}".Split(',');
        foreach (var readySlotNumText in readySlotNums)
        {
            int readySlotUserNum;
            if (int.TryParse(readySlotNumText, out readySlotUserNum) == false)
            {
                continue;
            }

            foreach (var userSlot in userSlots)
            {
                if (userSlot.slotUserNum == readySlotUserNum)
                {
                    userSlot.IsReady = true;
                    break;
                }
            }
        }

        Player[] users = PhotonNetwork.PlayerList;
        for (int userIdx = 0; userIdx < users.Length; userIdx++)
        {
            Player nowUser = users[userIdx];
            EUserColorType userColorType = Utility.StringToEnum<EUserColorType>($"{nowUser.CustomProperties[SPlayerPropertyKey.COLOR_TYPE]}");
            int slotUserNum = (int)nowUser.CustomProperties[SPlayerPropertyKey.SLOT_USER_NUM];

            for (int slotIdx = 0; slotIdx < userSlots.Length; slotIdx++)
            {
                if (userSlots[slotIdx].slotUserNum == slotUserNum)
                {
                    userSlots[slotIdx].InitSlot(nowUser, userColorType);
                    break;
                }
            }
        }

        #endregion

        mapSlot.InitSlot(MyRoomManager.mapName);

        roomNameText.text = MyRoomManager.roomName;

        #region 자신이 방장인지 아닌지에 따라 보이는 버튼 구분
        gameStartBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        gameReadyBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient == false);

        changeMapBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        roomOptionBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        #endregion
    }

    private void OnClickLobbyBtn()
    {
        // 레디 중인 경우 해제
        if (userSlots[0].IsReady)
        {
            userSlots[0].SetReadySlot();
        }

        NetworkManager.Instance.LeaveRoom();
    }

    private void OnClickGameStartBtn()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            Debug.Assert(false, "마스터 클라이언트가 아닙니다");
            return;
        }
        
        if (PhotonNetwork.PlayerListOthers.Length == 0)
        {
            Popup.CreateErrorPopup("Game Start Failed", "There is no one in the room.\nYou can start the game with a minimum of 2 players.");
            return;
        }

        // 들어온 모든 유저가 준비중인지 확인
        for (int i = 1; i < userSlots.Length; i++)
        {
            // 비어있는 슬롯은 무시
            if (userSlots[i].IsEmptySlot)
            {
                continue;
            }

            // 1명이라도 준비가 안되있다면 게임 시작 실패
            if (userSlots[i].IsReady == false)
            {
                Popup.CreateErrorPopup("Game Start Failed", "Someone doesn't ready yet.");
                return;
            }
        }

        NetworkManager.Instance.StartGame();
    }

    private void OnClickGameReadyBtn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Assert(false, "마스터 클라이언트일 땐 레디할 수 없습니다.");
            return;
        }

        userSlots[0].SetReadySlot();
    }

    private void OnClickRoomOptionBtn() => Popup.CreateSpecialPopup(EPopupType.ROOM_OPTION_POPUP);
}
