using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomScene : MonoBehaviour
{
    [SerializeField] private Button lobbyBtn;
    [SerializeField] private InputField chatInputField;

    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI roomNameText;

    [SerializeField] private Button gameStartBtn;
    [SerializeField] private Button gameReadyBtn;

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

        // 채팅 인풋필드 활성화
        chatInputField.ActivateInputField();

        userNameText.text = UserManager.userName;
        roomNameText.text = MyRoomManager.roomName;

        UpdateRoom();
    }

    private void OnDestroy()
    {
        lobbyBtn.onClick.RemoveAllListeners();

        gameStartBtn.onClick.RemoveAllListeners();
        gameReadyBtn.onClick.RemoveAllListeners();
    }

    // 방 요소 갱신
    public void UpdateRoom()
    {
        gameStartBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        gameReadyBtn.gameObject.SetActive(PhotonNetwork.IsMasterClient == false);

        roomNameText.text = MyRoomManager.roomName;
    }

    private void OnClickLobbyBtn() => NetworkManager.Instance.LeaveRoom();
}
