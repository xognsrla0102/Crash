using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using DG.Tweening;

public class LoadingScene : MonoBehaviour
{
    // 로딩 텍스트 애니메이션 반복 주기
    private const float LOADING_TEXT_ANIMATION_TIME = 3f;
    // 로딩이 너무 빨리 끝나는 것을 방지하기 위한 패딩 시간
    private const float LOADING_PADDING_TIME = LOADING_TEXT_ANIMATION_TIME * 2f;
    // 밀리 초 단위를 초로 변환 시켜주는 매직넘버
    private const float MILLISEC_TO_SEC = 0.001f;

    [Header("로딩 상태를 보여주는 개체들")]
    public TextMeshProUGUI loadingText;
    public Transform loadingObjRoot;

    [Header("테스트 변수")]
    // 로드씬에서부터 시작하는 경우, 타이틀 씬으로 제대로 이동하는지 확인하는 변수
    public bool isStartInLoadScene;

    private static string loadSceneName;

    // 로딩 패팅 시간을 체크할 타이머
    private readonly Stopwatch stopWatch = new Stopwatch();

    public static void LoadScene(string sceneName)
    {
        loadSceneName = sceneName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
    }

    private void Start()
    {
        // 로드씬에서부터 시작하는 경우, 타이틀 씬으로 제대로 이동하는지 확인
        if (isStartInLoadScene)
        {
            loadSceneName = "TitleScene";
        }

        loadingObjRoot.DOLocalRotate(new Vector3(0, 0, 90), LOADING_TEXT_ANIMATION_TIME).SetEase(Ease.OutExpo).SetLoops(-1, LoopType.Incremental);
        loadingObjRoot.DOScale(new Vector3(1.3f, 1.3f, 1f), LOADING_TEXT_ANIMATION_TIME / 2f).SetEase(Ease.OutElastic).SetLoops(-1, LoopType.Yoyo);
        
        DOTween.To(x => loadingText.maxVisibleCharacters = (int)x, 0f, loadingText.text.Length + 1, LOADING_TEXT_ANIMATION_TIME).SetLoops(-1);
        loadingText.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), LOADING_TEXT_ANIMATION_TIME).SetEase(Ease.OutElastic).SetLoops(-1);

        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        // 로드할 씬을 비동기로 로드 시작
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(loadSceneName);

        // 로드할 씬이 준비된 경우 바로 이동하지 않고, 우리가 원하는 타임에 씬을 이동하도록 false로 만듬
        asyncOperation.allowSceneActivation = false;

        while (true)
        {
            // 90% 이상 로드시에
            if (asyncOperation.progress >= 0.9f)
            {
                // 타이머 시작
                if (stopWatch.IsRunning == false)
                {
                    stopWatch.Restart();
                }

                // 타이머가 패딩 시간만큼 지났다면
                if (stopWatch.ElapsedMilliseconds * MILLISEC_TO_SEC > LOADING_PADDING_TIME)
                {
                    // 타이머 종료
                    stopWatch.Stop();

                    // 트윈 전부 킬
                    DOTween.KillAll();

                    // 로드 어느정도 다 했으니 씬 로딩이 끝나면 씬 이동 되도록 코드 변경
                    asyncOperation.allowSceneActivation = true;
                    yield break;
                }
            }
            yield return null;
        }
    }
}
