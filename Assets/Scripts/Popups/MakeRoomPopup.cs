using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MakeRoomPopup : YesNoPopup
{
    [SerializeField] private InputField roomNameInputField;
    [SerializeField] private InputField maxPlayerNumInputField;

    private InputFieldUtility inputFieldUtility;

    protected override void Start()
    {
        inputFieldUtility = GetComponent<InputFieldUtility>();
        inputFieldUtility.EnterAction = OnClickMakeRoomBtn;

        roomNameInputField.ActivateInputField();

        yesBtn.onClick.AddListener(OnClickMakeRoomBtn);
        noBtn.onClick.AddListener(ClosePopup);
    }

    private void OnClickMakeRoomBtn()
    {
        byte maxPlayerNum;

        #region 인풋 필드 유효성 검사
        if (string.IsNullOrWhiteSpace(roomNameInputField.text))
        {
            CreateErrorPopup("Failed Make Room", "Room Name is Essential.");
            return;
        }

        if (string.IsNullOrWhiteSpace(maxPlayerNumInputField.text))
        {
            CreateErrorPopup("Failed Make Room", "Player Num is Essential.");
            return;
        }

        maxPlayerNum = byte.Parse(maxPlayerNumInputField.text);

        if (1 > maxPlayerNum || 4 < maxPlayerNum)
        {
            CreateErrorPopup("Failed Make Room", "Player Num must be between 1 and 4");
            return;
        }
        #endregion


        RoomOptions roomOption = new RoomOptions
        {
            PublishUserId = true, // 룸 유저가 같은 방에 있는 유저의 UserID 확인할 수 있게 설정
            MaxPlayers = maxPlayerNum,
            CustomRoomProperties = new Hashtable()
            {
                { SRoomPropertyKey.ROOM_NAME, roomNameInputField.text },
                { SRoomPropertyKey.MASTER_CLIENT, UserManager.userName },
                { SRoomPropertyKey.MAP_NAME, SMapName.STADIUM },
                { SRoomPropertyKey.ROOM_STATE, SRoomState.PREPARING_GAME }
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
