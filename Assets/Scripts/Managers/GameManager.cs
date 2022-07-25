﻿using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Material[] skyboxMats;
    
    private CinemachineVirtualCamera playerCam;
    private Transform[] spawnPos;

    private void Start()
    {
        // 방장이 스카이박스 세팅
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SelectSkyBoxRPC", RpcTarget.All, Random.Range(0, skyboxMats.Length));
        }

        playerCam = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();

        Transform spawnPosTransform = GameObject.Find("Map").transform.Find("SpwnPos");
        spawnPos = new Transform[spawnPosTransform.childCount];
        for (int i = 0; i < spawnPos.Length; i++)
        {
            spawnPos[i] = spawnPosTransform.GetChild(i);
        }

        Player[] playerList = PhotonNetwork.PlayerList;
        int spawnPosIdx = playerList.Length - 2;
        int colorIdx = (int)UserManager.Instance.userColorType - 1;

        GameObject user = PhotonNetwork.Instantiate($"{SResourceLoadPath.PREFAB}Player",
               spawnPos[spawnPosIdx].GetChild(colorIdx).position,
               spawnPos[spawnPosIdx].GetChild(colorIdx).rotation);

        user.GetComponent<Rigidbody>().velocity = Vector3.zero;

        playerCam.Follow = user.transform;
        playerCam.LookAt = user.transform;
    }

    [PunRPC] private void SelectSkyBoxRPC(int skyboxIdx) => RenderSettings.skybox = skyboxMats[skyboxIdx];
}
