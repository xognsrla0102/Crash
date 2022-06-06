using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private Button gameStartBtn;
    [SerializeField] private Button gameExitBtn;

    [SerializeField] private RectTransform titleText;

    private void Start()
    {
        gameStartBtn.onClick.AddListener(OnClickGameStartBtn);
        gameExitBtn.onClick.AddListener(OnClickGameExitBtn);

        titleText.DOAnchorPosX(50f, 3f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        titleText.DOAnchorPosY(260f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        SoundManager.Instance.PlayBGM(ESoundName.TITLE);
    }

    private void OnDestroy()
    {
        gameStartBtn.onClick.RemoveAllListeners();
        gameExitBtn.onClick.RemoveAllListeners();

        DOTween.KillAll();
    }

    private void OnClickGameStartBtn()
    {
        // NetworkManager.Instance.
    }

    private void OnClickGameExitBtn()
    {
        print("종료 버튼을 통한 게임 종료");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
