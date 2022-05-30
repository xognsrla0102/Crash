using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    private void Start()
    {
        // 전역 매니저 DonDestroy 개체로 만듬
        DontDestroyOnLoad(NetworkManager.Instance.gameObject);
        DontDestroyOnLoad(SoundManager.Instance.gameObject);
    }
}
