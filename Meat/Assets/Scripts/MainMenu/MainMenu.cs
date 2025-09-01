using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Animator fadeOut;
    [SerializeField] GameObject[] lights;
    [SerializeField] GameObject[] lightsObj;
    [SerializeField] GameObject[] text;
    [SerializeField] TMP_Text DifficultyText;

    [SerializeField] AudioSource Lightbulb;
    [SerializeField] AudioSource Select;
    [SerializeField] AudioSource Music;

    [SerializeField] GameObject ContinueText;
    [SerializeField] GameObject ContinueButton;
    [SerializeField] GameObject First;
    [SerializeField] GameObject Second;

    bool newGameSelected = false;

    private void Start()
    {
        PlayerPrefs.SetInt("Day", 5);
        Music.volume = 0.4f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateDifficulty(0);
        StartCoroutine(Anim());
        StartCoroutine(reduceCooldown());

        if (!(PlayerPrefs.HasKey("Day") || PlayerPrefs.HasKey("Difficulty")))
        {
            ContinueButton.SetActive(false);
            ContinueText.SetActive(false);
        }
    }

    bool pressedEnter = false;
    bool cooldownPassed = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !pressedEnter && cooldownPassed && newGameSelected)
        {
            PlayerPrefs.SetInt("Difficulty", DifficultyHandler.difficulty);
            StartCoroutine(Play());
            pressedEnter = true;
            DifficultyHandler.cont = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && newGameSelected)
        {
            newGameSelected = false;
            First.SetActive(true);
            Second.SetActive(false);
        }
    }

    IEnumerator reduceCooldown()
    {
        yield return new WaitForSeconds(1f);
        cooldownPassed = true;
    }

    IEnumerator Anim()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
        anim.SetBool("Play", true);
        yield return new WaitForSeconds(6.5f);
        anim.SetBool("Play", false);
        StartCoroutine(Anim());
    }

    IEnumerator Play()
    {
        StartCoroutine(fadeOutMusic());

        First.SetActive(false);

        foreach (GameObject g in lights)
        {
            g.SetActive(false);
        }

        foreach (GameObject g in lightsObj)
        {
            g.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
        }

        foreach (GameObject g in text)
        {
            g.SetActive(false);
        }

        Lightbulb.Play();
         
        yield return new WaitForSeconds(1f);

        fadeOut.SetBool("Out", true);

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("Main");
    }

    IEnumerator fadeOutMusic()
    {
        while (Music.volume > 0)
        {
            Music.volume -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void UpdateDifficulty(int dir)
    {
        if (dir != 0)
        {
            Select.Play();
            if (dir > 0)
            {
                if (DifficultyHandler.difficulty < 2)
                {
                    DifficultyHandler.difficulty++;
                }
            }
            else
            {
                if (DifficultyHandler.difficulty > 0)
                {
                    DifficultyHandler.difficulty--;
                }
            }
        }

        if (DifficultyHandler.difficulty == 0)
        {
            DifficultyText.text = "Hard";
        }
        else if (DifficultyHandler.difficulty == 1)
        {
            DifficultyText.text = "Medium";
        }
        else
        {
            DifficultyText.text = "Easy";
        }
    }

    public void NewGame()
    {
        Select.Play();
        newGameSelected = true;
        First.SetActive(false);
        Second.SetActive(true);
    }

    public void Continue()
    {
        DifficultyHandler.difficulty = PlayerPrefs.GetInt("Difficulty");
        StartCoroutine(Play());
        pressedEnter = true;
        DifficultyHandler.cont = true;
    }
}
