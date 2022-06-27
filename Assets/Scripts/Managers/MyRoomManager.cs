using Photon;
using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum EEntryRoomState
{
    NONE,
    CREATE_ROOM,
    JOIN_ROOM,
    JOIN_RANDOM_ROOM
}

public static class MyRoomManager
{
    public static EEntryRoomState entryRoomState;
    public static EUserColorType userColorType;

    public static RoomInfo roomInfo;

    public static string roomName;
    public static string roomState;

    public static string masterName;
    public static string mapName;

    public static int nowPlayerNum;
    public static byte maxPlayerNum;

    public static void SetRoomManager()
    {
        roomInfo = PhotonNetwork.CurrentRoom;

        userColorType = (EUserColorType)(PhotonNetwork.PlayerList.Length - 1);

        // 방에 참가한 유저에 대해 컬러 속성을 저장함
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { SPlayerPropertyKey.COLOR_TYPE, $"{userColorType}" } });

        Hashtable roomProperty = roomInfo.CustomProperties;

        roomName = $"{roomProperty[SRoomPropertyKey.ROOM_NAME]}";
        roomState= $"{roomProperty[SRoomPropertyKey.ROOM_STATE]}";

        masterName = $"{roomProperty[SRoomPropertyKey.MASTER_CLIENT]}";
        mapName = $"{roomProperty[SRoomPropertyKey.MAP_NAME]}";

        nowPlayerNum = roomInfo.PlayerCount;
        maxPlayerNum = roomInfo.MaxPlayers;
    }
}
