using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using DG.Tweening;
using Photon.Pun;

public class LoadingManager : MonoBehaviour
{
    // 로딩 텍스트 애니메이션 반복 주기
    private const float LOADING_TEXT_ANIMATION_TIME = 3f;
    // 로딩이 너무 빨리 끝나는 것을 방지하기 위한 패딩 시간
    private const float LOADING_PADDING_TIME = 0.5f;
    // 밀리 초 단위를 초로 변환 시켜주는 매직넘버
    private const float MILLISEC_TO_SEC = 0.001f;

    private static string loadSceneName;

    [Header("로딩 상태를 보여주는 개체들")]
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Transform loadingObjRoot;

    // 로딩 패딩 시간을 체크할 타이머
    private readonly Stopwatch stopWatch = new Stopwatch();

    public static void LoadScene(string sceneName)
    {
        loadSceneName = sceneName;
        SceneManager.LoadScene(SSceneName.LOADING_SCENE);
    }

    private void Start()
    {
        // 왼쪽으로 90도씩 돌아가는 트윈과 1.3배 스케일 변형하는 트윈
        loadingObjRoot.DOLocalRotate(new Vector3(0f, 0f, 90f), LOADING_TEXT_ANIMATION_TIME).SetEase(Ease.OutExpo).SetLoops(-1, LoopType.Incremental);
        loadingObjRoot.DOScale(new Vector3(1.3f, 1.3f, 1f), LOADING_TEXT_ANIMATION_TIME / 2f).SetEase(Ease.OutElastic).SetLoops(-1, LoopType.Yoyo);
        
        // 한 글자씩 나오는 트윈과 1.2배 스케일 변형하는 트윈 
        DOTween.To(x => loadingText.maxVisibleCharacters = (int)x, 0f, loadingText.text.Length + 1, LOADING_TEXT_ANIMATION_TIME).SetLoops(-1);
        loadingText.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), LOADING_TEXT_ANIMATION_TIME).SetEase(Ease.OutElastic).SetLoops(-1);

        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(loadSceneName);

        // 로드할 씬이 준비된 경우 바로 이동하지 않고, 우리가 원하는 타임에 씬을 이동하도록 false로 만듬
        asyncOperation.allowSceneActivation = false;

        // 타이머 시작
        stopWatch.Restart();

        while (true)
        {
            // 90% 이상 로드시에
            if (asyncOperation.progress >= 0.9f)
            {
                // 타이머가 패딩 시간만큼 지났다면
                if (stopWatch.ElapsedMilliseconds * MILLISEC_TO_SEC > LOADING_PADDING_TIME)
                {
                    stopWatch.Stop();
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
