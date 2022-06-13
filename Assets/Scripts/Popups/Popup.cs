using System.Text;
using UnityEngine;
using TMPro;
using PlayFab;

public enum EPopupType
{
    OK_POPUP,
    YES_NO_POPUP,
    OPTION_POPUP,
    GAME_NAME_POPUP,
    NUMS
}

public abstract class Popup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;

    public static Popup CreatePopup(EPopupType popupType)
    {
        Popup obj;

        switch (popupType)
        {
            case EPopupType.OPTION_POPUP:
                obj = Resources.Load<OptionPopup>("Prefabs/OptionPopup");
                break;
            case EPopupType.GAME_NAME_POPUP:
                obj = Resources.Load<GameNamePopup>("Prefabs/GameNamePopup");
                break;
            default:
                Debug.Assert(false);
                obj = null;
                break;
        }

        return Instantiate(obj, GameObject.Find("UI").transform);
    }

    public static void CreateInfoPopup(string titleText, string bodyText, EPopupType popupType = EPopupType.OK_POPUP)
    {
        Popup obj;

        switch (popupType)
        {
            case EPopupType.OK_POPUP:     obj = Resources.Load<OKPopup>("Prefabs/OKPopup"); break;
            case EPopupType.YES_NO_POPUP: obj = Resources.Load<YesNoPopup>("Prefabs/YesNoPopup"); break;
            default: Debug.Assert(false); obj = null; break;
        }

        Popup popup = Instantiate(obj, GameObject.Find("UI").transform);
        popup.InitInfoPopup(titleText, bodyText);
    }

    public static void CreateInfoPopup(string titleText, PlayFabError error, EPopupType popupType = EPopupType.OK_POPUP)
    {
        string errorString = SetBodyTextPlayFabErrorString(error);
        CreateInfoPopup(titleText, errorString, popupType);
    }

    private static string SetBodyTextPlayFabErrorString(PlayFabError error)
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

        return $"Failed Reason\n{sb}";
    }

    public static Popup GetPopup(EPopupType popupType)
    {
        switch (popupType)
        {
            case EPopupType.OPTION_POPUP:
                return FindObjectOfType<OptionPopup>();
            default:
                Debug.Assert(false);
                break;
        }
        return null;
    }

    public static bool Exists(EPopupType popupType) => GetPopup(popupType) != null;

    public void InitInfoPopup(string titleText, string bodyText)
    {
        this.titleText.text = titleText;
        this.bodyText.text = bodyText;
    }

    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}
