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

    [Header("유저 슬롯")]
    [SerializeField] private UserSlot[] userSlots;

    private InputFieldUtility inputFieldUtility;

    private void Start()
    {
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

    public void InitRoomScene()
    {
        MyRoomManager.SetRoomManager();

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
        roomOptionBtn.onClick.AddListener(OnClickRoomOptionBtn);

        // 채팅 인풋필드 활성화
        chatInputField.ActivateInputField();

        roomNameText.text = MyRoomManager.roomName;

        UpdateRoom();
    }

    private void OnDestroy()
    {
        lobbyBtn.onClick.RemoveAllListeners();

        gameStartBtn.onClick.RemoveAllListeners();
        gameReadyBtn.onClick.RemoveAllListeners();

        changeMapBtn.onClick.RemoveAllListeners();

        roomOptionBtn.onClick.RemoveAllListeners();
    }

    // 방 요소 갱신
    public void UpdateRoom()
    {
        #region 방장인지 아닌지에 따라 보이는 버튼 구분
        gameStartBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        gameReadyBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient == false);

        changeMapBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        roomOptionBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        #endregion

        roomNameText.text = MyRoomManager.roomName;

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {

        }
    }

    private void OnClickLobbyBtn() => NetworkManager.Instance.LeaveRoom();

    private void OnClickRoomOptionBtn() => Popup.CreateSpecialPopup(EPopupType.ROOM_OPTION_POPUP);
}
