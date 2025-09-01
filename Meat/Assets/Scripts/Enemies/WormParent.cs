using UnityEngine;

public class WormParent : MonoBehaviour
{
    public GameObject[] children;
    private void Update()
    {
        if (!children[0].activeInHierarchy && !children[1].activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }
}
