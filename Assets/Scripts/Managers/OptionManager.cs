using UnityEngine;
using UnityEngine.SceneManagement;

public enum EResolutionType
{
    _3840X2160, //4K
    _2560X1440, //QHD
    _1920X1080, //FHD
    _1600X900, //HD+
    _1280X720, //HD
    NUMS
}

public class OptionManager : Singleton<OptionManager>
{
    public const float DEFAULT_BGM_VOLUME = 0.4f;
    public const float DEFAULT_SFX_VOLUME = 0.2f;

    public const EResolutionType DEFAULT_RESOLUTION = EResolutionType._1920X1080;

    private float bgmVolume;
    public float BgmVolume
    {
        get => bgmVolume;
        set
        {
            if (bgmVolume == value)
            {
                return;
            }

            EncryptPlayerPrefs.SetFloat(SPrefsKey.BGM_VOLUME, value);
            SoundManager.Instance.BgmSource.volume = value;
            bgmVolume = value;
        }
    }

    private float sfxVolume;
    public float SfxVolume
    {
        get => sfxVolume;
        set
        {
            if (sfxVolume == value)
            {
                return;
            }

            EncryptPlayerPrefs.SetFloat(SPrefsKey.SOUND_VOLUME, value);
            SoundManager.Instance.SfxSource.volume = value;
            sfxVolume = value;
        }
    }

    private bool isMuteBgm;
    public bool IsMuteBgm
    {
        get => isMuteBgm;
        set
        {
            EncryptPlayerPrefs.SetBool(SPrefsKey.IS_BGM_MUTE, value);
            SoundManager.Instance.BgmSource.mute = value;
            isMuteBgm = value;
        }
    }

    private bool isMuteSfx;
    public bool IsMuteSfx
    {
        get => isMuteSfx;
        set
        {
            EncryptPlayerPrefs.SetBool(SPrefsKey.IS_SFX_MUTE, value);
            SoundManager.Instance.SfxSource.mute = value;
            isMuteSfx = value;
        }
    }

    public bool IsFullScreen
    {
        get => Screen.fullScreen;
        set
        {
            if (Screen.fullScreen == value)
            {
                return;
            }

            bool isFullScreen = value;
            EncryptPlayerPrefs.SetBool(SPrefsKey.IS_FULLSCREEN, isFullScreen);
            Screen.SetResolution(Screen.width, Screen.height, isFullScreen);
        }
    }

    private int[] widths = new int[] { 3840, 2560, 1920, 1600, 1280 };
    private int[] heights = new int[] { 2160, 1440, 1080, 900, 720 };
    private EResolutionType resolutionType;
    public EResolutionType ResolutionType
    {
        get => resolutionType;
        set
        {
            if (resolutionType == value)
            {
                return;
            }

            resolutionType = value;
            EncryptPlayerPrefs.SetInt(SPrefsKey.RESOLUTION_TYPE, (int)resolutionType);
            Screen.SetResolution(widths[(int)resolutionType], heights[(int)resolutionType], IsFullScreen);
        }
    }

    private void Start()
    {
        // 볼륨 값들 로드
        BgmVolume = EncryptPlayerPrefs.GetFloat(SPrefsKey.BGM_VOLUME, DEFAULT_BGM_VOLUME);
        SfxVolume = EncryptPlayerPrefs.GetFloat(SPrefsKey.SOUND_VOLUME, DEFAULT_SFX_VOLUME);

        // 무음 여부 로드
        IsMuteBgm = EncryptPlayerPrefs.GetBool(SPrefsKey.IS_BGM_MUTE);
        IsMuteSfx = EncryptPlayerPrefs.GetBool(SPrefsKey.IS_SFX_MUTE);

        // 해상도 로드
        ResolutionType = (EResolutionType)EncryptPlayerPrefs.GetInt(SPrefsKey.RESOLUTION_TYPE, (int)DEFAULT_RESOLUTION);
        Screen.fullScreen = EncryptPlayerPrefs.GetBool(SPrefsKey.IS_FULLSCREEN);
    }

    private void Update()
    {
        // ESC 키 누르면 옵션 열림
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickOptionBtn();
        }
    }

    public void OnClickOptionBtn()
    {
        // 현재 씬이 Init, Loading 씬이라면 무시
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name.Equals(SSceneName.INIT_SCENE) || scene.name.Equals(SSceneName.LOADING_SCENE))
        {
            return;
        }

        // 이미 게임 설정 창이 켜져있다면 끔
        if (Popup.Exists(EPopupType.OPTION_POPUP))
        {
            Popup.GetPopup(EPopupType.OPTION_POPUP).ClosePopup();
            return;
        }

        // 없다면 띄움
        Popup.CreateSpecialPopup(EPopupType.OPTION_POPUP);
    }
}
