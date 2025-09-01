using UnityEngine;
using System.Collections;

public class DieAfterSeconds : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(destroy());
    }
    IEnumerator destroy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
