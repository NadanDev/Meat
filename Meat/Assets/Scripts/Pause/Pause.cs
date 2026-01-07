using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Pause : MonoBehaviour
{
    [SerializeField] Scrollbar Sensitivity;
    [SerializeField] Scrollbar Volume;
    [SerializeField] AudioMixer mixer;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject OptionsMenu;
    [SerializeField] GameObject ControlsMenu;
    [SerializeField] GameObject MainPauseMenu;
    [SerializeField] TMP_Text DifficultyText;

    private void Start()
    {
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

    public void OpenOptions()
    {
        MainPauseMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void OpenControls()
    {
        MainPauseMenu.SetActive(false);
        ControlsMenu.SetActive(true);
    }

    public void Back()
    {
        OptionsMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        MainPauseMenu.SetActive(true);
    }

    public void RestartDay()
    {
        DifficultyHandler.restart = true;
        SceneManager.LoadScene("MainMenu");
    }

    public void SensitivityChanged()
    {
        Player.GetComponent<CameraRotation>().mouseSensitivity = 400 * (Mathf.Pow(Sensitivity.value, 2));
    }

    public void VolumeChanged()
    {
        mixer.SetFloat("Volume", Volume.value * 50f - 25f);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateDifficulty(int dir)
    {
        if (dir != 0)
        {
            if (dir > 0)
            {
                if (DifficultyHandler.difficulty < 2)
                {
                    DifficultyHandler.difficulty++;
                    PlayerPrefs.SetInt("Difficulty", DifficultyHandler.difficulty);
                }
            }
            else
            {
                if (DifficultyHandler.difficulty > 0)
                {
                    DifficultyHandler.difficulty--;
                    PlayerPrefs.SetInt("Difficulty", DifficultyHandler.difficulty);
                }
            }
        }

        EventHandler.i.Difficulty = DifficultyHandler.difficulty;
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
}
