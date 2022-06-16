using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class GameNamePopup : OKPopup
{
    [SerializeField] private InputField gameNameInputField;

    private void Start()
    {
        SetOKBtnAction(() =>
        {
            // 디스플레이 네임 설정
            var request = new UpdateUserTitleDisplayNameRequest { DisplayName = gameNameInputField.text };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request,
                (result) =>
                {
                    // 유저 네임[게임에서 표시되는 이름] 캐싱
                    UserManager.userName = gameNameInputField.text;
                    ClosePopup();
                },
                (error) => CreateInfoPopup("SettingGameName Failed", error)
                );
        });
    }
}
