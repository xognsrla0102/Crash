using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private RawImage profileImage;
    [SerializeField] private float spd;

    private float MAX_SPD = 30;
    private InGameUserSlot userSlot;
    private Rigidbody rb;

    private bool isGameOver;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        int userColorNum = (int)Utility.StringToEnum<EUserColorType>($"{photonView.Owner.CustomProperties[SPlayerPropertyKey.COLOR_TYPE]}");
        transform.GetChild(userColorNum - 1).gameObject.SetActive(true);
        userSlot = GameObject.Find("UI").transform.Find($"UserSlot{userColorNum}").GetComponent<InGameUserSlot>();

        name = photonView.Owner.NickName;
        userNameText.text = name;
        userSlot.InitUserSlot(name);
    }

    private void FixedUpdate()
    {
        //if (photonView.IsMine && isGameOver == false)
        //{
            Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            transform.LookAt(transform.position + moveDir);

            rb.AddForce(moveDir * spd, ForceMode.Acceleration);

            if (rb.velocity.magnitude > MAX_SPD)
            {
                rb.velocity = rb.velocity.normalized * MAX_SPD;
            }
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        // OnCollision되는 시기는 컴퓨터 마다 동기화 딜레이 때문에 다 다르므로 마스터가 한 번에 물리 처리 해줌
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // 내가 누군가와 충돌한 경우
            if (photonView.IsMine)
            {
                // 튕김 이동 처리.. Observe 되있어서 RPC 필요없음
                Crash(collision.contacts[0].normal);
            }
            // 남이 누군가와 충돌한 경우
            else
            {
                // 마스터에서 직접 이동시키면 IsMine이 true가 아니므로 IsMine이 true인 컴퓨터의 개체의 값으로 Observe되서 튕김 이동 처리 무시됨
                // 그러므로 RPC 함수를 통해 튕김 이동 처리
                photonView.RPC("CrashRPC", RpcTarget.All, collision.contacts[0].normal);
            }
        }
    }

    [PunRPC] private void CrashRPC(Vector3 contactNormalPoint) => Crash(contactNormalPoint);

    private void Crash(Vector3 contactNormalPoint)
    {
        rb.AddForce(contactNormalPoint * 100);
        rb.AddForce(Vector3.up * 80);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;

        }

        if (other.CompareTag("DeadLine"))
        {
            photonView.RPC("GameOverRPC", RpcTarget.All);
        }
    }

    [PunRPC] private void GameOverRPC()
    {
        isGameOver = true;
        userSlot.GameOver();
    }
}