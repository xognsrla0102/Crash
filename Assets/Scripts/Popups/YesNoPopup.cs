using UnityEngine;
using UnityEngine.UI;

public class YesNoPopup : Popup
{
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    protected virtual void Start()
    {
        yesBtn.onClick.AddListener(ClosePopup);
        noBtn.onClick.AddListener(ClosePopup);
    }

    protected virtual void OnDestroy()
    {
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
    }

    public void SetYesBtnAction(UnityEngine.Events.UnityAction action)
    {
        // 기존 기본 리스너 삭제
        yesBtn.onClick.RemoveAllListeners();
        yesBtn.onClick.AddListener(action);
    }

    public void SetNoBtnAction(UnityEngine.Events.UnityAction action)
    {
        // 기존 기본 리스너 삭제
        noBtn.onClick.RemoveAllListeners();
        noBtn.onClick.AddListener(action);
    }
}
