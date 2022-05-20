using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Test : MonoBehaviourPunCallbacks
{
    public InputField nickNameInputField;

    public void OnClickConnect()
    {
        print($"닉네임 : {nickNameInputField.text}으로 연결");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.NickName = nickNameInputField.text;
        print("연결 완료");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print($"연결 끊김 : {cause}");
    }
}
