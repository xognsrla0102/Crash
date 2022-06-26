using System.Collections.Generic;
using UnityEngine;

public class ChatBoxPoolManager : Singleton<ChatBoxPoolManager>
{
    [HideInInspector] public const int SHOW_CHAT_BOX_CNT = 20;

    // 만약 풀링 오브젝트를 다 쓸 경우, 추가로 풀링 오브젝트를 할당할 숫자, 나중에 필요해지면 쓸 것
    // [HideInInspector] public const int ADD_POOL_OBJ_CNT = 10;

    [SerializeField] private GameObject chatBoxObj;

    private Queue<GameObject> poolObjQueue = new Queue<GameObject>();

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

    private GameObject CreatePoolObj()
    {
        var obj = Instantiate(chatBoxObj, transform);
        obj.SetActive(false);
        return obj;
    }

    public void Push(GameObject willPoolObj)
    {
        poolObjQueue.Enqueue(willPoolObj);
        willPoolObj.transform.SetParent(transform);
        willPoolObj.SetActive(false);
    }

    public GameObject Pop(Transform parent = null)
    {
        // 풀링된 오브젝트를 다 쓴 경우에 Pop을 하는 경우는 버그
        if (poolObjQueue.Count == 0)
        {
            Debug.Assert(false, "풀링 오브젝트를 전부 사용하였습니다.");
        }

        GameObject retObj = poolObjQueue.Dequeue();
        retObj.transform.SetParent(parent);
        retObj.transform.localScale = Vector3.one;
        retObj.gameObject.SetActive(true);
        return retObj;
    }
}
