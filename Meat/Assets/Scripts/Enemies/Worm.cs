using UnityEngine;

public class Worm : MonoBehaviour
{
    [SerializeField] GameObject BloodEffect;
    [SerializeField] AudioSource DieSound;

    public float health = 5f;

    private void Update()
    {
        if (health <= 0)
        {
            GameObject.FindGameObjectWithTag("WormDeath").transform.position = transform.position;
            DieSound.Play();
            Instantiate(BloodEffect, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}
