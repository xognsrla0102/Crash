using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private Button titleBtn;

    private void Start()
    {
        titleBtn.onClick.AddListener(OnClickTitleBtn);
    }

    private void OnDestroy()
    {
        titleBtn.onClick.RemoveAllListeners();
    }

    private void OnClickTitleBtn()
    {
        NetworkManager.Instance.LeaveLobby();
    }
}
