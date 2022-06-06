using UnityEngine;

public static class TestManager
{
    // 모든 PlayerPrefs 키 삭제하는 변수
    public static bool deleteAllPrefs;
    // 싱글턴 개체가 없으면 잠깐 InitScene 갔다가 돌아오는 변수
    public static bool isNullSingletonObjGoInitScene = true;
}
