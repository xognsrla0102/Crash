using UnityEngine;

public class InitScene : MonoBehaviour
{
    public static string loadSceneName = ESceneName.TITLE_SCENE;

    [SerializeField] private GameObject[] dontdestroyObjs;

    private void Start()
    {
        if (TestManager.deleteAllPrefs)
        {
            EncryptPlayerPrefs.DeleteAll();
        }

        foreach (var obj in dontdestroyObjs)
        {
            DontDestroyOnLoad(obj);
        }

        LoadingManager.LoadScene(loadSceneName);
    }
}
