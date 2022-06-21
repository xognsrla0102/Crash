using System.Collections;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    public static string loadSceneName = SSceneName.TITLE_SCENE;

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

        StartCoroutine(WaitForInitCoroutine());
    }

    private IEnumerator WaitForInitCoroutine()
    {
        // 2 프레임 대기
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        LoadingManager.LoadScene(loadSceneName);
    }
}
