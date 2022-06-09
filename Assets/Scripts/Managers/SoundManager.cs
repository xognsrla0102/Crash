using System.Collections.Generic;
using UnityEngine;

public enum ESoundType
{
    BGM,
    SFX,
    NUMS
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource[] sources = new AudioSource[(int)ESoundType.NUMS];
    // 인스펙터에서 넣을 소리들
    [SerializeField] private AudioClip[] sounds;

    // 실제로 쓰일 소리들
    private Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    public float BgmVolume
    {
        set => sources[(int)ESoundType.BGM].volume = value;
    }

    public float SfxVolume
    {
        set => sources[(int)ESoundType.BGM].volume = value;
    }

    private void Start()
    {
        foreach (var sound in sounds)
        {
            if (clips.ContainsKey(sound.name) == false)
            {
                clips.Add(sound.name, sound);
            }
        }
    }

    public void PlayBGM(string name)
    {
        if (OptionManager.Instance.isMuteBgm)
        {
            return;
        }

        AudioSource bgmSource = sources[(int)ESoundType.BGM];

        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }

        bgmSource.clip = clips[name];
        bgmSource.Play();
    }

    public void PlaySND(string name)
    {
        if (OptionManager.Instance.isMuteSfx)
        {
            return;
        }

        AudioSource sfxSource = sources[(int)ESoundType.SFX];
        sfxSource.clip = clips[name];
        sfxSource.PlayOneShot(sfxSource.clip);
    }
}
