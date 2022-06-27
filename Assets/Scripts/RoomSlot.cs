using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomSlot : MonoBehaviour
{
    public TextMeshProUGUI playerNumText;
    public TextMeshProUGUI roomStateText;

    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI masterUserNameText;
    [SerializeField] private TextMeshProUGUI mapNameText;

    [HideInInspector] public Button slotBtn;
    [HideInInspector] public RoomInfo roomInfo;

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

            roomNameText.text = $"Room Name [{roomProperty[SRoomPropertyKey.ROOM_NAME]}]";
            roomStateText.text = $"Room State [<color=#ff00ffff>{roomProperty[SRoomPropertyKey.ROOM_STATE]}</color>]";

            masterUserNameText.text = $"Master User(<color=red>\"{roomProperty[SRoomPropertyKey.MASTER_CLIENT]}\"</color>)";

            mapNameText.text = $"MAP [<color=blue>\"{roomProperty[SRoomPropertyKey.MAP_NAME]}\"</color>]";
            playerNumText.text = roomInfo.PlayerCount == roomInfo.MaxPlayers ? "<color=red>FULL</color>" : $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        }
        else
        {
            roomNameText.text = "None";

            roomStateText.text =
            masterUserNameText.text = 
            mapNameText.text =
            playerNumText.text = string.Empty;
        }
    }
}
