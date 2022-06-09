using UnityEngine;
using UnityEngine.UI;


// 옵션 팝업을 따로 띄우고 그 클래스에서 토글과 슬라이더를 받아야 할 듯
// 버튼을 누르면 그냥 옵션 매니저에 있는 옵션 팝업을 생성하는 함수를 호출하게?

public class OptionManager : Singleton<OptionManager>
{
    private const float DEFAULT_BGM_VOLUME = 0.4f;
    private const float DEFAULT_SFX_VOLUME = 0.2f;

    private float bgmVolume;
    public float BgmVolume
    {
        get => bgmVolume;
        set
        {
            EncryptPlayerPrefs.SetFloat(PrefsKeys.BGM_VOLUME, value);
            SoundManager.Instance.BgmVolume = value;
            bgmVolume = value;
        }
    }

    private float sfxVolume;
    public float SfxVolume
    {
        
        get => sfxVolume;
        set
        {
            EncryptPlayerPrefs.SetFloat(PrefsKeys.SOUND_VOLUME, value);
            SoundManager.Instance.SfxVolume = value;
            sfxVolume = value;
        }
    }

    [HideInInspector] public bool isMuteBgm;
    [HideInInspector] public bool isMuteSfx;

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
        BgmVolume = EncryptPlayerPrefs.GetFloat(PrefsKeys.BGM_VOLUME, DEFAULT_BGM_VOLUME);
        SfxVolume = EncryptPlayerPrefs.GetFloat(PrefsKeys.SOUND_VOLUME, DEFAULT_SFX_VOLUME);

        bgmVolumeSlider.value = BgmVolume;
        sfxVolumeSlider.value = SfxVolume;

        muteBgmToggle.isOn = EncryptPlayerPrefs.GetBool(PrefsKeys.IS_BGM_MUTE);
        muteSfxToggle.isOn = EncryptPlayerPrefs.GetBool(PrefsKeys.IS_SFX_MUTE);
    }

    private void OnDestroy()
    {
        muteBgmToggle.onValueChanged.RemoveAllListeners();
        muteSfxToggle.onValueChanged.RemoveAllListeners();
    }

    private void OnClickMuteBgmToggle(bool isOn)
    {
        isMuteBgm = isOn;
    }

    private void OnClickMuteSfxToggle(bool isOn)
    {
        isMuteSfx = isOn;
    }

    private void OnValueChangedBgmSlider(float value)
    {
        BgmVolume = value;
    }

    private void OnValueChangedSfxSlider(float value)
    {
        SfxVolume = value;
    }
}
