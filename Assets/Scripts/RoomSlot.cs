using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomSlot : MonoBehaviour
{
    [HideInInspector] public Button slotBtn;
    [HideInInspector] public RoomInfo roomInfo;

    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI mapNameText;
    [SerializeField] private TextMeshProUGUI roomStateText;
    [SerializeField] private TextMeshProUGUI playerNumText;

    public void InitSlot()
    {
        slotBtn = GetComponent<Button>();
    }

    private void OnDestroy()
    {
        slotBtn.onClick.RemoveAllListeners();
    }

    public void SetSlot(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;

        if (roomInfo != null)
        {
            Hashtable roomProperty = roomInfo.CustomProperties;

            roomNameText.text = roomInfo.Name;
            mapNameText.text = $"{roomProperty[SRoomPropertyKey.MAP_NAME]}";
            roomStateText.text = $"{roomProperty[SRoomPropertyKey.ROOM_STATE]}";
            playerNumText.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        }
        else
        {
            roomNameText.text = "None";

            mapNameText.text =
            roomStateText.text =
            playerNumText.text = string.Empty;
        }
    }
}
