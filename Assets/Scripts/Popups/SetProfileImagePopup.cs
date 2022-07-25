using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetProfileImagePopup : YesNoPopup
{
    [SerializeField] private RawImage profileImage;
    [SerializeField] private InputField imageUrlInputField;

    [SerializeField] private Button loadBtn;

    private InputFieldUtility inputFieldUtility;

    protected override void Start()
    {
        inputFieldUtility = GetComponent<InputFieldUtility>();
        inputFieldUtility.EnterAction = OnClickSettingBtn;

        imageUrlInputField.ActivateInputField();

        loadBtn.onClick.AddListener(OnClickLoadBtn);

        yesBtn.onClick.AddListener(OnClickSettingBtn);
        noBtn.onClick.AddListener(ClosePopup);

        if (UserManager.Instance.profileImage != null)
        {
            imageUrlInputField.text = UserManager.Instance.profileImageUrl;
            profileImage.texture = UserManager.Instance.profileImage;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        loadBtn.onClick.RemoveAllListeners();
    }

    private void OnClickLoadBtn()
    {
        if (string.IsNullOrWhiteSpace(imageUrlInputField.text))
        {
            CreateErrorPopup("Failed Load Profile Image", "Image Url cannot be empty.");
            return;
        }

        StartCoroutine(SetProfileImageCoroutine(imageUrlInputField.text));
    }

    private IEnumerator SetProfileImageCoroutine(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        NetworkManager.Instance.CanvasGroup.interactable = false;

        yield return request.SendWebRequest();
        NetworkManager.Instance.CanvasGroup.interactable = true;

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogWarning(request.error);
            CreateErrorPopup("Failed Load Profile Image", request.error);
            yield break;
        }

        profileImage.texture = (request.downloadHandler as DownloadHandlerTexture).texture;
    }

    private void OnClickSettingBtn()
    {
        if (UserManager.Instance.profileImage == profileImage.texture)
        {
            return;
        }

        UserManager.Instance.profileImageUrl = imageUrlInputField.text;
        EncryptPlayerPrefs.SetString(SPrefsKey.PROFILE_IMAGE_URL, imageUrlInputField.text);

        UserManager.Instance.profileImage = profileImage.texture;

        CreateNormalPopup("Success Setting Profile Image", "Success Setting Profile Image");
        ClosePopup();
    }
}
