using UnityEngine;
using UnityEngine.UI;

// 버튼을 누르면 그냥 옵션 매니저에 있는 옵션 팝업을 생성하는 함수를 호출하게?
// 왜냐면 옵션 매니저에서 버튼과 슬라이더 개체를 받지 못하고 있으니... 그냥 씬마다 다 복붙해서 일일이 넣어줘야하나?

public class OptionManager : Singleton<OptionManager>
{
    public const float DEFAULT_BGM_VOLUME = 0.4f;
    public const float DEFAULT_SFX_VOLUME = 0.2f;

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

            EncryptPlayerPrefs.SetFloat(PrefsKeys.BGM_VOLUME, value);
            SoundManager.Instance.BgmSourceVolume = value;
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

            EncryptPlayerPrefs.SetFloat(PrefsKeys.SOUND_VOLUME, value);
            SoundManager.Instance.SfxSourceVolume = value;
            sfxVolume = value;
        }
    }

    [HideInInspector] public bool isMuteBgm;
    [HideInInspector] public bool isMuteSfx;
}
