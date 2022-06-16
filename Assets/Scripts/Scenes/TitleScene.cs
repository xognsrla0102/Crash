using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private Button lobbyBtn;
    [SerializeField] private Button gameExitBtn;

    [SerializeField] private RectTransform titleText;

    private void Start()
    {
        lobbyBtn.onClick.AddListener(OnClickLobbyBtn);
        gameExitBtn.onClick.AddListener(OnClickGameExitBtn);

        SoundManager.Instance.PlayBGM(EBgmName.TITLE_BGM);
    }

    private void OnDestroy()
    {
        lobbyBtn.onClick.RemoveAllListeners();
        gameExitBtn.onClick.RemoveAllListeners();
    }

    private void OnClickLobbyBtn()
    {
        // 서버 연결
        NetworkManager.Instance.ConnectMasterServer();
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
