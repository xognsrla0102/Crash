using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class CarTest : MonoBehaviourPunCallbacks
{
    public GameObject[] spawnPos;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("Prefabs/Player", spawnPos[Random.Range(0, spawnPos.Length)].transform.position, Quaternion.identity);
    }
}
