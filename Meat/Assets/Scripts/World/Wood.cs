using UnityEngine;

public class Wood : MonoBehaviour
{
    private Rigidbody rb;
    public float lifespan = 15f;

    public bool inFire;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * 10, ForceMode.Impulse);
        transform.rotation = Quaternion.Euler(0, Random.Range(75, 105), 0);
    }

    private void Update()
    {
        if (lifespan <= 0)
        {
            EventHandler.i.WoodInCooker.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Belt") && EventHandler.i.DayIsGoing)
        {
            rb.AddForce(other.transform.up * -0.5f, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cooker") && !inFire)
        {
            inFire = true;
            EventHandler.i.WoodInCooker.Add(gameObject);
        }
        else if (other.CompareTag("BaggerCollector"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("EndOfBelt"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Trash"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cooker"))
        {
            inFire = false;
            EventHandler.i.WoodInCooker.Remove(gameObject);
        }
    }

    [SerializeField] AudioSource WoodSound;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }

        WoodSound.pitch = Random.Range(0.8f, 1.2f);
        WoodSound.Play();
    }
}
