using UnityEngine;
using UnityEngine.UI;

public class RoomOptionPopup : YesNoPopup
{
    [SerializeField] private InputField roomNameInputField;

    private InputFieldUtility inputFieldUtility;

    protected override void Start()
    {
        inputFieldUtility = GetComponent<InputFieldUtility>();
        inputFieldUtility.EnterAction = OnClickUpdateRoomOptionBtn;

        roomNameInputField.ActivateInputField();

        yesBtn.onClick.AddListener(OnClickUpdateRoomOptionBtn);
        noBtn.onClick.AddListener(ClosePopup);
    }

    private void OnClickUpdateRoomOptionBtn()
    {
        #region 인풋 필드 유효성 검사
        if (string.IsNullOrWhiteSpace(roomNameInputField.text))
        {
            CreateErrorPopup("Failed Update Room Option", "Room Name is Essential.");
            return;
        }
        #endregion

        // MyRoomManager.roomName = roomNameInputField.text;

        ClosePopup();
    }
}
