using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : Singleton<NetworkManager>
{
    #region 멤버 변수 및 MonoBehaviour 함수
    private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (canvasGroup == null)
            {
                canvasGroup = FindObjectOfType<CanvasGroup>();
                Debug.Assert(canvasGroup != null, "캔버스 그룹을 찾지 못했습니다.");
            }
            return canvasGroup;
        }
    }

    private Transform chatContent;
    private Transform ChatContent
    {
        get
        {
            if (chatContent == null)
            {
                chatContent = GameObject.Find("UI").transform.Find("ChatField")
                    .Find("ScrollView").Find("Viewport").Find("Content");
            }
            return chatContent;
        }
    }

    private LoginManager loginManager => FindObjectOfType<LoginManager>();

    private void Start()
    {
        // 호스트가 씬을 이동할 때, 다른 클라이언트들도 씬을 이동하게 하면서 동시에, 씬을 동기화시켜줌.
        // (서로 씬이 달라서 같은 포톤 뷰 개체를 못 찾아서 RPC함수 호출이 씹히는 문제를 막을 수 있음[RPC 손실 방지])
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion

    #region 플레이팹 코드
    private void SendRequest<RequestType, ResultType>(
        Action<RequestType, Action<ResultType>, Action<PlayFabError>, object, Dictionary<string, string>> playFabAPI,
        RequestType request, Action<ResultType> OnSuccess, Action<PlayFabError> OnFailed
        )
    {
        CanvasGroup.interactable = false;

        // 이벤트를 호출하기 전, ui를 다시 활성화 시켜줌
        Action<ResultType> ResultTypeActiveUI = (result) => { CanvasGroup.interactable = true; };
        Action<PlayFabError> ErrorActiveUI = (error) => { CanvasGroup.interactable = true; };

        playFabAPI(request, ResultTypeActiveUI + OnSuccess, ErrorActiveUI + OnFailed, null, null);
    }

    public void RegisterAccount(string userName, string pw)
    {
        // 닉네임(ID으로 쓰임), 비번만으로 회원가입 요청
        var request = new RegisterPlayFabUserRequest { Username = userName, Password = pw, RequireBothUsernameAndEmail = false };

        SendRequest<RegisterPlayFabUserRequest, RegisterPlayFabUserResult>(
            PlayFabClientAPI.RegisterPlayFabUser, request,
            (result) =>
            {
                print($"회원가입 성공! : {result}");
                loginManager.SuccessRegister();
            },
            (error) =>
            {
                Popup.CreateErrorPopup("Register Failed", error);
                print($"회원가입 실패 이유 : {error}");
            }
        );
    }

    public void LoginAccount(string userName, string pw)
    {
        var request = new LoginWithPlayFabRequest { Username = userName, Password = pw };

        SendRequest<LoginWithPlayFabRequest, LoginResult>(
            PlayFabClientAPI.LoginWithPlayFab, request,
            (result) =>
            {
                print($"로그인 성공! : {result}");
                loginManager.SuccessLogin();
            },
            (error) =>
            {
                print($"로그인 실패 이유 : {error}");
                Popup.CreateErrorPopup("Login Failed", error);
                loginManager.FailedLogin();
            }
        );
    }

    // 디스플레이 이름 존재 유무 확인
    public void CheckUserGameName()
    {
        var request = new GetAccountInfoRequest() { Username = EncryptPlayerPrefs.GetString(SPrefsKey.USER_NAME) };

        SendRequest<GetAccountInfoRequest, GetAccountInfoResult>(
            PlayFabClientAPI.GetAccountInfo, request,
            (accountResult) =>
            {
                string displayName = accountResult.AccountInfo.TitleInfo.DisplayName;
                if (string.IsNullOrEmpty(displayName))
                {
                    var popup = Popup.CreateSpecialPopup(EPopupType.GAME_NAME_POPUP);
                }
                else
                {
                    // 유저 네임[게임에서 표시되는 이름] 캐싱
                    UserManager.userName = displayName;
                }
            },
            (error) =>
            {
                print("유저 정보를 받지 못했습니다.");
                Debug.Assert(false);
            });
    }

    // 로그인 API 사용을 허용하는 클라이언트 세션 토큰 삭제
    public void LogoutAccount() => PlayFabClientAPI.ForgetAllCredentials();
    #endregion

    #region 포톤 코드

    #region 서버 연결 코드
    public void ConnectMasterServer()
    {
        CanvasGroup.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("마스터 서버에 연결 완료");
        CanvasGroup.interactable = true;

        PhotonNetwork.NickName = UserManager.userName;
        LoadingManager.LoadScene(SSceneName.LOBBY_SCENE);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print($"연결 끊김. 이유[{cause}]");

        CanvasGroup.interactable = true;

        switch (cause)
        {
            case DisconnectCause.DisconnectByClientLogic:
                print("타이틀로 이동");
                LoadingManager.LoadScene(SSceneName.TITLE_SCENE);
                break;
            default:
                OKPopup popup = Popup.CreateErrorPopup("Server Disconnected", $"{cause}") as OKPopup;
                popup.SetOKBtnAction(() =>
                {
                    print("서버 끊김으로 인한 타이틀 씬 이동");
                    LoadingManager.LoadScene(SSceneName.TITLE_SCENE);
                });
                break;
        }
    }
    #endregion

    #region 로비 관련 코드
    // 로비 서버 접속
    public void JoinLobby()
    {
        CanvasGroup.interactable = false;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("로비 서버 접속 완료");
        CanvasGroup.interactable = true;
    }

    public void LeaveLobby()
    {
        CanvasGroup.interactable = false;
        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        print("로비 떠남");
        PhotonNetwork.Disconnect();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 로비가 아닐 경우 무시
        if (PhotonNetwork.InLobby == false)
        {
            return;
        }

        print("방 리스트 업데이트");
        LobbyScene lobbyScene = FindObjectOfType<LobbyScene>();
        int roomCnt = roomList.Count;

        for (int roomIdx = 0; roomIdx < roomCnt; roomIdx++)
        {
            print($"방[{roomIdx}] : {roomList[roomIdx]}");

            int findRoomIdx = lobbyScene.roomInfos.IndexOf(roomList[roomIdx]);

            // 해당 방이 존재하는 경우
            if (roomList[roomIdx].RemovedFromList == false)
            {
                // 실제 리스트에 존재한다면 갱신
                if (lobbyScene.roomInfos.Contains(roomList[roomIdx]))
                {
                    lobbyScene.roomInfos[findRoomIdx] = roomList[roomIdx];
                }
                // 실제 리스트에 존재하지 않으면 추가
                else
                {
                    lobbyScene.roomInfos.Add(roomList[roomIdx]);
                }
            }
            // 해당 방이 삭제 처리됐지만(removedFromList == true), 실제 리스트에는 제거되지 않은 경우 제거
            else if (findRoomIdx != -1)
            {
                lobbyScene.roomInfos.RemoveAt(findRoomIdx);
            }
        }

        // 방 슬롯도 갱신
        lobbyScene.UpdateRoomSlot();
    }
    #endregion

    #region 방 관련 코드

    // 주의 사항
    // 방 생성 및 조인은 룸 씬으로 먼저 이동한 후에 Photon.CreateRoom, JoinRoom 시도를 함
    // 포톤에서 먼저 CreateRoom, JoinRoom 처리를 하게 되면, 룸 씬 이동 전에, 로딩 씬에서 룸 관련 이벤트 메시지를 제대로 처리할 수 없기 때문

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Popup.CreateErrorPopup("Failed CreateRoom", $"Error Code : {returnCode}\nMessage : {message}");
        Debug.Log($"방 생성 실패 :\n코드 : {returnCode}\n메세지 : {message}");
    }

    public void JoinRoom(string roomName) => PhotonNetwork.JoinRoom(roomName);

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Popup.CreateErrorPopup("Failed Join Room", $"Error Code : {returnCode}\nMessage : {message}");
        Debug.Log($"방 참가 실패 :\n코드 : {returnCode}\n메세지 : {message}");
    }

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Popup.CreateErrorPopup("Failed Join Random Room", $"Error Code : {returnCode}\nMessage : {message}");
        Debug.Log($"랜덤으로 방 참가 실패 :\n코드 : {returnCode}\n메세지 : {message}");
    }

    // CreateRoom 함수 호출 시에도 OnCreateRoom 함수 호출 뒤 이곳으로 들어옴
    public override void OnJoinedRoom()
    {
        print($"방 참가 완료");
        LoadingManager.LoadScene(SSceneName.ROOM_SCENE);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SendSystemChat($"{newPlayer.NickName}님이 참가하였습니다.");
        RoomScene roomScene = FindObjectOfType<RoomScene>();
        roomScene.UpdateRoomAfterUpdateCustomProperties();
    }

    public void LeaveRoom()
    {
        // 방이 아니라면 무시
        if (PhotonNetwork.InRoom == false)
        {
            return;
        }

        CanvasGroup.interactable = false;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        CanvasGroup.interactable = true;
        MyRoomManager.ClearRoomManager();
        print("방 떠남, 게임 서버 연결 해제 후 마스터 서버 접속 시도..");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SendSystemChat($"{otherPlayer.NickName}님이 떠났습니다.");
        RoomScene roomScene = FindObjectOfType<RoomScene>();
        roomScene.UpdateRoomAfterUpdateCustomProperties();
    }

    public void SendChat(string msg)
    {
        photonView.RPC("AddChatBoxRPC", RpcTarget.All,
            msg,
            PhotonNetwork.NickName,
            Utility.StringToEnum<EUserColorType>($"{PhotonNetwork.LocalPlayer.CustomProperties[SPlayerPropertyKey.COLOR_TYPE]}")
        );
    }

    public void SendSystemChat(string msg) => AddChatBox($"<color=red>{msg}</color>");

    // 채팅 메시지와 말한 유저의 색상 정보를 전달
    [PunRPC] private void AddChatBoxRPC(string msg, string userName, EUserColorType userColorType) => AddChatBox(msg, userName, userColorType);

    private void AddChatBox(string msg, string userName = "", EUserColorType userColorType = EUserColorType.NONE)
    {
        // 이미 채팅 슬롯이 보여줄 슬롯만큼 생성되었다면
        if (ChatContent.childCount == ChatBoxPoolManager.SHOW_CHAT_BOX_CNT)
        {
            // 가장 위에 있던 채팅 슬롯을 풀링 오브젝트에 다시 넣음
            ChatBoxPoolManager.Instance.Push(ChatContent.GetChild(0).GetComponent<ChatBox>());
        }

        ChatBox chatBox = ChatBoxPoolManager.Instance.Pop(ChatContent);
        chatBox.SetText(string.IsNullOrWhiteSpace(userName) ? msg : $"[{userName}] : {msg}");

        if (userColorType == EUserColorType.NONE)
        {
            return;
        }

        // 현재 방에서 말한 유저의 슬롯에 있는 채팅 이펙트를 보여줌
        RoomScene roomScene = FindObjectOfType<RoomScene>();
        foreach (var userSlot in roomScene.userSlots)
        {
            if (userSlot.userColorType == userColorType)
            {
                userSlot.ShowChatEffect(msg);
                break;
            }
        }
    }
    #endregion

    #region 방 설정 관련 코드
    public void SetRoomProperties(Hashtable roomProperty)
    {
        Debug.Assert(PhotonNetwork.IsMasterClient, "방장이 아닙니다.");
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperty);
    }

    // 룸 프로퍼티 변경되었을 때
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        StringBuilder sb = new StringBuilder(256);
        foreach (var property in propertiesThatChanged)
        {
            sb.AppendLine($"key : {property.Key}, value : {property.Value}");
        }
        print($"방 정보가 변경되었습니다.\n{sb}");

        RoomScene roomScene = FindObjectOfType<RoomScene>();
        if (roomScene != null)
        {
            roomScene.UpdateRoomAfterUpdateCustomProperties();
        }
    }

    public void SetMasterClient(Player player)
    {
        Debug.Assert(PhotonNetwork.IsMasterClient, "방장이 아닙니다.");
        print($"방장을 [\"{player.NickName}\"] 유저로 변경 시도");
        PhotonNetwork.SetMasterClient(player);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        print($"방장이 {newMasterClient.NickName}(으)로 변경되었습니다.");

        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        roomProperties[SRoomPropertyKey.MASTER_CLIENT] = newMasterClient.NickName;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        SendSystemChat($"방장이 {newMasterClient.NickName}(으)로 변경되었습니다.");
    }

    public void KickUser(string userID) => photonView.RPC("KickUserRPC", RpcTarget.All, userID);

    [PunRPC] private void KickUserRPC(string userID)
    {
        if (PhotonNetwork.LocalPlayer.UserId.Equals(userID) == false)
        {
            return;
        }

        LeaveRoom();
    }
    #endregion

    #endregion
}
