using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class IngameScene : MonoBehaviour
{
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate($"{SResourceLoadPath.PREFAB}GameManager", Vector3.zero, Quaternion.identity);
        }
    }
}
