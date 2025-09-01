using UnityEngine;

public class PartsArm : MonoBehaviour
{
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Belt"))
        {
            rb.AddForce(other.transform.up * -0.19f, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndOfBelt"))
        {
            Destroy(gameObject);
        }
    }
}
