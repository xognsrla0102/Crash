using UnityEngine;
using UnityEngine.UI;

public class RoomScene : MonoBehaviour
{
    [SerializeField] private Button lobbyBtn;

    private void Start()
    {
        lobbyBtn.onClick.AddListener(OnClickLobbyBtn);
    }

    private void OnDestroy()
    {
        lobbyBtn.onClick.RemoveAllListeners();
    }

    private void OnClickLobbyBtn() => NetworkManager.Instance.LeaveRoom();

}
