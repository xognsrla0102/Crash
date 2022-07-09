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
        if (photonView.IsMine == false)
        {
            // 상대방이 충돌 감지할 때 기준
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
