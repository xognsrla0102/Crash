using UnityEngine;
using UnityEngine.UI;

public class OptionPopup : Popup
{
    [SerializeField] private Toggle muteBgmToggle;
    [SerializeField] private Toggle muteSfxToggle;

    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    protected override void Start()
    {
        base.Start();

        muteBgmToggle.onValueChanged.AddListener(OnClickMuteBgmToggle);
        muteSfxToggle.onValueChanged.AddListener(OnClickMuteSfxToggle);

        bgmVolumeSlider.onValueChanged.AddListener(OnValueChangedBgmSlider);
        sfxVolumeSlider.onValueChanged.AddListener(OnValueChangedSfxSlider);

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
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        muteBgmToggle.onValueChanged.RemoveAllListeners();
        muteSfxToggle.onValueChanged.RemoveAllListeners();

        bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();
    }
    
    private void OnClickMuteBgmToggle(bool isOn) => OptionManager.Instance.IsMuteBgm = isOn;
    private void OnClickMuteSfxToggle(bool isOn) => OptionManager.Instance.IsMuteSfx = isOn;
    private void OnValueChangedBgmSlider(float value) => OptionManager.Instance.BgmVolume = value;
    private void OnValueChangedSfxSlider(float value) => OptionManager.Instance.SfxVolume = value;
}
