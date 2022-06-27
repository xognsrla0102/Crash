using UnityEngine;
using UnityEngine.UI;

public class OKPopup : Popup
{
    [SerializeField] protected Button okBtn;

    private void Start()
    {
        okBtn.onClick.AddListener(ClosePopup);
    }

    private void OnDestroy()
    {
        okBtn.onClick.RemoveAllListeners();        
    }

    public void SetOKBtnAction(UnityEngine.Events.UnityAction action)
    {
        // 기존 기본 리스너 삭제
        okBtn.onClick.RemoveAllListeners();
        okBtn.onClick.AddListener(action + ClosePopup);
    }
}
