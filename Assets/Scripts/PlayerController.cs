using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float spd;

    private Rigidbody rigid;
    private Vector3 nowPos;
    private bool isCrash;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            nowPos = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        transform.Find(photonView.IsMine ? "CarBlue" : "CarRed").gameObject.SetActive(true);
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
        else if ((transform.position - nowPos).sqrMagnitude >= 50)
        {
            transform.position = nowPos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, nowPos, Time.deltaTime * 10);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(CrashCoroutine(collision.gameObject.GetComponent<Rigidbody>().velocity));
        }
    }

    private IEnumerator CrashCoroutine(Vector3 enemyVelocity)
    {
        print("충돌");
        isCrash = true;
        Vector3 oldVelocity = rigid.velocity;
        rigid.velocity = Vector3.zero;

        yield return new WaitForSeconds(0.5f);

        print("충돌해제 및 충격파");
        isCrash = false;
        Vector3 totalVelocity = enemyVelocity + oldVelocity;
        rigid.AddForce(totalVelocity * 3f, ForceMode.Impulse);
    }
}
