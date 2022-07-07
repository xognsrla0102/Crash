using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionPopup : Popup
{
    [SerializeField] private Button closeBtn;

    [SerializeField] private Toggle muteBgmToggle;
    [SerializeField] private Toggle muteSfxToggle;

    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle isFullScreenToggle;

    private void Start()
    {
        closeBtn.onClick.AddListener(ClosePopup);

        muteBgmToggle.onValueChanged.AddListener(OnClickMuteBgmToggle);
        muteSfxToggle.onValueChanged.AddListener(OnClickMuteSfxToggle);

        bgmVolumeSlider.onValueChanged.AddListener(OnValueChangedBgmSlider);
        sfxVolumeSlider.onValueChanged.AddListener(OnValueChangedSfxSlider);

        resolutionDropdown.onValueChanged.AddListener(OnValueChangedResolutionDropdown);
        isFullScreenToggle.onValueChanged.AddListener(OnValueChangedIsFullScreenToggle);

        LoadOption();
    }

    private void LoadOption()
    {
        // 슬라이더 값 갱신
        bgmVolumeSlider.value = OptionManager.Instance.BgmVolume;
        sfxVolumeSlider.value = OptionManager.Instance.SfxVolume;

        // 무음 여부 로드
        muteBgmToggle.isOn = OptionManager.Instance.IsMuteBgm;
        muteSfxToggle.isOn = OptionManager.Instance.IsMuteSfx;

        // 해상도 로드
        resolutionDropdown.value = (int)OptionManager.Instance.ResolutionType;
        isFullScreenToggle.isOn = OptionManager.Instance.IsFullScreen;
    }

    private void OnDestroy()
    {
        closeBtn.onClick.RemoveAllListeners();

        muteBgmToggle.onValueChanged.RemoveAllListeners();
        muteSfxToggle.onValueChanged.RemoveAllListeners();

        bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();

        resolutionDropdown.onValueChanged.RemoveAllListeners();
        isFullScreenToggle.onValueChanged.RemoveAllListeners();
    }
    
    private void OnClickMuteBgmToggle(bool isMuteBgm) => OptionManager.Instance.IsMuteBgm = isMuteBgm;
    private void OnClickMuteSfxToggle(bool isMuteSfx) => OptionManager.Instance.IsMuteSfx = isMuteSfx;
    private void OnValueChangedBgmSlider(float bgmValue) => OptionManager.Instance.BgmVolume = bgmValue;
    private void OnValueChangedSfxSlider(float sfxValue) => OptionManager.Instance.SfxVolume = sfxValue;
    private void OnValueChangedResolutionDropdown(int resolutionType) => OptionManager.Instance.ResolutionType = (EResolutionType)resolutionType;
    private void OnValueChangedIsFullScreenToggle(bool isFullScreen) => OptionManager.Instance.IsFullScreen = isFullScreen;
}
