using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : Singleton<TestManager>
{
    [Header("테스트 변수")]
    // 로드씬에서부터 시작하는 경우, 타이틀 씬으로 제대로 이동하는지 확인하는 변수
    public bool goTitleSceneWhenStartAtLoadScene;
    // 모든 PlayerPrefs 키 삭제하는 변수
    public bool deleteAllPrefs;
}
