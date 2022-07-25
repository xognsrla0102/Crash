using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MakeRoomPopup : YesNoPopup
{
    [SerializeField] private InputField roomNameInputField;
    [SerializeField] private Toggle[] playerNumToggles;

    private InputFieldUtility inputFieldUtility;

    // 현재 방의 최대 인원 수
    private byte maxPlayerNum = 2;

    protected override void Start()
    {
        inputFieldUtility = GetComponent<InputFieldUtility>();
        inputFieldUtility.EnterAction = OnClickMakeRoomBtn;

        roomNameInputField.ActivateInputField();

        for (int i = 0; i < 3; i++)
        {
            byte toggleNum = (byte)(i + 2);
            playerNumToggles[i].onValueChanged.AddListener((isOn) =>
                {
                    if (isOn == false)
                    {
                        return;
                    }
                    OnClickPlayerNumToggle(toggleNum);
                });
        }

        yesBtn.onClick.AddListener(OnClickMakeRoomBtn);
        noBtn.onClick.AddListener(ClosePopup);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        foreach (var toggle in playerNumToggles)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }

    private void OnClickPlayerNumToggle(byte toggleNum)
    {
        maxPlayerNum = toggleNum;
        print($"방 최대 인원 : {maxPlayerNum}");
    }

    private void OnClickMakeRoomBtn()
    {
        #region 인풋 필드 유효성 검사
        if (string.IsNullOrWhiteSpace(roomNameInputField.text))
        {
            CreateErrorPopup("Failed Make Room", "Room Name is Essential.");
            return;
        }
        #endregion

        // 최대 방에 있을 수 있는 유저 수
        int limitPlayerNum = 4;

        // 현재 방에 있을 수 있는 유저 수보다 최대 인원 수가 작다면 그만큼 뒤 슬롯부터 잠금 처리 
        string lockedSlots = string.Empty;
        for (int i = 0; i < limitPlayerNum - maxPlayerNum; i++)
        {
            lockedSlots += $"{limitPlayerNum - i - 1},"; 
        }

        RoomOptions roomOption = new RoomOptions
        {
            PublishUserId = true, // 룸 유저가 같은 방에 있는 유저의 UserID 확인할 수 있게 설정
            MaxPlayers = maxPlayerNum,
            CustomRoomProperties = new Hashtable()
            {
                { SRoomPropertyKey.ROOM_NAME, roomNameInputField.text },
                { SRoomPropertyKey.MASTER_CLIENT, UserManager.Instance.userName },
                { SRoomPropertyKey.MAP_NAME, SMapName.STADIUM },
                { SRoomPropertyKey.ROOM_STATE, SRoomState.PREPARING_GAME },
                { SRoomPropertyKey.LOCKED_SLOT_NUMS, lockedSlots },
                { SRoomPropertyKey.READY_SLOT_NUMS, string.Empty }
            },
            CustomRoomPropertiesForLobby = new string[]
            {
                SRoomPropertyKey.ROOM_NAME,
                SRoomPropertyKey.MASTER_CLIENT,
                SRoomPropertyKey.MAP_NAME,
                SRoomPropertyKey.ROOM_STATE
            },
        };

        // 룸 ID 는 방 생성자의 userID와 생성 시간을 넣음
        PhotonNetwork.CreateRoom($"{PhotonNetwork.LocalPlayer.UserId}_{DateTime.UtcNow.ToFileTime()}", roomOption);
        ClosePopup();
    }
}
