using Photon.Pun;
using UnityEngine;

public class UserManager : Singleton<UserManager>
{
    [HideInInspector] public string userName;

    [HideInInspector] public string profileImageUrl;
    [HideInInspector] public Texture profileImage;

    [HideInInspector] public int slotUserNum = -1;
    [HideInInspector] public EUserColorType userColorType;

    public void InitUserManager()
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

    public void ClearUserManager()
    {
        userColorType = EUserColorType.NONE;
        NetworkManager.Instance.SetPlayerProperties(PhotonNetwork.LocalPlayer, SPlayerPropertyKey.COLOR_TYPE, $"{userColorType}");

        slotUserNum = -1;
        NetworkManager.Instance.SetPlayerProperties(PhotonNetwork.LocalPlayer, SPlayerPropertyKey.SLOT_USER_NUM, slotUserNum);
    }
}
