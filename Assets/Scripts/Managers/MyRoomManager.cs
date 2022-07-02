﻿using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class MyRoomManager
{
    public static RoomInfo roomInfo;

    public static string roomName;
    public static string roomState;

    public static string masterName;
    public static string mapName;

    public static int nowPlayerNum;
    public static byte maxPlayerNum;

    private static EUserColorType UserColorType
    {
        set
        {
            UserManager.UserColorType = value;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { SPlayerPropertyKey.COLOR_TYPE, $"{UserManager.UserColorType}" } });
        }
    }

    public static void SetRoomManager()
    {
        roomInfo = PhotonNetwork.CurrentRoom;

        Hashtable roomProperty = roomInfo.CustomProperties;

        roomName = $"{roomProperty[SRoomPropertyKey.ROOM_NAME]}";
        roomState= $"{roomProperty[SRoomPropertyKey.ROOM_STATE]}";

        masterName = $"{roomProperty[SRoomPropertyKey.MASTER_CLIENT]}";
        mapName = $"{roomProperty[SRoomPropertyKey.MAP_NAME]}";

        nowPlayerNum = roomInfo.PlayerCount;
        maxPlayerNum = roomInfo.MaxPlayers;

        // 내 색상이 초기화 되지 않았을 경우
        if (UserManager.UserColorType == EUserColorType.NONE)
        {
            UserManager.InitUserColorType();
        }
    }

    public static void ClearRoomManager()
    {
        roomInfo = null;

        roomName =
        roomState =
        masterName =
        mapName = string.Empty;

        nowPlayerNum =
        maxPlayerNum = 0;

        UserColorType = EUserColorType.NONE;
    }
}
