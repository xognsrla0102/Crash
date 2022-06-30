using System.Collections.Generic;
using UnityEngine;

public class ChatBoxPoolManager : Singleton<ChatBoxPoolManager>
{
    [HideInInspector] public const int SHOW_CHAT_BOX_CNT = 20;

    // 만약 풀링 오브젝트를 다 쓸 경우, 추가로 풀링 오브젝트를 할당할 숫자, 나중에 필요해지면 쓸 것
    // [HideInInspector] public const int ADD_POOL_OBJ_CNT = 10;

    [SerializeField] private ChatBox chatBoxObj;

    private Queue<ChatBox> poolObjQueue = new Queue<ChatBox>();

    private void Awake()
    {
        // queue가 빈 상태에서 pop되는 경우가 있어서 임시로 하나 더 생성함
        AddPoolObj(SHOW_CHAT_BOX_CNT + 1);
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

    public void Push(ChatBox willPoolObj)
    {
        poolObjQueue.Enqueue(willPoolObj);
        willPoolObj.transform.SetParent(transform);
        willPoolObj.gameObject.SetActive(false);
    }

    public ChatBox Pop(Transform parent = null)
    {
        // 풀링된 오브젝트를 다 쓴 경우에 Pop을 하는 경우는 버그
        if (poolObjQueue.Count == 0)
        {
            Debug.Assert(false, "풀링 오브젝트를 전부 사용하였습니다.");
        }

        ChatBox retObj = poolObjQueue.Dequeue();
        retObj.transform.SetParent(parent);
        retObj.transform.localScale = Vector3.one;
        retObj.gameObject.SetActive(true);
        return retObj;
    }
}
