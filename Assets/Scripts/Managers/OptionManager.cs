﻿using UnityEngine;
using UnityEngine.SceneManagement;

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

            EncryptPlayerPrefs.SetFloat(PrefsKeys.SOUND_VOLUME, value);
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
            EncryptPlayerPrefs.SetBool(PrefsKeys.IS_BGM_MUTE, value);
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
            EncryptPlayerPrefs.SetBool(PrefsKeys.IS_SFX_MUTE, value);
            SoundManager.Instance.SfxSource.mute = value;
            isMuteSfx = value;
        }
    }

    private void Start()
    {
        // 볼륨 값들 로드
        BgmVolume = EncryptPlayerPrefs.GetFloat(PrefsKeys.BGM_VOLUME, DEFAULT_BGM_VOLUME);
        SfxVolume = EncryptPlayerPrefs.GetFloat(PrefsKeys.SOUND_VOLUME, DEFAULT_SFX_VOLUME);

        // 무음 여부 로드
        IsMuteBgm = EncryptPlayerPrefs.GetBool(PrefsKeys.IS_BGM_MUTE);
        IsMuteSfx = EncryptPlayerPrefs.GetBool(PrefsKeys.IS_SFX_MUTE);
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
        if (scene.name.Equals(ESceneName.INIT_SCENE) || scene.name.Equals(ESceneName.LOADING_SCENE))
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
        Popup.CreatePopup(EPopupType.OPTION_POPUP);
    }
}
