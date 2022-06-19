using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class LobbyScene : MonoBehaviour
{
    [HideInInspector] public List<RoomInfo> roomInfos;
    [SerializeField] private List<RoomSlot> roomSlots;

    [SerializeField] private Button titleBtn;
    [SerializeField] private Button makeRoomBtn;
    [SerializeField] private Button joinRandomRoomBtn;
    [SerializeField] private Button prevPageBtn;
    [SerializeField] private Button nextPageBtn;

    private const int MIN_PAGE_NUM = 0;
    private int nowPageNum;
    private int maxPageNum;

    private void Start()
    {
        titleBtn.onClick.AddListener(OnClickTitleBtn);
        makeRoomBtn.onClick.AddListener(OnClickMakeRoomBtn);
        joinRandomRoomBtn.onClick.AddListener(OnClickJoinRandomRoomBtn);
        prevPageBtn.onClick.AddListener(OnClickPrevPageBtn);
        nextPageBtn.onClick.AddListener(OnClickNextPageBtn);

        for (int slotIdx = 1; slotIdx <= roomSlots.Count; slotIdx++)
        {
            // 람다 함수라서 매개변수 전달 이상하게 되는 것 방지
            int param = slotIdx;
            roomSlots[slotIdx].GetComponent<Button>().onClick.AddListener(() => OnClickRoomBtn(param));
        }

        SoundManager.Instance.PlayBGM(EBgmName.LOBBY_BGM);
    }

    private void OnDestroy()
    {
        titleBtn.onClick.RemoveAllListeners();
        makeRoomBtn.onClick.RemoveAllListeners();
        joinRandomRoomBtn.onClick.RemoveAllListeners();
        prevPageBtn.onClick.RemoveAllListeners();
        nextPageBtn.onClick.RemoveAllListeners();

        foreach (var slot in roomSlots)
        {
            slot.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void OnClickTitleBtn() => NetworkManager.Instance.LeaveLobby();

    private void OnClickMakeRoomBtn() => Popup.CreatePopup(EPopupType.MAKE_ROOM_POPUP);

    private void OnClickJoinRandomRoomBtn() => NetworkManager.Instance.JoinRandomRoom();

    private void OnClickRoomBtn(int roomNum)
    {
        

    }

    private void OnClickPrevPageBtn()
    {
        nowPageNum--;

        // 최소 페이지가 되면 이전 페이지 못 가게 설정
        if (nowPageNum == MIN_PAGE_NUM)
        {
            prevPageBtn.interactable = false;
        }
        // 현재 페이지가 최대 페이지였다가 이전 페이지로 이동한 상태라면 다음 페이지로 갈 수 있게 설정
        else if (nowPageNum == maxPageNum - 1)
        {
            nextPageBtn.interactable = true;
        }
    }

    private void OnClickNextPageBtn()
    {
        nowPageNum++;

        // 최대 페이지가 되면 다음 페이지 못 가게 설정
        if (nowPageNum == maxPageNum)
        {
            nextPageBtn.interactable = false;
        }
        // 현재 페이지가 최소 페이지였다가 다음 페이지로 이동한 상태라면 이전 페이지로 갈 수 있게 설정
        else if (nowPageNum == MIN_PAGE_NUM + 1)
        {
            prevPageBtn.interactable = true;
        }
    }

    public void UpdateRoomList()
    {

    }
}
