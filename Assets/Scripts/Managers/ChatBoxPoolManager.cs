using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class ChatBoxPoolManager : Singleton<ChatBoxPoolManager>
{
    [HideInInspector] public const int SHOW_CHAT_BOX_CNT = 20;

    // 만약 풀링 오브젝트를 다 쓸 경우, 추가로 풀링 오브젝트를 할당할 숫자, 나중에 필요해지면 쓸 것
    // [HideInInspector] public const int ADD_POOL_OBJ_CNT = 10;

    [SerializeField] private ChatBox chatBoxObj;

    private Queue<ChatBox> poolObjQueue = new Queue<ChatBox>();

    private Transform chatContent;
    private Transform ChatContent
    {
        get
        {
            if (chatContent == null)
            {
                chatContent = GameObject.Find("UI").transform.Find("ChatField")
                    .Find("ScrollView").Find("Viewport").Find("Content");
            }
            return chatContent;
        }
    }

    private void Awake()
    {
        AddPoolObj(SHOW_CHAT_BOX_CNT);
    }

    private void AddPoolObj(int addCnt = 1)
    {
        for (int i = 0; i < addCnt; i++)
        {
            poolObjQueue.Enqueue(CreatePoolObj());
        }
    }

    private ChatBox CreatePoolObj()
    {
        var obj = Instantiate(chatBoxObj, transform);
        obj.gameObject.SetActive(false);
        return obj;
    }

    public void Push()
    {
        ChatBox willPoolObj = ChatContent.GetChild(0).GetComponent<ChatBox>();

        poolObjQueue.Enqueue(willPoolObj);
        willPoolObj.transform.SetParent(transform);
        willPoolObj.gameObject.SetActive(false);
    }

    public ChatBox Pop()
    {
        // 풀링된 오브젝트를 다 쓴 경우에 Pop을 하는 경우는 버그
        Debug.Assert(poolObjQueue.Count != 0, "풀링 오브젝트를 전부 사용하였습니다.");

        ChatBox retObj = poolObjQueue.Dequeue();
        retObj.transform.SetParent(ChatContent);
        retObj.transform.localScale = Vector3.one;
        retObj.gameObject.SetActive(true);
        return retObj;
    }

    public void AddChatBox(string msg, Player user)
    {
        // 인 게임 중에는 무시
        if (MyRoomManager.roomState.Equals(SRoomState.IN_GAME))
        {
            return;
        }

        // 이미 채팅 슬롯이 보여줄 슬롯만큼 생성되었다면
        if (ChatContent.childCount == SHOW_CHAT_BOX_CNT)
        {
            // 가장 위에 있던 채팅 슬롯을 풀링 오브젝트에 다시 넣음
            Push();
        }

        ChatBox chatBox = Pop();
        chatBox.SetText(user == null ? msg : $"[{user.NickName}] : {msg}");

        if (user == null)
        {
            return;
        }

        // 현재 방에서 말한 유저의 슬롯에 있는 채팅 이펙트를 보여줌
        RoomScene roomScene = FindObjectOfType<RoomScene>();
        foreach (var userSlot in roomScene.userSlots)
        {
            if (userSlot.IsEmptySlot)
            {
                continue;
            }

            if (userSlot.userInfo.UserId == user.UserId)
            {
                userSlot.ShowChatEffect(msg);
                break;
            }
        }
    }

    public void ResetPoolingChatBox()
    {
        // 인 게임 중에는 무시
        if (MyRoomManager.roomState.Equals(SRoomState.IN_GAME))
        {
            return;
        }

        print("채팅 필드에 있던 채팅 박스 전부 다시 풀링해두고 로비로 이동");

        while (ChatContent.childCount > 0)
        {
            Push();
        }
    }
}
