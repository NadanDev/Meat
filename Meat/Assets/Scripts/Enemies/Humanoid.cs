using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Humanoid : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject PlayerCam;
    [SerializeField] GameObject ComputerCam;
    [SerializeField] GameObject CuttingBoardCam;
    [SerializeField] GameObject BenchCam;
    [SerializeField] GameObject ThisCam;
    [SerializeField] GameObject StartDayButton;
    [SerializeField] GameObject LogButton;
    [SerializeField] GameObject TempImage;
    [SerializeField] GameObject Head;
    [SerializeField] Animator StartUI;
    [SerializeField] Animator Door;

    float speed = 20f;
    public bool madeItToRoom = false;
    public bool calledNoisesAlready = false;
    public bool isScaring = false;

    [SerializeField] AudioSource Step;
    [SerializeField] AudioSource Roar;
    [SerializeField] AudioSource Ambience;
    [SerializeField] AudioSource Meat;
    [SerializeField] AudioSource Bite;
    [SerializeField] AudioSource Slam;
    private float stepCoolDown = 0f;
    private float stepRate = 0.3f;

    private void Update()
    {
        float step = speed * Time.deltaTime;
        
        if (EventHandler.i.EnableHumanoid && !isScaring)
        {
            if (EventHandler.i.DoorClosed)
            {
                Ambience.volume = 0.5f;
                Roar.volume = 0.5f;
                Step.volume = 0.5f;
                Meat.volume = 0.5f;
            }
            else
            {
                Ambience.volume = 1f;
                Roar.volume = 1f;
                Step.volume = 1f;
                Meat.volume = 1f;
            }

            if (Vector3.Distance(transform.position, new Vector3(-14f, 1.725f, 21.5f)) < 0.25f || madeItToRoom)
            {
                madeItToRoom = true;

                if (EventHandler.i.LightsOff && EventHandler.i.DoorClosed)
                {
                    EventHandler.i.EnableHumanoid = false;
                    StartCoroutine(RunAway());
                }
                else
                {
                    if (EventHandler.i.DoorClosed)
                    {
                        Slam.Play();
                        Door.SetBool("Slam", true);
                        EventHandler.i.DoorClosed = false;
                    }
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(Player.transform.position.x, 1.725f, Player.transform.position.z), step);
                    Vector3 towards = Player.transform.position - transform.position;
                    Quaternion rot = Quaternion.LookRotation(towards);
                    transform.rotation = rot;
                }
            }
            else if (!madeItToRoom)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(-14f, 1.725f, 21.5f), step);
                Vector3 towards = Player.transform.position - transform.position;
                Quaternion rot = Quaternion.LookRotation(towards);
                transform.rotation = rot;
            }

            if (!calledNoisesAlready)
            {
                StartCoroutine(Noises());
            }
        }

        if (EventHandler.i.EnableHumanoid && !isScaring)
        {
            stepCoolDown -= Time.deltaTime;
            if (stepCoolDown < 0f)
            {
                Step.Play();
                Meat.Play();
                stepCoolDown = stepRate;
            }
        }

        if (!EventHandler.i.EnableHumanoid && transform.position != new Vector3(80f, 1.725f, 21.6f))
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(80f, 1.725f, 21.6f), step);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isScaring)
        {
            isScaring = true;

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

    IEnumerator RunAway()
    {
        yield return new WaitForSeconds(5f);
        madeItToRoom = false;

        calledNoisesAlready = false;
        transform.position = new Vector3(80f, 1.725f, 21.6f);
        gameObject.SetActive(false);
        EventHandler.i.switchLight(0);
    }

    IEnumerator Scare(int day)
    {
        madeItToRoom = false;

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
        BenchCam.SetActive(false);
        ThisCam.gameObject.SetActive(true);

        GetComponent<Animator>().SetBool("Scare", true);
        Bite.Play();

        yield return new WaitForSeconds(0.75f);
        EventHandler.i.StartCoroutine("fadeOutAmbience", 99);

        StartUI.SetBool("FadeOut", true);
        yield return new WaitForSeconds(5f);
        TempImage.SetActive(true);
        StartUI.SetBool("FadeOut", false);

        StartCoroutine(FadeOutSound());
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
        calledNoisesAlready = false;

        EventHandler.i.SpiderStage = 0;
        Head.transform.position = new Vector3(-18.11f, 0.186f, 14.57f);
        Head.transform.rotation = Quaternion.Euler(-8.364f, -48.652f, -0.658f);

        EventHandler.i.EnableHumanoid = false;
        EventHandler.i.switchLight(0);

        yield return new WaitForSeconds(2f);
        transform.position = new Vector3(80f, 1.725f, 21.6f);
        transform.rotation = Quaternion.Euler(0, -90, 0);
        yield return new WaitForSeconds(0.1f);

        isScaring = false;
        gameObject.SetActive(false);

        if (EventHandler.i.SkipTo > 0)
        {
            GameObject.FindGameObjectWithTag("Computer").GetComponent<ComputerScript>().startDay();
        }
        else
        {
            GameObject.Find("Player").GetComponent<Interact>().StartCoroutine("LoadFromBench", true);
        }
    }

    IEnumerator Noises()
    {
        calledNoisesAlready = true;

        Ambience.Play();
        Roar.Play();
        yield return new WaitForSeconds(3f);
        Roar.Play();
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