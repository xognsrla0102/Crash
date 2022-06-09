using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : Singleton<OptionManager>
{
    public float BgmVolume
    {
        get
        {
            if (BgmVolume != 0) ;
            return EncryptPlayerPrefs.GetFloat(PrefsKeys.BGM_VOLUME, 0.1f);
        }
    }
    public float SfxVolume => EncryptPlayerPrefs.GetFloat(PrefsKeys.SOUND_VOLUME, 0.1f);

    private void Start()
    {
        
    }
}
