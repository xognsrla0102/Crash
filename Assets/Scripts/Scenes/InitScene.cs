using UnityEngine;

public class InitScene : MonoBehaviour
{
    [SerializeField] private GameObject[] dontdestroyObjs;

    private void Start()
    {
        if (TestManager.Instance.deleteAllPrefs)
        {
            EncryptPlayerPrefs.DeleteAll();
        }

        foreach (var obj in dontdestroyObjs)
        {
            DontDestroyOnLoad(obj);
        }

        LoadingManager.LoadScene("TitleScene");
    }
}
