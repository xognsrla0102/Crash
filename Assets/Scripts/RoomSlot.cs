using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

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
            roomNameText.text = roomInfo.Name;
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
