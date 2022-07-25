﻿using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class GameNamePopup : OKPopup
{
    [SerializeField] private InputField gameNameInputField;

    private InputFieldUtility inputFieldUtility;

    protected override void Start()
    {
        base.Start();

        gameNameInputField.ActivateInputField();

        inputFieldUtility = GetComponent<InputFieldUtility>();
        inputFieldUtility.EnterAction = OkAction;
        SetOKBtnAction(OkAction);
    }

    private void OkAction()
    {
        // 디스플레이 네임 설정
        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = gameNameInputField.text };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            (result) =>
            {
                // 유저 네임[게임에서 표시되는 이름] 캐싱
                UserManager.Instance.userName = gameNameInputField.text;
                ClosePopup();
            },
            (error) => CreateErrorPopup("ChangeGameName Failed", error)
            );
    }
}
