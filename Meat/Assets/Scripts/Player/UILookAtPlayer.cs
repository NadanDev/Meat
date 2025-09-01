using UnityEngine;

public class UILookAtPlayer : MonoBehaviour
{
    [SerializeField] Camera cam;
    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
