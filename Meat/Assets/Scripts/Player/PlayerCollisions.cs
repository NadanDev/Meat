using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField] Animator Shot;
    [SerializeField] Animator Blood;
    [SerializeField] Animator ShadowAnim;
    [SerializeField] GameObject playerCam;
    [SerializeField] GameObject orientation;
    [SerializeField] GameObject EndOfHallPointer;
    [SerializeField] GameObject Shadow;
    [SerializeField] GameObject WaitUntilClick;
    [SerializeField] AudioSource Step;
    [SerializeField] AudioSource GunSound;
    [SerializeField] AudioSource Thud;
    [SerializeField] TMP_Text textBox;
    [SerializeField] AudioSource GroundSound;
    [SerializeField] AudioSource GroundSound2;
    [SerializeField] AudioSource Ambience;
    [SerializeField] AudioSource Talk;
    [SerializeField] Rigidbody rb;

    bool inHall;
    bool shot;
    bool click;
    bool prompt;

    string[] lines = { 
        "Thank you so much for all the work you've done this week.",
        "Who would have thought you'd make it all the way to the end.",
        "The monsters took out everyone else before they could finish.",
        "But now that you've worked here I can't just let you leave alive.",
        "Don't worry though,",
        "you'll be great product for our future customers."
    };

    private void Update()
    {
        if (inHall && !shot)
        {
            GetComponent<CameraRotation>().enabled = false;
            GetComponent<PlayerMove>().inHall = true;

            Quaternion lookDir = Quaternion.LookRotation(EndOfHallPointer.transform.position - transform.position);

            playerCam.transform.rotation = Quaternion.Slerp(playerCam.transform.rotation, lookDir, 1f * Time.deltaTime);
            orientation.transform.rotation = Quaternion.Slerp(orientation.transform.rotation, lookDir, 1f * Time.deltaTime);
        }

        if (Input.GetMouseButtonDown(0) && prompt)
        {
            click = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HallColliderStart"))
        {
            gameObject.GetComponent<Interact>().ThrowFunc();
            gameObject.GetComponent<Interact>().ThrowFunc();

            inHall = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Interact>().AccessUnpause();
            EventHandler.i.IsInEnd = true;
        }
        if (other.CompareTag("HallCollider"))
        {
            shot = true;
            GetComponent<PlayerMove>().enabled = false;
            rb.linearVelocity = Vector3.zero;
            Shot.SetBool("Shot", true);

            StartCoroutine(Dialogue());
        }
    }

    IEnumerator Dialogue()
    {
        //yield return new WaitForSeconds(0.325f);
        GunSound.Play();
        yield return new WaitForSeconds(0.15f);
        Blood.SetBool("Blood", true);
        yield return new WaitForSeconds(1f);
        GroundSound.Play();
        yield return new WaitForSeconds(1.9f);
        GroundSound2.Play();
        Shadow.SetActive(true);
        ShadowAnim.SetBool("Shadow", true);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.2f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.3f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.4f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.5f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.6f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.7f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.8f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.9f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 1f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        textBox.enabled = true;
        textBox.text = "";
        for (int i = 0; i < lines[0].Length; i++)
        {
            Talk.Play();
            textBox.text += lines[0][i];
            if (lines[0][i] != ' ') { yield return new WaitForSeconds(0.085f); }
        }

        prompt = true;
        WaitUntilClick.SetActive(true);
        yield return new WaitUntil(() => click);
        WaitUntilClick.SetActive(false);
        click = false;
        prompt = false;

        textBox.text = "";
        for (int i = 0; i < lines[1].Length; i++)
        {
            Talk.Play();
            textBox.text += lines[1][i];
            if (lines[1][i] != ' ') { yield return new WaitForSeconds(0.085f); }
        }

        prompt = true;
        yield return new WaitUntil(() => click);
        click = false;
        prompt = false;

        textBox.text = "";
        for (int i = 0; i < lines[2].Length; i++)
        {
            Talk.Play();
            textBox.text += lines[2][i];
            if (lines[2][i] != ' ') { yield return new WaitForSeconds(0.085f); }
        }

        prompt = true;
        yield return new WaitUntil(() => click);
        click = false;
        prompt = false;

        textBox.text = "";
        for (int i = 0; i < lines[3].Length; i++)
        {
            Talk.Play();
            textBox.text += lines[3][i];
            if (lines[3][i] != ' ') { yield return new WaitForSeconds(0.085f); }
        }

        prompt = true;
        yield return new WaitUntil(() => click);
        click = false;
        prompt = false;

        textBox.text = "";
        for (int i = 0; i < lines[4].Length; i++)
        {
            Talk.Play();
            textBox.text += lines[4][i];
            if (lines[4][i] != ' ') { yield return new WaitForSeconds(0.085f); }
        }

        prompt = true;
        yield return new WaitUntil(() => click);
        click = false;
        prompt = false;

        textBox.text = "";
        for (int i = 0; i < lines[5].Length; i++)
        {
            Talk.Play();
            textBox.text += lines[5][i];
            if (lines[5][i] != ' ') { yield return new WaitForSeconds(0.085f); }
        }

        yield return new WaitForSeconds(1f);

        ShadowAnim.SetBool("ShadowLeave", true);
        StartCoroutine(WalkAway());
        Blood.SetBool("PassOut", true);
        yield return new WaitForSeconds(7.5f);
        StartCoroutine(FadeOutSound());
        Thud.Play();
        yield return new WaitForSeconds(8.0f);
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator WalkAway()
    {
        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.1f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.9f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.8f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.7f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.6f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.5f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.4f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.3f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);

        Step.pitch = Random.Range(0.7f, 1.3f);
        Step.volume = 0.2f;
        Step.Play();
        yield return new WaitForSeconds(0.8f);
    }

    IEnumerator FadeOutSound()
    {
        while (Ambience.volume > 0)
        {
            Ambience.volume -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
