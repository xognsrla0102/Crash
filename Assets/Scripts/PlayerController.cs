using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float spd;
    [SerializeField] private float gravity;

    public ForceMode forceMode;
    private CharacterController characterController;
    private Rigidbody rigid;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(Vector3.up * 5, forceMode);
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v).normalized * Time.unscaledDeltaTime;
        transform.LookAt(transform.position + moveDir);
        // moveDir.y -= gravity * Time.unscaledDeltaTime;
        rigid.AddForce(moveDir);
    }
}
