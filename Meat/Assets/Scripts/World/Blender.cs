using UnityEngine;
using System.Collections;

public class Blender : MonoBehaviour
{
    [SerializeField] GameObject Jar;
    bool disableExtraTriggers = false;
    bool alreadySpawning = false;

    [SerializeField] AudioSource BlenderSound;
    [SerializeField] AudioSource BlenderLongSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!disableExtraTriggers)
        {
            if (other.CompareTag("Meat") && other.name != "trigger")
            {
                Destroy(other.gameObject.transform.parent.gameObject);
                disableExtraTriggers = true;
                StartCoroutine(resetExtraTriggers());
                StartCoroutine(Spawn(true));
            }
            else if (other.CompareTag("MeatLeft") && other.name != "trigger" || other.CompareTag("MeatRight") && other.name != "trigger")
            {
                Destroy(other.gameObject.transform.parent.gameObject);
                disableExtraTriggers = true;
                StartCoroutine(resetExtraTriggers());
                StartCoroutine(Spawn(false));
            }
            else if (other.CompareTag("Blender"))
            {
                return;
            }
            else if (other.name != "trigger")
            {
                if (other.CompareTag("Jar"))
                {
                    Destroy(other.gameObject);
                }
                else
                {
                    Destroy(other.gameObject.transform.parent.gameObject);
                }
                disableExtraTriggers = true;
                StartCoroutine(resetExtraTriggers());
            }
        }
    }

    IEnumerator Spawn(bool two)
    {
        yield return new WaitUntil(() => !alreadySpawning);
        alreadySpawning = true;
        if (two)
        {
            BlenderLongSound.Play();
            yield return new WaitForSeconds(15f);
            Instantiate(Jar, new Vector3(-31f, 2.5f, 9.925f), Quaternion.identity);
            yield return new WaitForSeconds(6f);
            Instantiate(Jar, new Vector3(-31f, 2.5f, 9.925f), Quaternion.identity);
        }
        else
        {
            BlenderSound.Play();
            yield return new WaitForSeconds(6f);
            Instantiate(Jar, new Vector3(-31f, 2.5f, 9.925f), Quaternion.identity);
        }
        alreadySpawning = false;
    }

    IEnumerator resetExtraTriggers()
    {
        yield return new WaitForSeconds(0.1f);
        disableExtraTriggers = false;
    }
}
