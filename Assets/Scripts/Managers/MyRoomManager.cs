using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class MyRoomManager
{
    public static RoomInfo RoomInfo => PhotonNetwork.CurrentRoom;

    public static string roomName;
    public static string roomState;

    public static string mapName;

    public static int nowPlayerNum;
    public static int maxPlayerNum;

    public static void SetRoomManager()
    {
        Hashtable roomProperty = RoomInfo.CustomProperties;

        roomName = RoomInfo.Name;
        roomState= $"{roomProperty[SRoomPropertyKey.ROOM_STATE]}";

        mapName = $"{roomProperty[SRoomPropertyKey.MAP_NAME]}";

        nowPlayerNum = RoomInfo.PlayerCount;
        maxPlayerNum = RoomInfo.MaxPlayers;
    }
}
