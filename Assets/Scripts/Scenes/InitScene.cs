using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    [SerializeField] private GameObject[] dontdestroyObjs;

    private void Start()
    {
        foreach (var obj in dontdestroyObjs)
        {
            DontDestroyOnLoad(obj);
        }

        LoadingScene.LoadScene("TitleScene");
    }
}
