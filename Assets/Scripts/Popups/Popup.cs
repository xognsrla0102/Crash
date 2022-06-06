using System.Text;
using UnityEngine;
using TMPro;
using PlayFab;

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

    public static void CreateErrorPopup(string titleText, PlayFabError error, EPopupType popupType = EPopupType.OK_POPUP)
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
        popup.InitErrorPopup(titleText, error);
    }

    public void InitPopup(string titleText, string bodyText)
    {
        this.titleText.text = titleText;
        this.bodyText.text = bodyText;
    }

    public void InitErrorPopup(string titleText, PlayFabError error)
    {
        this.titleText.text = titleText;
        bodyText.text = SetBodyTextPlayFabErrorString(error);
    }

    public void ClosePopup()
    {
        Destroy(gameObject);
    }

    private string SetBodyTextPlayFabErrorString(PlayFabError error)
    {
        StringBuilder sb = new StringBuilder(100);

        if (error.ErrorDetails != null)
        {
            int i = 1;
            foreach (var errorDetail in error.ErrorDetails)
            {
                sb.Append($"{i}. [{errorDetail.Key}] ");
                foreach (var errorValue in errorDetail.Value)
                {
                    sb.AppendLine($"\"{errorValue}\"");
                }
                i++;
            }
        }
        else
        {
            sb.AppendLine(error.ErrorMessage);
        }

        return $"실패 원인\n{sb}";
    }
}
