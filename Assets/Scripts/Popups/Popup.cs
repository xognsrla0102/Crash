﻿using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;

public enum EPopupType
{
    OK_POPUP,
    YES_NO_POPUP,
    OPTION_POPUP,
    NUMS
}

public abstract class Popup : MonoBehaviour
{
    [SerializeField] private Button closeBtn;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;

    public static void CreatePopup(EPopupType popupType)
    {
        string titleText;
        Popup obj;

        switch (popupType)
        {
            case EPopupType.OPTION_POPUP:
                obj = Resources.Load<OptionPopup>("Prefabs/OptionPopup");
                titleText = "게임 설정";
                break;
            default:
                Debug.Assert(false);
                obj = null;
                titleText = string.Empty;
                break;
        }

        Popup popup = Instantiate(obj, GameObject.Find("UI").transform);
        popup.InitPopup(titleText);
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

        return $"실패 원인\n{sb}";
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

    public void InitPopup(string titleText)
    {
        this.titleText.text = titleText;
    }

    public void InitInfoPopup(string titleText, string bodyText)
    {
        this.titleText.text = titleText;
        this.bodyText.text = bodyText;
    }

    protected virtual void Start()
    {
        closeBtn.onClick.AddListener(ClosePopup);
    }

    protected virtual void OnDestroy()
    {
        closeBtn.onClick.RemoveAllListeners();
    }

    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}
