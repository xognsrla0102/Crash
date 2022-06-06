using UnityEngine;
using TMPro;

public enum EPopupType
{
    OK_POPUP,
    YES_NO_POPUP,
    NUMS
}

public abstract class Popup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;

    public static void CreatePopup(string titleText, string bodyText, EPopupType popupType = EPopupType.OK_POPUP)
    {
        Popup obj;

        switch (popupType)
        {
            case EPopupType.OK_POPUP:
                obj = Resources.Load<OKPopup>("Prefabs/OKPopup");
                break;
            case EPopupType.YES_NO_POPUP:
                obj = Resources.Load<YesNoPopup>("Prefabs/YesNoPopup");
                break;
            default:
                Debug.Assert(false);
                obj = null;
                break;
        }

        Popup popup = Instantiate(obj, GameObject.Find("UI").transform);
        popup.InitPopup(titleText, bodyText);
    }

    public void InitPopup(string titleText, string bodyText)
    {
        this.titleText.text = titleText;
        this.bodyText.text = bodyText;
    }

    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}
