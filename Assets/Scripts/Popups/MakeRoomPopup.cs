﻿using UnityEngine;
using UnityEngine.UI;

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

        NetworkManager.Instance.CreateRoom(roomNameInputField.text, maxPlayerNum);
        ClosePopup();
    }
}
