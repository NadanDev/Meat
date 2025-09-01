using UnityEngine;

public class SprayParticle : MonoBehaviour
{
    public GameObject parent;

    private void Update()
    {
        if (parent.activeInHierarchy == false)
        {
            Destroy(gameObject);
        }
    }
}
