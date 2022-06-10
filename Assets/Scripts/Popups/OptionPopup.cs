using UnityEngine;
using UnityEngine.UI;

public class OptionPopup : Popup
{
    [SerializeField] private Toggle muteBgmToggle;
    [SerializeField] private Toggle muteSfxToggle;

    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Start()
    {
        

        muteBgmToggle.onValueChanged.AddListener(OnClickMuteBgmToggle);
        muteSfxToggle.onValueChanged.AddListener(OnClickMuteSfxToggle);

        bgmVolumeSlider.onValueChanged.AddListener(OnValueChangedBgmSlider);
        sfxVolumeSlider.onValueChanged.AddListener(OnValueChangedSfxSlider);

        LoadOption();
    }

    private void LoadOption()
    {
        // 볼륨 값들 로드
        OptionManager.Instance.BgmVolume = EncryptPlayerPrefs.GetFloat(PrefsKeys.BGM_VOLUME, OptionManager.DEFAULT_BGM_VOLUME);
        OptionManager.Instance.SfxVolume = EncryptPlayerPrefs.GetFloat(PrefsKeys.SOUND_VOLUME, OptionManager.DEFAULT_SFX_VOLUME);

        // 슬라이더 값 갱신
        bgmVolumeSlider.value = OptionManager.Instance.BgmVolume;
        sfxVolumeSlider.value = OptionManager.Instance.SfxVolume;

        // 무음 여부 로드
        muteBgmToggle.isOn = EncryptPlayerPrefs.GetBool(PrefsKeys.IS_BGM_MUTE);
        muteSfxToggle.isOn = EncryptPlayerPrefs.GetBool(PrefsKeys.IS_SFX_MUTE);
    }

    private void OnDestroy()
    {
        muteBgmToggle.onValueChanged.RemoveAllListeners();
        muteSfxToggle.onValueChanged.RemoveAllListeners();

        bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();
    }
    
    private void OnClickMuteBgmToggle(bool isOn) => OptionManager.Instance.isMuteBgm = isOn;
    private void OnClickMuteSfxToggle(bool isOn) => OptionManager.Instance.isMuteSfx = isOn;
    private void OnValueChangedBgmSlider(float value) => OptionManager.Instance.BgmVolume = value;
    private void OnValueChangedSfxSlider(float value) => OptionManager.Instance.SfxVolume = value;
}
