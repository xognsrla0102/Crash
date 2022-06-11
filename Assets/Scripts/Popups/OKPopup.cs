using UnityEngine;
using UnityEngine.UI;

public class OKPopup : Popup
{
    [SerializeField] private Button okBtn;

    protected override void Start()
    {
        okBtn.onClick.AddListener(ClosePopup);
    }

    protected override void OnDestroy()
    {
        okBtn.onClick.RemoveAllListeners();        
    }

    public void SetOKBtnAction(UnityEngine.Events.UnityAction action)
    {
        // 기존 기본 리스너 삭제
        okBtn.onClick.RemoveAllListeners();
        okBtn.onClick.AddListener(action);
    }
}
