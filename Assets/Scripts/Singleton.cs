using UnityEngine;
using UnityEngine.SceneManagement;

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
                    Scene scene = SceneManager.GetActiveScene();
                    if (scene.name.Equals(ESceneName.INIT_SCENE))
                    {
                        Debug.Assert(false, $"싱글턴 {typeof(T)} 개체가 없습니다.");
                    }
                    else
                    {
                        Debug.Assert(false, "InitScene에서 시작하세요.");
                    }
                }
            }
            return instance;
        }
    }
}
