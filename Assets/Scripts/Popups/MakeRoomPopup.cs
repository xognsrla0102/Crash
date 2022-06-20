﻿using UnityEngine;
using UnityEngine.UI;

public class MakeRoomPopup : YesNoPopup
{
    [SerializeField] private InputField roomNameInputField;
    [SerializeField] private InputField maxPlayerNumInputField;

    protected override void Start()
    {
        yesBtn.onClick.AddListener(OnClickMakeRoomBtn);
        noBtn.onClick.AddListener(ClosePopup);
    }

    private void OnClickMakeRoomBtn()
    {
        // 인풋 필드 유효성 검사
        byte maxPlayerNum = byte.Parse(maxPlayerNumInputField.text);

        if (string.IsNullOrWhiteSpace(roomNameInputField.text))
        {
            CreateErrorPopup("Failed Make Room", "Room Name is Essential.");
            return;
        }

        if (2 > maxPlayerNum || 4 < maxPlayerNum)
        {
            CreateErrorPopup("Failed Make Room", "Player Num must be between 2 and 4");
            return;
        }

        NetworkManager.Instance.CreateRoom(roomNameInputField.text, maxPlayerNum);
        ClosePopup();
    }
}
