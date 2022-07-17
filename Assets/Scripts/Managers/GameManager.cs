using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Material[] skyboxMats;

    private Transform[] spawnPos;

    private void Start()
    {
        photonView.RPC("SelectSkyBoxRPC", RpcTarget.All, Random.Range(0, skyboxMats.Length));

        Transform map = GameObject.Find("Map").transform;
        spawnPos = new Transform[map.childCount];
        for (int i = 0; i < spawnPos.Length; i++)
        {
            spawnPos[i] = map.GetChild(i);
        }

        Player[] playerList = PhotonNetwork.PlayerList;
        int spawnPosIdx = playerList.Length - 2;
        int colorIdx = (int)UserManager.userColorType - 1;

        GameObject user = PhotonNetwork.Instantiate($"{SResourceLoadPath.PREFAB}Player",
               spawnPos[spawnPosIdx].GetChild(colorIdx).position,
               spawnPos[spawnPosIdx].GetChild(colorIdx).rotation);

        user.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    [PunRPC] private void SelectSkyBoxRPC(int skyboxIdx) => RenderSettings.skybox = skyboxMats[skyboxIdx];
}
