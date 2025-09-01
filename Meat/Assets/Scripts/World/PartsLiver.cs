using UnityEngine;

public class PartsLiver : MonoBehaviour
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
            rb.AddForce(other.transform.up * -0.4f, ForceMode.Force);
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
