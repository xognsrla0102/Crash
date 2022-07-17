using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private float spd;

    private Rigidbody rigid;
    private Rigidbody collisionObjRigidbody;

    private bool isCrash;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        transform.Find(photonView.IsMine ? "CarBlue" : "CarRed").gameObject.SetActive(true);
        gameObject.name = photonView.IsMine ? "CarBlue" : "CarRed";
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (isCrash)
            {
                return;
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 moveDir = new Vector3(h, 0, v).normalized;
            transform.LookAt(transform.position + moveDir);
            rigid.AddForce(moveDir * spd, ForceMode.Acceleration);
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                CarTest carTest = FindObjectOfType<CarTest>();
                transform.position = carTest.spawnPos[Random.Range(0, carTest.spawnPos.Length)].transform.position;
                rigid.velocity = Vector3.zero;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 방장 PC 기준으로 충돌 검사
        if (PhotonNetwork.IsMasterClient)
        {
            // 이 플레이어 개체가 내 것이 아니면서
            if (photonView.IsMine == false)
            {
                // 충돌체가 플레이어고, 충돌체가 내가 조작하는 플레이어라면
                if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    rigid.AddExplosionForce(50, collision.contacts[0].point, 5);
                    rigid.AddForce(Vector3.up * 5);
                    collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(50, collision.contacts[0].point, 5);
                    collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 5);
                }
            }
        }
    }
}
