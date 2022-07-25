using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class LobbyScene : MonoBehaviour
{
    [HideInInspector] public List<RoomInfo> roomInfos = new List<RoomInfo>();
    [SerializeField] private List<RoomSlot> roomSlots;

    [SerializeField] private Button titleBtn;
    [SerializeField] private Button makeRoomBtn;
    [SerializeField] private Button joinRandomRoomBtn;
    [SerializeField] private Button setProfileImageBtn;
    [SerializeField] private Button changeGameNameBtn;
    [SerializeField] private Button prevPageBtn;
    [SerializeField] private Button nextPageBtn;

    private const int MIN_PAGE_NUM = 1;
    private int nowPageNum = MIN_PAGE_NUM;
    private int maxPageNum;

    private void Start()
    {
        NetworkManager.Instance.JoinLobby();

        titleBtn.onClick.AddListener(OnClickTitleBtn);
        makeRoomBtn.onClick.AddListener(OnClickMakeRoomBtn);
        joinRandomRoomBtn.onClick.AddListener(OnClickJoinRandomRoomBtn);
        setProfileImageBtn.onClick.AddListener(OnClickSetProfileImageBtn);
        changeGameNameBtn.onClick.AddListener(OnClickChangeGameNameBtn);
        prevPageBtn.onClick.AddListener(OnClickPrevPageBtn);
        nextPageBtn.onClick.AddListener(OnClickNextPageBtn);

        for (int slotIdx = 0; slotIdx < roomSlots.Count; slotIdx++)
        {
            // 람다 함수라서 매개변수 전달 이상하게 되는 것 방지
            int param = slotIdx;
            roomSlots[slotIdx].InitSlot();
            roomSlots[slotIdx].slotBtn.onClick.AddListener(() => OnClickRoomSlotBtn(param));
        }

        SoundManager.Instance.PlayBGM(SBgmName.LOBBY_BGM);
    }

    private void OnDestroy()
    {
        titleBtn.onClick.RemoveAllListeners();
        makeRoomBtn.onClick.RemoveAllListeners();
        joinRandomRoomBtn.onClick.RemoveAllListeners();
        setProfileImageBtn.onClick.RemoveAllListeners();
        changeGameNameBtn.onClick.RemoveAllListeners();
        prevPageBtn.onClick.RemoveAllListeners();
        nextPageBtn.onClick.RemoveAllListeners();

        foreach (var slot in roomSlots)
        {
            slot.slotBtn.onClick.RemoveAllListeners();
        }
    }

    private void OnClickTitleBtn() => NetworkManager.Instance.LeaveLobby();

    private void OnClickMakeRoomBtn() => Popup.CreateSpecialPopup(EPopupType.MAKE_ROOM_POPUP);

    private void OnClickJoinRandomRoomBtn() => NetworkManager.Instance.JoinRandomRoom();

    private void OnClickSetProfileImageBtn() => Popup.CreateSpecialPopup(EPopupType.SET_PROFILE_IMAGE_POPUP);
    private void OnClickChangeGameNameBtn() => Popup.CreateSpecialPopup(EPopupType.CHANGE_GAME_NAME_POPUP);
    private void OnClickRoomSlotBtn(int slotIdx)
    {
        #region 해당 슬롯의 방 정보 유효 검사
        RoomSlot roomSlot = roomSlots[slotIdx];
        if (roomSlot.playerNumText.text.Contains("FULL"))
        {
            Popup.CreateErrorPopup("Failed Join Room", "The Room is FULL");
            print("방 유저 가득 차서 참가 실패");
            return;
        }

        if (roomSlot.roomStateText.text.Contains(SRoomState.IN_GAME))
        {
            Popup.CreateErrorPopup("Failed Join Room", "The Room is Playing Game now.");
            print("게임 진행 중이라 참가 실패");
            return;
        }
        #endregion

        NetworkManager.Instance.JoinRoom(roomSlot.roomInfo.Name);
    }

    private void OnClickPrevPageBtn()
    {
        nowPageNum--;

        // 페이지가 바뀌었으므로 슬롯도 갱신
        UpdateRoomSlot();
    }

    private void OnClickNextPageBtn()
    {
        nowPageNum++;

        // 페이지가 바뀌었으므로 슬롯도 갱신
        UpdateRoomSlot();
    }

    public void UpdateRoomSlot()
    {
        maxPageNum = roomInfos.Count / roomSlots.Count;
        if (roomInfos.Count % roomSlots.Count != 0)
        {
            maxPageNum++;
        }

        // 룸이 업데이트되면서 현재 페이지가 최대 페이지보다 커질 경우 최대를 넘지 않도록 갱신
        if (maxPageNum != 0)
        {
            nowPageNum = Mathf.Min(nowPageNum, maxPageNum);
        }

        prevPageBtn.interactable = nowPageNum > MIN_PAGE_NUM;
        nextPageBtn.interactable = nowPageNum < maxPageNum;

        int startRoomInfoIdxPerPage = roomSlots.Count * (nowPageNum - 1);
        for (int slotIdx = 0; slotIdx < roomSlots.Count; slotIdx++)
        {
            int nowRoomIdx = startRoomInfoIdxPerPage + slotIdx;
            bool existRoomInfo = nowRoomIdx < roomInfos.Count;

            roomSlots[slotIdx].slotBtn.interactable = existRoomInfo;
            roomSlots[slotIdx].SetSlot(existRoomInfo ? roomInfos[nowRoomIdx] : null);
        }
    }
}
