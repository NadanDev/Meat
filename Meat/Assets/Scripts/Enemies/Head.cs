using UnityEngine;
using System.Collections;

public class Head : MonoBehaviour
{
    [SerializeField] GameObject StartDayButton;
    [SerializeField] GameObject LogButton;
    [SerializeField] GameObject humanoid;
    [SerializeField] GameObject TempImage;
    [SerializeField] Animator StartUI;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject PlayerCam;
    [SerializeField] GameObject ComputerCam;
    [SerializeField] GameObject CuttingBoardCam;
    [SerializeField] GameObject ThisCam;
    [SerializeField] LookingAt Parent;
    [SerializeField] AudioSource Squish;
    [SerializeField] AudioSource SpiderScare;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !Parent.isScaring)
        {
            Parent.isScaring = true;
            SpiderScare.Play();

            Player.GetComponent<Interact>().ThrowFunc();
            Player.GetComponent<Interact>().ThrowFunc();
            Player.GetComponent<Interact>().StopBagger();
            StartDayButton.SetActive(true);
            LogButton.SetActive(false);
            EventHandler.i.DailyLog.Clear();
            GameObject.FindGameObjectWithTag("Computer").GetComponent<ComputerScript>().updateLog();
            EventHandler.i.IsJumpScare = true;
            EventHandler.i.finishDay(0);
            EventHandler.i.resetForNewDay();
            StartCoroutine(Scare(EventHandler.i.Day));
        }
    }

    IEnumerator Scare(int day)
    {
        if (Player.GetComponent<Interact>().isComputer)
        {
            Player.GetComponent<Interact>().Access();
        }

        Player.GetComponent<PlayerMove>().enabled = false;
        Player.GetComponent<CameraRotation>().enabled = false;
        Player.GetComponent<CameraRotation>().yRot = -90f;
        PlayerCam.gameObject.SetActive(false);
        CuttingBoardCam.SetActive(false);
        ComputerCam.SetActive(false);
        ThisCam.gameObject.SetActive(true);

        GetComponent<Animator>().SetBool("Scare", true);

        yield return new WaitForSeconds(0.5f);
        Squish.Play();
        yield return new WaitForSeconds(0.25f);

        StartUI.SetBool("FadeOut", true);
        yield return new WaitForSeconds(5f);
        TempImage.SetActive(true);
        StartUI.SetBool("FadeOut", false);

        if (day == 1)
        {
            EventHandler.i.runStartingAnimCoroutine("MONDAY", false);
        }
        else if (day == 2)
        {
            EventHandler.i.runStartingAnimCoroutine("TUESDAY", false);
        }
        else if (day == 3)
        {
            EventHandler.i.runStartingAnimCoroutine("WEDNESDAY", false);
        }
        else if (day == 4)
        {
            EventHandler.i.runStartingAnimCoroutine("THURSDAY", false);
        }
        else if (day == 5)
        {
            EventHandler.i.runStartingAnimCoroutine("FRIDAY", false);
        }
        yield return new WaitForSeconds(0.05f);
        TempImage.SetActive(false);

        GetComponent<Animator>().SetBool("Scare", false);

        PlayerCam.transform.rotation = Quaternion.Euler(0, 90f, 0);
        Player.transform.position = new Vector3(-19f, 2, 21.6f);

        Player.GetComponent<PlayerMove>().enabled = true;
        Player.GetComponent<CameraRotation>().enabled = true;
        PlayerCam.gameObject.SetActive(true);
        ThisCam.gameObject.SetActive(false);

        EventHandler.i.EnableHumanoid = false;
        humanoid.GetComponent<Humanoid>().madeItToRoom = false;
        humanoid.GetComponent<Humanoid>().calledNoisesAlready = false;
        humanoid.transform.position = new Vector3(80f, 1.725f, 21.6f);
        humanoid.gameObject.SetActive(false);
        EventHandler.i.switchLight(0);

        yield return new WaitForSeconds(2f);

        EventHandler.i.SpiderStage = 0;
        transform.position = new Vector3(-18.11f, 0.186f, 14.57f);
        transform.rotation = Quaternion.Euler(-8.364f, -48.652f, -0.658f);

        Parent.isScaring = false;
        gameObject.SetActive(false);
    }
}
