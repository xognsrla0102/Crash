using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class UserManager
{
    public static string userName;
    // 룸에서 쓰이는 유저 번호
    public static int myUserNum = -1;

    private static EUserColorType userColorType;
    public static EUserColorType UserColorType
    {
        get => userColorType;
        set { userColorType = value; }
    }

    public static void InitUserSlot()
    {
        InitUserColorType();
        myUserNum = PhotonNetwork.PlayerList.Length - 1;
    }

    private static void InitUserColorType()
    {
        // 어떤 유저 색깔이 있는지 체크
        bool[] checkUserColorType = new bool[4];
        Player[] otherUsers = PhotonNetwork.PlayerListOthers;
        for (int i = 0; i < otherUsers.Length; i++)
        {
            var userColorType = Utility.StringToEnum<EUserColorType>($"{otherUsers[i].CustomProperties[SPlayerPropertyKey.COLOR_TYPE]}");
            Debug.Log($"userColorType : {userColorType}");
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

        Debug.Assert(userColorType != EUserColorType.NONE, "유저 색상을 None으로 초기화할 수 없습니다.");

        // 방에 참가한 유저에 대해 컬러 속성을 저장함
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { SPlayerPropertyKey.COLOR_TYPE, $"{userColorType}" } });
    }

    public static void ClearUserSlot()
    {
        UserColorType = EUserColorType.NONE;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { SPlayerPropertyKey.COLOR_TYPE, $"{UserColorType}" } });

        myUserNum = -1;
    }
}
