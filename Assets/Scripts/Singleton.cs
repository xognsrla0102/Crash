using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<T>();

                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    Debug.Assert(false, "싱글턴 개체가 없습니다. InitScene에서 시작하세요.");
                }
            }
            return instance;
        }
    }
}
