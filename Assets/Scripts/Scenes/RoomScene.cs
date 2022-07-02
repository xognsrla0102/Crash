﻿using System;
using System.Collections;
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
    [SerializeField] private TextMeshProUGUI roomNameText;

    [Header("슬롯들")]
    public UserSlot[] userSlots;
    [SerializeField] private MapSlot mapSlot;

    private InputFieldUtility inputFieldUtility;

    private void Start()
    {
        // 서버로부터 방 정보 다 받을 때까지는 UI 비활성화
        StartCoroutine(SetInteractableUiCoroutine());

        switch (MyRoomManager.entryRoomState)
        {
            case EEntryRoomState.CREATE_ROOM:
                NetworkManager.Instance.CreateRoom();
                break;
            case EEntryRoomState.JOIN_ROOM:
                NetworkManager.Instance.JoinRoom();
                break;
            case EEntryRoomState.JOIN_RANDOM_ROOM:
                NetworkManager.Instance.JoinRandomRoom();
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }

    private IEnumerator SetInteractableUiCoroutine()
    {
        NetworkManager.Instance.CanvasGroup.interactable = false;
        yield return new WaitForSeconds(0.5f);
        NetworkManager.Instance.CanvasGroup.interactable = true;
    }

    public void InitRoomScene()
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

        // 채팅 인풋필드 활성화
        chatInputField.ActivateInputField();

        // 방 매니저 정보 갱신하고
        MyRoomManager.SetUserColorType();

        // 방 생성 시에는 RoomProperty가 업데이트되면서 이 함수가 자동 호출되므로 호출 안 함
        if (MyRoomManager.entryRoomState != EEntryRoomState.CREATE_ROOM)
        {
            UpdateRoomUntilUpdateCustomProperties();
        }
    }

    private void OnDestroy()
    {
        lobbyBtn.onClick.RemoveAllListeners();

        gameStartBtn.onClick.RemoveAllListeners();
        gameReadyBtn.onClick.RemoveAllListeners();

        changeMapBtn.onClick.RemoveAllListeners();

        roomOptionBtn.onClick.RemoveAllListeners();
    }

    // 방 갱신 (누군가 떠나거나, 들어왔을 때 호출)
    public void UpdateRoomUntilUpdateCustomProperties()
    {
        // 방 매니저 정보 갱신하고
        MyRoomManager.SetRoomManager();

        // 유저 커스텀 프로퍼티 갱신을 위해 대기
        Invoke("UpdateRoom", 0.1f);
    }

    private void UpdateRoom()
    {
        #region 슬롯 세팅

        mapSlot.InitSlot(MyRoomManager.mapName);

        // 나를 제외한 나머지 방 유저들
        Player[] otherUsers = PhotonNetwork.PlayerListOthers;

        // 유저 범퍼카 색상 대입
        EUserColorType[] userColorTypes = new EUserColorType[4];
        userColorTypes[0] = (EUserColorType)Enum.Parse(typeof(EUserColorType), $"{PhotonNetwork.LocalPlayer.CustomProperties[SPlayerPropertyKey.COLOR_TYPE]}");

        for (int i = 0; i < otherUsers.Length; i++)
        {
            userColorTypes[i + 1] = (EUserColorType)Enum.Parse(typeof(EUserColorType), $"{otherUsers[i].CustomProperties[SPlayerPropertyKey.COLOR_TYPE]}");
        }

        // 자기 자신 슬롯 세팅
        userSlots[0].InitSlot(PhotonNetwork.LocalPlayer, userColorTypes[0]);

        // 방에 있는 다른 사람 슬롯 세팅
        int otherUserIdx;
        for (otherUserIdx = 0; otherUserIdx < otherUsers.Length; otherUserIdx++)
        {
            // 자기 자신은 이미 했으므로 1번째 슬롯부터 시작
            userSlots[otherUserIdx + 1].InitSlot(otherUsers[otherUserIdx], userColorTypes[otherUserIdx + 1]);
        }

        int playerCnt = PhotonNetwork.PlayerList.Length;
        for (int emptySlotIdx = playerCnt; emptySlotIdx < userSlots.Length; emptySlotIdx++)
        {
            // 빈 슬롯 세팅
            userSlots[emptySlotIdx].InitEmptySlot();
            // 슬롯이 방 최대 인원을 넘길 경우 잠김 슬롯으로 초기화
            userSlots[emptySlotIdx].SetLockSlot(emptySlotIdx >= PhotonNetwork.CurrentRoom.MaxPlayers);
        }
        #endregion

        roomNameText.text = MyRoomManager.roomName;

        #region 자신이 방장인지 아닌지에 따라 보이는 버튼 구분
        gameStartBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        gameReadyBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient == false);

        changeMapBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        roomOptionBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        for (int i = 1; i < userSlots.Length; i++)
        {
            userSlots[i].userSlotBtn.enabled = PhotonNetwork.IsMasterClient;

            bool visibleMakeMasterBtn = PhotonNetwork.IsMasterClient && userSlots[i].IsEmptySlot == false;
            userSlots[i].makeMasterBtn.gameObject.SetActive(visibleMakeMasterBtn);
        }
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
