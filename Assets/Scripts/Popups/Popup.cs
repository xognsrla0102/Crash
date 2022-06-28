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
    MAKE_ROOM_POPUP,
    ROOM_OPTION_POPUP,
    NUMS
}

public abstract class Popup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;

    [HideInInspector] public bool isNormalPopup;

    #region 팝업 생성
    public static Popup CreateSpecialPopup(EPopupType popupType)
    {
        Popup obj;

        switch (popupType)
        {
            case EPopupType.OPTION_POPUP:
                obj = Resources.Load<OptionPopup>($"{SResourceLoadPath.POPUP}OptionPopup");
                break;
            case EPopupType.GAME_NAME_POPUP:
                obj = Resources.Load<GameNamePopup>($"{SResourceLoadPath.POPUP}GameNamePopup");
                break;
            case EPopupType.MAKE_ROOM_POPUP:
                obj = Resources.Load<MakeRoomPopup>($"{SResourceLoadPath.POPUP}MakeRoomPopup");
                break;
            case EPopupType.ROOM_OPTION_POPUP:
                obj = Resources.Load<RoomOptionPopup>($"{SResourceLoadPath.POPUP}RoomOptionPopup");
                break;
            default:
                Debug.Assert(false);
                obj = null;
                break;
        }

        return Instantiate(obj, GameObject.Find("UI").transform);
    }

    public static Popup CreateNormalPopup(string titleText, string bodyText, EPopupType popupType = EPopupType.OK_POPUP)
    {
        Popup obj;

        switch (popupType)
        {
            case EPopupType.OK_POPUP:
                obj = Resources.Load<OKPopup>($"{SResourceLoadPath.POPUP}OKPopup");
                break;
            case EPopupType.YES_NO_POPUP:
                obj = Resources.Load<YesNoPopup>($"{SResourceLoadPath.POPUP}YesNoPopup");
                break;
            default:
                Debug.Assert(false);
                obj = null;
                break;
        }

        Popup popup = Instantiate(obj, GameObject.Find("UI").transform);
        popup.InitNormalPopup(titleText, bodyText);
        return popup;
    }

    public void InitNormalPopup(string titleText, string bodyText)
    {
        this.titleText.text = titleText;
        this.bodyText.text = bodyText;
        isNormalPopup = true;
    }

    public static Popup CreateErrorPopup(string titleText, string bodyText, EPopupType popupType = EPopupType.OK_POPUP)
        => CreateNormalPopup(titleText, bodyText, popupType);

    public static Popup CreateErrorPopup(string titleText, PlayFabError error, EPopupType popupType = EPopupType.OK_POPUP)
        => CreateErrorPopup(titleText, SetBodyTextPlayFabErrorString(error), popupType);

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
    #endregion

    #region 팝업 관련 유틸리티 기능
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
    
    public void ClosePopup() => Destroy(gameObject);
    #endregion
}
