using System.Collections.Generic;
using UnityEngine;

public class ChatBoxPoolManager : Singleton<ChatBoxPoolManager>
{
    [SerializeField] private int addPoolObjCnt;
    [SerializeField] private GameObject chatBoxObj;

    private Queue<GameObject> poolObjQueue = new Queue<GameObject>();

    private void Awake()
    {
        AddPoolObj(addPoolObjCnt);
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
        // 풀링된 오브젝트를 다 쓴 경우 풀링 오브젝트 더 생성
        if (poolObjQueue.Count == 0)
        {
            AddPoolObj(addPoolObjCnt);
        }

        GameObject retObj = poolObjQueue.Dequeue();
        retObj.transform.SetParent(parent);
        retObj.gameObject.SetActive(true);
        return retObj;
    }
}
