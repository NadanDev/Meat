using UnityEngine;
using System.Collections;

public class BurntMeat : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(life());
    }

    IEnumerator life()
    {
        yield return new WaitForSeconds(30f);
        Destroy(gameObject);
    }
}
