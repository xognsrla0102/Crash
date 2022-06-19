using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.SharedModels;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class NetworkManager : Singleton<NetworkManager>
{
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

    private LoginManager loginManager => FindObjectOfType<LoginManager>();

    private void Start()
    {
        // 호스트가 씬을 이동할 때, 다른 클라이언트들도 씬을 이동하게 하면서 동시에, 씬을 동기화시켜줌.
        // (서로 씬이 달라서 같은 포톤 뷰 개체를 못 찾아서 RPC함수 호출이 씹히는 문제를 막을 수 있음[RPC 손실 방지])
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region 플레이팹 코드
    private void SendRequest<RequestType, ResultType>(
        Action<RequestType, Action<ResultType>, Action<PlayFabError>, object, Dictionary<string, string>> playFabAPI,
        RequestType request, Action<ResultType> OnSuccess, Action<PlayFabError> OnFailed
        )
    {
        CanvasGroup.interactable = false;

        // 이벤트 호출하기 전, ui를 다시 활성화 시켜줌
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
                Popup.CreateInfoPopup("Register Failed", error);
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
                Popup.CreateInfoPopup("Login Failed", error);
                loginManager.FailedLogin();
            }
        );
    }

    // 디스플레이 이름 존재 유무 확인
    public void CheckUserGameName()
    {
        var request = new GetAccountInfoRequest() { Username = EncryptPlayerPrefs.GetString(PrefsKeys.USER_NAME) };

        SendRequest<GetAccountInfoRequest, GetAccountInfoResult>(
            PlayFabClientAPI.GetAccountInfo, request,
            (accountResult) =>
            {
                string displayName = accountResult.AccountInfo.TitleInfo.DisplayName;
                if (string.IsNullOrEmpty(displayName))
                {
                    var popup = Popup.CreatePopup(EPopupType.GAME_NAME_POPUP);
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
    public void ConnectMasterServer()
    {
        CanvasGroup.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("마스터 서버에 연결 완료");
        PhotonNetwork.NickName = UserManager.userName;

        // 로비 서버 접속
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("로비 서버 접속 완료");
        CanvasGroup.interactable = true;
        LoadingManager.LoadScene(ESceneName.LOBBY_SCENE);
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        print($"연결 끊김. 이유[{cause}]");

        CanvasGroup.interactable = true;

        switch (cause)
        {
            case DisconnectCause.DisconnectByClientLogic:
                print("타이틀로 이동");
                LoadingManager.LoadScene(ESceneName.TITLE_SCENE);
                break;
            default:
                Popup.CreateInfoPopup("Disconnected", $"{cause}");
                break;
        }
    }
    #endregion
}
