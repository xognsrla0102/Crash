using UnityEngine;
using UnityEngine.UI;

public class YesNoPopup : Popup
{
    [SerializeField] protected Button yesBtn;
    [SerializeField] protected Button noBtn;

    protected virtual void Start()
    {
        yesBtn.onClick.AddListener(ClosePopup);
        noBtn.onClick.AddListener(ClosePopup);
    }

    private void OnDestroy()
    {
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
    }
}
