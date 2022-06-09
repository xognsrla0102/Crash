using System.Collections;
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

    // 볼륨 값들
    private float bgmVolume;
    private float sfxVolume;

    private void Start()
    {
        foreach (var sound in sounds)
        {
            if (clips.ContainsKey(sound.name) == false)
            {
                clips.Add(sound.name, sound);
            }
        }

        bgmVolume = OptionManager.Instance.BgmVolume;
        sfxVolume = OptionManager.Instance.SfxVolume;

        sources[(int)ESoundType.BGM].volume = bgmVolume;
        sources[(int)ESoundType.SFX].volume = sfxVolume;
    }

    public void PlayBGM(string name)
    {
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
        AudioSource sfxSource = sources[(int)ESoundType.SFX];
        sfxSource.clip = clips[name];
        sfxSource.PlayOneShot(sfxSource.clip);
    }
}
