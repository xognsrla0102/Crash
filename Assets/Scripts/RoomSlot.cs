using UnityEngine;
using Photon.Realtime;
using TMPro;

public class RoomSlot : MonoBehaviour
{
    [HideInInspector] public RoomInfo roomInfo;

    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI mapNameText;
    [SerializeField] private TextMeshProUGUI roomStateText;
    [SerializeField] private TextMeshProUGUI userCntText;

    private void Start()
    {
        
    }
}
