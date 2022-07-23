using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private void Update()
    {
        gameObject.transform.LookAt(Camera.main.transform);
    }
}
