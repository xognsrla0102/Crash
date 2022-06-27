using System;
using UnityEngine;
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
        // 아직 내 유저 색깔이 정해지지 않았을 경우
        if (userColorType == EUserColorType.NONE)
        {
            // 어떤 유저 색깔이 있는지 체크
            bool[] checkUserColorType = new bool[4];
            Player[] otherUsers = PhotonNetwork.PlayerListOthers;
            for (int i = 0; i < otherUsers.Length; i++)
            {
                var userColorType = (EUserColorType)Enum.Parse(typeof(EUserColorType), $"{otherUsers[i].CustomProperties[SPlayerPropertyKey.COLOR_TYPE]}");
                checkUserColorType[(int)userColorType - 1] = true;
            }

            // 유저 색깔 없는 것 중 처음 것을 내 색깔로 결정
            for (int i = 0; i < checkUserColorType.Length; i++)
            {
                if (checkUserColorType[i] == false)
                {
                    userColorType = (EUserColorType)(i + 1);
                    break;
                }
            }

            Debug.Assert(userColorType != EUserColorType.NONE, "유저 색상이 None입니다.");

            // 방에 참가한 유저에 대해 컬러 속성을 저장함
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { SPlayerPropertyKey.COLOR_TYPE, $"{userColorType}" } });
        }

        roomInfo = PhotonNetwork.CurrentRoom;

        Hashtable roomProperty = roomInfo.CustomProperties;

        roomName = $"{roomProperty[SRoomPropertyKey.ROOM_NAME]}";
        roomState= $"{roomProperty[SRoomPropertyKey.ROOM_STATE]}";

        masterName = $"{roomProperty[SRoomPropertyKey.MASTER_CLIENT]}";
        mapName = $"{roomProperty[SRoomPropertyKey.MAP_NAME]}";

        nowPlayerNum = roomInfo.PlayerCount;
        maxPlayerNum = roomInfo.MaxPlayers;
    }

    public static void ClearRoomManager()
    {
        entryRoomState = EEntryRoomState.NONE;

        userColorType = EUserColorType.NONE;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { SPlayerPropertyKey.COLOR_TYPE, $"{userColorType}" } });

        roomInfo = null;

        roomName =
        roomState =
        masterName =
        mapName = string.Empty;

        nowPlayerNum =
        maxPlayerNum = 0;
    }
}
