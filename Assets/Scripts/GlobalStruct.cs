public struct ESceneName
{
    public const string INIT_SCENE = "InitScene";
    public const string LOADING_SCENE = "LoadingScene";
    public const string TITLE_SCENE = "TitleScene";
    public const string LOBBY_SCENE = "LobbyScene";
}

// PlayerPrefs에 사용되는 키들
public struct PrefsKeys
{
    public const string IS_AUTO_LOGIN = "IsAutoLogin";
    public const string USER_NAME = "UserName";
    public const string PW = "Pw";

    public const string BGM_VOLUME = "BgmVolume";
    public const string SOUND_VOLUME = "SoundVolume";

    public const string IS_BGM_MUTE = "IsBgmMute";
    public const string IS_SFX_MUTE = "IsSfxMute";
}

public struct EBgmName
{
    public const string TITLE_BGM = "Title";
    public const string LOBBY_BGM = "Lobby";
}

public struct ESfxName
{
    public const string BUTTON_OVER_SFX = "BtnOver";
    public const string BUTTON_CLICK_SFX = "BtnClick";
}