using Photon.Pun;

public static class UserManager
{
    public static string userName;

    public static int slotUserNum = -1;

    private static EUserColorType userColorType;

    public static void InitUserManager()
    {
        // 최초 방 생성 시 (방장이 방 초기화 할 때)
        if (PhotonNetwork.IsMasterClient)
        {
            userColorType = EUserColorType.RED;
            NetworkManager.Instance.SetPlayerProperties(PhotonNetwork.LocalPlayer, SPlayerPropertyKey.COLOR_TYPE, userColorType);

            slotUserNum = 0;
            NetworkManager.Instance.SetPlayerProperties(PhotonNetwork.LocalPlayer, SPlayerPropertyKey.SLOT_USER_NUM, slotUserNum);
            return;
        }

        userColorType = Utility.StringToEnum<EUserColorType>($"{PhotonNetwork.LocalPlayer.CustomProperties[SPlayerPropertyKey.COLOR_TYPE]}");
        slotUserNum = (int)PhotonNetwork.LocalPlayer.CustomProperties[SPlayerPropertyKey.SLOT_USER_NUM];
    }

    public static void ClearUserManager()
    {
        userColorType = EUserColorType.NONE;
        NetworkManager.Instance.SetPlayerProperties(PhotonNetwork.LocalPlayer, SPlayerPropertyKey.COLOR_TYPE, $"{userColorType}");

        slotUserNum = -1;
        NetworkManager.Instance.SetPlayerProperties(PhotonNetwork.LocalPlayer, SPlayerPropertyKey.SLOT_USER_NUM, slotUserNum);
    }
}
