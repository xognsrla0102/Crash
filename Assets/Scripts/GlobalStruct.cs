public struct SSceneName
{
    public const string INIT_SCENE = "InitScene";
    public const string LOADING_SCENE = "LoadingScene";
    public const string TITLE_SCENE = "TitleScene";
    public const string LOBBY_SCENE = "LobbyScene";
    public const string ROOM_SCENE = "RoomScene";
    public const string INGAME_SCENE = "IngameScene";
    public const string CAR_TEST = "CarTest";
}

// PlayerPrefs에 사용되는 키들
public struct SPrefsKey
{
    public const string IS_AUTO_LOGIN = "IsAutoLogin";
    public const string USER_NAME = "UserName";
    public const string PW = "Pw";

    public const string BGM_VOLUME = "BgmVolume";
    public const string SOUND_VOLUME = "SoundVolume";

    public const string IS_BGM_MUTE = "IsBgmMute";
    public const string IS_SFX_MUTE = "IsSfxMute";

    public const string RESOLUTION_TYPE = "ResolutionType";
    public const string IS_FULLSCREEN = "IsFullScreen";
}

public struct SBgmName
{
    public const string TITLE_BGM = "Title";
    public const string LOBBY_BGM = "Lobby";
}

public struct SSfxName
{
    public const string BUTTON_OVER_SFX = "BtnOver";
    public const string BUTTON_CLICK_SFX = "BtnClick";
}

public struct SMapName
{
    public const string STADIUM = "No.6 Stadium";
}

public struct SRoomState
{
    public const string PREPARING_GAME = "Preparing for the game";
    public const string IN_GAME = "InGame";
}

// 방 만들 때 사용되는 룸 속성 키들
public struct SRoomPropertyKey
{
    public const string ROOM_NAME = "RoomName";
    public const string MASTER_CLIENT = "MasterClient";
    public const string MAP_NAME = "MapName";
    public const string ROOM_STATE = "RoomState";
    public const string LOCKED_SLOT_NUMS = "LockedSlotNums";
    public const string READY_SLOT_NUMS = "ReadySlotNums";
}

// 방에 참가한 유저에 대한 속성 키들
public struct SPlayerPropertyKey
{
    public const string COLOR_TYPE = "ColorType";
    public const string SLOT_USER_NUM = "SlotUserNum";
}

// 리소스 로드 시 사용되는 경로
public struct SResourceLoadPath
{
    public const string POPUP = "Prefabs/Popups/";
    public const string IMAGE = "Images/";
}