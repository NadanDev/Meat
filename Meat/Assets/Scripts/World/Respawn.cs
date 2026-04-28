using UnityEngine;

public class Respawn : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.y <= -5f)
        {
            transform.position = new Vector3(-30, 1, 20);
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }
}
