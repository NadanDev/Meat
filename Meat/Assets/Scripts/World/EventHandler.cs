using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class EventHandler : MonoBehaviour
{
    public static EventHandler i { get; private set; }

    private int day = 1;
    private int skipTo = 0;
    private int curOrder = 0;
    private int piecesInBagger = 0;
    private int piecesInHeldBag = 0;
    private int foodFed = 0;
    private int spiderStage = 0;
    private int totalDayErrors = 0;
    private int difficulty = 0; // 0 = Hard, 1 = Medium, 2 = Easy

    private float timer = 0;
    private float failedPercentAvg;

    private bool meatOnBoard = false;
    private bool dayIsGoing = false;
    private bool lightsOff = false;
    private bool tutorialOrderComplete = false;
    private bool retryTutorialOrder = false;
    private bool dayDone = false;
    private bool enableHumanoid = false;
    private bool isJumpScare = false;
    private bool doorClosed = true;
    private bool cookerOn = false;
    private bool isInStartingAnim = false;
    private bool isInEnd = false;
    private bool notInDay = false;

    private string[] orderContents = new string[16];
    private string[] summaries = new string[5];
    private List<string> dailyLog = new List<string>();

    // 0: Small meat, 1: big meat, 2: small bag, 3: smMedBag, 4: bigMedBag, 5: bigBag, 6: cooked light small meat, 7: cooked well small meat, 8: cooked light big meat, 9: cooked well big meat, 10: jar
    private int[] ordersSent = new int[11]; // How many of each type is sent
    private int[,] orders = new int[16, 11]; // What needs to be done
    private int[] ordersCombined = new int[11]; // What needs to be done (combined into one)
    private int[] errors = new int[11];
    private int[] errorsMidOrder = new int[11];

    private bool[] ordersComplete = new bool[16]; // Which of the 16 orders are complete
    private bool[] ordersFailed = new bool[16]; // Failed

    private GameObject boardMeat;
    private GameObject[] woodPieces = new GameObject[5];
    private List<GameObject> woodInCooker = new List<GameObject>();

    // AUDIO
    public GameObject ConveyorAudioHolder;
    private AudioSource[] conveyorAudioSources;
    public AudioSource Alarm;

    // BEGINNING
    private void Awake()
    {
        StartCoroutine(PlayRandomSoundRoutine());

        difficulty = DifficultyHandler.difficulty;
        skipTo = PlayerPrefs.GetInt("SkipTo", 0);

        if (i != null && i != this)
        {
            Destroy(this);
        }
        else
        {
            i = this;
        }

        conveyorAudioSources = ConveyorAudioHolder.GetComponents<AudioSource>();

        summaries[0] = "Orders Today: 4\n\nNo data for the first day";
        summaries[1] = "Orders Today: 4\n\nNo data for the second day";
        summaries[2] = "Orders Today: 7\n\nNo data for the third day";
        summaries[3] = "Orders Today: 9\n\nNo data for the fourth day";
        summaries[4] = "Orders Today: 10\n\nNo data for the fifth day";

        if (DifficultyHandler.cont)
        {
            dayDone = true;
            day = PlayerPrefs.GetInt("Day", 1);

            if (day == 1)
            {
                newMessageNotif.SetActive(true);

                dayDone = false;
                StartCoroutine(StartingUIAnim("MONDAY", false));
            }
            else if (day == 2)
            {
                day = 1;
                computerScript.clockOut(true, false);
            }
            else if (day == 3)
            {
                day = 2;
                computerScript.clockOut(true, false);
            }
            else if (day == 4)
            {
                day = 3;
                computerScript.clockOut(true, false);
            }
            else if (day == 5)
            {
                day = 4;
                computerScript.clockOut(true, false);
            }
        }
        else
        {
            newMessageNotif.SetActive(true);

            PlayerPrefs.SetInt("Day", 1);
            PlayerPrefs.SetInt("SkipTo", 0);
            StartCoroutine(StartingUIAnim("MONDAY", false));
        }
    }

    public int Day
    {
        get => day;
        set { day = value; }
    }
    public int SkipTo
    {
        get => skipTo;
        set { skipTo = value; }
    }
    public int CurOrder
    {
        get => curOrder;
        set { curOrder = value; }
    }
    public int PiecesInBagger
    {
        get => piecesInBagger;
        set { piecesInBagger = value; }
    }
    public int PiecesInHeldBag
    {
        get => piecesInHeldBag;
        set { piecesInHeldBag = value; }
    }
    public int FoodFed
    { 
        get => foodFed;
        set { foodFed = value; }
    }
    public int SpiderStage
    {
        get => spiderStage;
        set { spiderStage = value; }
    }
    public int TotalDayErrors
    {
        get => totalDayErrors;
        set { totalDayErrors = value; }
    }
    public int Difficulty
    {
        get => difficulty;
        set { difficulty = value; }
    }

    public float Timer
    {
        get => timer;
        set { timer = value; }
    }
    public float FailedPercentAvg
    {
        get => failedPercentAvg;
        set { failedPercentAvg = value; }
    }


    public bool DayIsGoing
    {
        get => dayIsGoing;
        set { dayIsGoing = value; }
    }
    public bool MeatOnBoard
    {
        get => meatOnBoard;
        set { meatOnBoard = value; }
    }
    public bool LightsOff
    {
        get => lightsOff;
        set { lightsOff = value; }
    }
    public bool TutorialOrderComplete
    {
        get => tutorialOrderComplete;
        set { tutorialOrderComplete = value; }
    }
    public bool RetryTutorialOrder
    {
        get => retryTutorialOrder;
        set { retryTutorialOrder = value; }
    }
    public bool DayDone
    {
        get => dayDone;
        set { dayDone = value; }
    }
    public bool EnableHumanoid
    {
        get => enableHumanoid;
        set { enableHumanoid = value;  }
    }
    public bool IsJumpScare
    {
        get => isJumpScare;
        set { isJumpScare = value; }
    }
    public bool DoorClosed
    {
        get => doorClosed;
        set { doorClosed = value; }
    }
    public bool CookerOn
    {
        get => cookerOn;
        set { cookerOn = value; }
    }
    public bool IsInStartingAnim
    {
        get => isInStartingAnim;
        set { isInStartingAnim = value; }
    }
    public bool IsInEnd
    {
        get => isInEnd;
        set { isInEnd = value; }
    }
    public bool NotInDay
    {
        get => notInDay;
        set { notInDay = value; }
    }


    public string[] OrderContents
    {
        get => orderContents;
        set { orderContents = value; }
    }
    public string[] Summaries
    {
        get => summaries;
        set { summaries = value; }
    }
    public List<string> DailyLog
    {
        get => dailyLog;
        set { dailyLog = value; }
    }


    public int[] OrdersSent
    {
        get => ordersSent;
        set { ordersSent = value; }
    }
    public int[] OrdersCombined
    {
        get => ordersCombined;
        set { ordersCombined = value; }
    }
    public int[] Errors
    {
        get => errors;
        set { errors = value; }
    }
    public int[] ErrorsMidOrder
    {
        get => errorsMidOrder;
        set { errorsMidOrder = value; }
    }
    public int[,] Orders
    {
        get => orders;
        set { orders = value; }
    }


    public bool[] OrdersComplete
    {
        get => ordersComplete;
        set { ordersComplete = value; }
    }
    public bool[] OrdersFailed
    {
        get => ordersFailed;
        set { ordersFailed = value; }
    }


    public GameObject BoardMeat
    {
        get => boardMeat;
        set { boardMeat = value; }
    }
    public GameObject[] WoodPieces
    {
        get => woodPieces;
        set { woodPieces = value; }
    }
    public List<GameObject> WoodInCooker
    {
        get => woodInCooker;
        set { woodInCooker = value; }
    }








    // --------------------------------------- LIGHTS ---------------------------------------
    public GameObject redLightObject;
    public Light redLight;
    public GameObject light1, light2, light3, light4;
    public Light[] handheldLights;


    public void switchLight(int whichLight)
    {
        if (whichLight == 0)
        {
            light1.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");
            light2.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");
            light3.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");
            light4.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");

            handheldLights[0].enabled = true;
            handheldLights[1].enabled = true;
            handheldLights[2].enabled = true;
            handheldLights[3].enabled = true;
        }
        else if (whichLight == 1)
        {
            light1.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
            light2.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");
            light3.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");
            light4.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");

            handheldLights[0].enabled = false;
            handheldLights[1].enabled = true;
            handheldLights[2].enabled = true;
            handheldLights[3].enabled = true;
        }
        else if (whichLight == 2)
        {
            light1.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
            light2.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
            light3.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");
            light4.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");

            handheldLights[0].enabled = false;
            handheldLights[1].enabled = false;
            handheldLights[2].enabled = true;
            handheldLights[3].enabled = true;
        }
        else if (whichLight == 3)
        {
            light1.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
            light2.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
            light3.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
            light4.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");

            handheldLights[0].enabled = false;
            handheldLights[1].enabled = false;
            handheldLights[2].enabled = false;
            handheldLights[3].enabled = true;
        }
        else if (whichLight == 4)
        {
            light1.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
            light2.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
            light3.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
            light4.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");

            handheldLights[0].enabled = false;
            handheldLights[1].enabled = false;
            handheldLights[2].enabled = false;
            handheldLights[3].enabled = false;
        }
    }

    IEnumerator switchRed()
    {
        redLight.enabled = true;
        redLightObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");

        yield return new WaitForSeconds(1f);

        redLight.enabled = false;
        redLightObject.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
    }
    // --------------------------------------- END LIGHTS ---------------------------------------








    public GameObject Meat;
    public GameObject RetryButton;
    public GameObject MonsterHumanoid;
    public GameObject CookerSwitch;
    public GameObject FireEffect;
    public GameObject Arm;
    public GameObject Liver;
    public GameObject FinalMessage;
    public GameObject MessagesLeftArrow;
    public GameObject MessagesRightArrow;
    public GameObject OrdersLeftArrow;
    public GameObject OrdersRightArrow;
    public GameObject SummaryLeftArrow;
    public GameObject SummaryRightArrow;
    public GameObject finalMessageNotif;
    public GameObject newMessageNotif;
    public Worm[] wormBaggerChildren;
    public Worm[] wormCookerChildren;
    public Collider DoorCollider;
    public ComputerScript computerScript;
    public GameObject[] OrderTags;
    public GameObject[] OrderCheckmarks;
    public GameObject[] OrderX;
    public GameObject[] WormMonsters;
    public Image[] UnreadsSummary;
    public Image ClockOut;
    public TMP_Text StartUIText;
    public TMP_Text TimeText;
    public TMP_Text TimeDisplayText;
    public Animator FadeIn;
    public Animator Door;
    public Animator Controls;
    public AudioSource Type;
    public AudioSource SpiderRoar;
    public AudioSource SpiderInRoom;
    public AudioSource OrderComplete;
    public AudioSource OrderFailed;
    public AudioSource WrongOrder;
    public AudioSource RightOrder;
    public AudioSource MonsterCall;
    public AudioSource spiderEat;
    public AudioSource Ambience;
    public AudioSource Legs;
    public AudioSource Bang1;
    public AudioSource Bang2;
    public AudioSource Bang3;

    private bool startTimer = false;
    private bool spiderInEffect = false;
    private bool runnerInEffect = false;
    private bool wormInEffect = false;

    private int attempt = 0;

    private void Update()
    {
        if (startTimer)
        {
            timer += Time.deltaTime;
            TimeText.text = "Time: " + (int)timer;
            TimeDisplayText.text = $"{(int)timer}";
        }
        else
        {
            TimeDisplayText.text = "";
        }

        if (cookerOn && woodInCooker.Count > 0)
        {
            woodInCooker[0].GetComponent<Wood>().lifespan -= Time.deltaTime;
        }
        else if (cookerOn)
        {
            cookerOn = false;
            CookerSwitch.transform.localRotation = Quaternion.Euler(-90f, 0, 0);
            GameObject.Find("Player").GetComponent<Interact>().cookerSwitchFlipped = false;
            FireEffect.SetActive(false);
        }
    }

    IEnumerator PlayRandomSoundRoutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => dayIsGoing);

            float duration = Random.Range(20f, 80f);
            yield return new WaitForSeconds(duration);
            if (!dayIsGoing) continue;

            switch (Random.Range(0, 4))
            {
                case 0: Legs.Play(); break;
                case 1: Bang1.Play(); break;
                case 2: Bang2.Play(); break;
                case 3: Bang3.Play(); break;
            }
        }
    }

    public void startDay()
    {
        skipTo = PlayerPrefs.GetInt("SkipTo", 0);
        if (skipTo > 0) // Load failures
        {
            //totalDayErrors = PlayerPrefs.GetInt("TotalDayErrors", 0);
            string errors = PlayerPrefs.GetString("Errors");
            string current = "";
            int iteration = 0;
            for (int i = 0; i < errors.Length; i++)
            {
                if (errors[i] == ' ')
                {
                    Errors[iteration] = int.Parse(current);
                    current = "";
                    iteration++;
                    continue;
                }
                current += errors[i];
            }
            string failedOrdersString = PlayerPrefs.GetString("FailedOrders");
            for (int i = 0; i < 16; i++)
            {
                if (failedOrdersString[i] == 't')
                {
                    ordersFailed[i] = true;
                }
                else
                {
                    ordersFailed[i] = false;
                }
            }
        }

        if (day == 1)
        {
            StartCoroutine("Day1");
        }
        else if (day == 2)
        {
            StartCoroutine("Day2");
        }
        else if (day == 3)
        {
            StartCoroutine("Day3");
        }
        else if (day == 4)
        {
            StartCoroutine("Day4");
        }
        else if (day == 5)
        {
            StartCoroutine("Day5");
        }
        startTimer = true;
        dayIsGoing = true;
    }

    public void resetForNewDay()
    {
        ClockOut.color = new Color32(0, 125, 0, 255);

        attempt++;
        dayDone = false;
        meatOnBoard = false;
        orderContents = new string[16];
        ordersSent = new int[11];
        orders = new int[16, 11];
        ordersCombined = new int[11];
        errors = new int[11];
        errorsMidOrder = new int[11];
        ordersComplete = new bool[16];
        ordersFailed = new bool[16];
        piecesInBagger = 0;
        foodFed = 0;
        clearMeatFromScene();

        for (int i = 0; i < 16; i++)
        {
            OrderTags[i].SetActive(false);
            OrderCheckmarks[i].SetActive(false);
            OrderX[i].SetActive(false);
        }

        Door.SetBool("IsOpen", false);
        Door.SetBool("Slam", false);
    }

    public IEnumerator StartingUIAnim(string day, bool isFired)
    {
        if (day == "MONDAY")
        {
            OrdersLeftArrow.SetActive(false);
            OrdersRightArrow.SetActive(false);
        }
        else if (day == "TUESDAY")
        {
            OrdersLeftArrow.SetActive(false);
            OrdersRightArrow.SetActive(false);
        }
        else if (day == "WEDNESDAY")
        {
            OrdersLeftArrow.SetActive(false);
            OrdersRightArrow.SetActive(false);
            MessagesLeftArrow.SetActive(true);
            MessagesRightArrow.SetActive(true);
        }
        else if (day == "THURSDAY")
        {
            OrdersLeftArrow.SetActive(false);
            OrdersRightArrow.SetActive(false);
            MessagesLeftArrow.SetActive(true);
            MessagesRightArrow.SetActive(true);
        }
        else if (day == "FRIDAY")
        {
            OrdersLeftArrow.SetActive(false);
            OrdersRightArrow.SetActive(false);
            SummaryLeftArrow.SetActive(true);
            SummaryRightArrow.SetActive(true);
            MessagesLeftArrow.SetActive(true);
            MessagesRightArrow.SetActive(true);
        }

        isInStartingAnim = true;

        StartUIText.text = "";
        FadeIn.SetBool("Fade", true);

        if (!isFired)
        {
            StartUIText.text = "";
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < day.Length; i++)
            {
                Type.pitch = Random.Range(0.9f, 0.95f);
                Type.Play();

                StartUIText.text += day[i];
                yield return new WaitForSeconds(0.2f);
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        isInStartingAnim = false;
        FadeIn.SetBool("Fade", false);
        yield return new WaitForSeconds(1f);
        // FADE OUT SOUND (fade in)
        if (Day == 1)
        {
            StartCoroutine("fadeInAmbience", 0);
        }
        yield return new WaitForSeconds(3f);
        StartUIText.text = "";
    }

    public IEnumerator fadeOutAmbience(int multiplier = 0)
    {
        if (multiplier >= 100)
        {
            multiplier = 99;
        }
        while (Ambience.volume > 0)
        {
            Ambience.volume -= 0.01f;
            yield return new WaitForSeconds(0.02f - 0.0002f * multiplier);
        }
    }

    public IEnumerator fadeInAmbience(int multiplier = 0)
    {
        if (multiplier >= 100)
        {
            multiplier = 99;
        }
        while (Ambience.volume < 1)
        {
            Ambience.volume += 0.01f;
            yield return new WaitForSeconds(0.02f - 0.0002f * multiplier);
        }
    }

    public void runStartingAnimCoroutine(string day, bool isFired)
    {
        StartCoroutine(EventHandler.i.StartingUIAnim(day, isFired));
    }

    public void checkmarkOrder(int orderNum, bool quiet)
    {
        curOrder++;
        for (int i = 0; i < 11; i++)
        {
            errors[i] += errorsMidOrder[i];
        }
        errorsMidOrder = new int[11];

        OrderCheckmarks[orderNum].SetActive(true);
        if (!quiet) OrderComplete.Play();
    }

    private void clearMeatFromScene()
    {
        GameObject[] meat = GameObject.FindGameObjectsWithTag("Meat");
        foreach (GameObject item in meat)
        {
            Destroy(item);
        }

        GameObject[] meatLeft = GameObject.FindGameObjectsWithTag("MeatLeft");
        foreach (GameObject item in meatLeft)
        {
            Destroy(item);
        }

        GameObject[] meatRight = GameObject.FindGameObjectsWithTag("MeatRight");
        foreach (GameObject item in meatRight)
        {
            Destroy(item);
        }

        GameObject[] bag = GameObject.FindGameObjectsWithTag("Bag");
        foreach (GameObject item in bag)
        {
            Destroy(item);
        }

        GameObject[] arm = GameObject.FindGameObjectsWithTag("Arm");
        foreach (GameObject item in arm)
        {
            Destroy(item);
        }

        GameObject[] liver = GameObject.FindGameObjectsWithTag("Liver");
        foreach (GameObject item in liver)
        {
            Destroy(item);
        }

        foreach (GameObject item in WormMonsters)
        {
            item.SetActive(false);
        }
    }

    public void finishDay(int day)
    {
        startTimer = false;

        ClockOut.color = new Color32(0, 255, 0, 255);
        RetryButton.SetActive(false);
        TimeText.gameObject.SetActive(false);
        timer = 0;
        dayIsGoing = false;
        spiderStage = 0;

        StopCoroutine("Day1");
        StopCoroutine("Day2");
        StopCoroutine("Day3");
        StopCoroutine("Day4");
        StopCoroutine("Day5");

        spiderInEffect = false;
        wormInEffect = false;
        runnerInEffect = false;

        StopCoroutine("SpiderMonster");
        StopCoroutine("StartRunningMonster");
        StopCoroutine("RandomizeRunner");
        StopCoroutine("WormMonster");

        if (!isJumpScare)
        {
            MonsterHumanoid.transform.position = new Vector3(80f, 1.725f, 21.6f);
            MonsterHumanoid.gameObject.SetActive(false);
            switchLight(0);
        }

        conveyorAudioSources[1].Stop();

        if (!isJumpScare)
        {
            conveyorAudioSources[2].Play();

            int numOrdersFailed = 0;
            for (int i = 0; i < 16; i++)
            {
                if (ordersFailed[i])
                {
                    numOrdersFailed++;
                }
            }

            int ordersNum = 0;
            if (day == 0)
            {
                ordersNum = 4;
            }
            else if (day == 1)
            {
                ordersNum = 4;
            }
            else if (day == 2)
            {
                ordersNum = 7;
            }
            else if (day == 3)
            {
                ordersNum = 9;
            }
            else if (day == 4)
            {
                ordersNum = 11;
            }

            summaries[day] = $"Day {day + 1} Summary\n\nOrders Today: {ordersNum}\n\nYour errors:\n";
            if (Errors[0] > 0)
            {
                summaries[day] += $"You sent {Errors[0]} too many half pieces\n";
                totalDayErrors += Errors[0];
            }
            if (Errors[1] > 0)
            {
                summaries[day] += $"You sent {Errors[1]} too many uncut pieces\n";
                totalDayErrors += Errors[1];
            }
            if (Errors[2] > 0)
            {
                summaries[day] += $"You sent {Errors[2]} too many bags containing a half piece\n";
                totalDayErrors += Errors[2];
            }
            if (Errors[3] > 0)
            {
                summaries[day] += $"You sent {Errors[3]} too many bags containing 1 piece\n";
                totalDayErrors += Errors[3];
            }
            if (Errors[4] > 0)
            {
                summaries[day] += $"You sent {Errors[4]} too many bags containing 1 and a half pieces\n";
                totalDayErrors += Errors[4];
            }
            if (Errors[5] > 0)
            {
                summaries[day] += $"You sent {Errors[5]} too many bags containing 2 pieces\n";
                totalDayErrors += Errors[5];
            }
            if (Errors[6] > 0 && day > 1)
            {
                summaries[day] += $"You sent {Errors[6]} too many lightly cooked half pieces\n";
                totalDayErrors += Errors[6];
            }
            if (Errors[7] > 0 && day > 1)
            {
                summaries[day] += $"You sent {Errors[7]} too many well cooked half pieces\n";
                totalDayErrors += Errors[7];
            }
            if (Errors[8] > 0 && day > 1)
            {
                summaries[day] += $"You sent {Errors[8]} too many lightly cooked uncut pieces\n";
                totalDayErrors += Errors[8];
            }
            if (Errors[9] > 0 && day > 1)
            {
                summaries[day] += $"You sent {Errors[9]} too many well cooked uncut pieces\n";
                totalDayErrors += Errors[9];
            }
            if (Errors[10] > 0 && day > 3)
            {
                summaries[day] += $"You sent {Errors[10]} too many jars of meat\n";
                totalDayErrors += Errors[10];
            }

            if (numOrdersFailed > 0)
            {
                summaries[day] += $"\nYou failed {numOrdersFailed} orders and completed an average of {100 - (int)((failedPercentAvg / numOrdersFailed) * 100)}% for each\n"; 
            }
            else
            {
                summaries[day] += $"\nYou failed {numOrdersFailed} orders";
            }

            if (numOrdersFailed != 0)
            {
                totalDayErrors += (int)(3 * numOrdersFailed * (failedPercentAvg / numOrdersFailed));
            }

            UnreadsSummary[day].enabled = true;
            dayDone = true;
        }
        IsJumpScare = false;
    }

    IEnumerator OrderTimer(int time, int order, int small, int big, int bag1, int bag2, int bag3, int bag4, int cookedLSmall, int cookedWSmall, int cookedLBig, int cookedWBig, int jar)
    {
        int attemptNum = attempt;
        yield return new WaitForSeconds(time);
        if (attempt > attemptNum)
        {
            yield break;
        }

        int total = small + big + bag1 + bag2 + bag3 + bag4 + cookedLSmall + cookedWSmall + cookedLBig + cookedWBig + jar;
        int totalLeft = 0;
        if (!ordersComplete[order])
        {
            for (int i = 0; i < small; i++)
            {
                if (ordersCombined[0] > 0)
                {
                    totalLeft++;
                    ordersCombined[0]--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < big; i++)
            {
                if (ordersCombined[1] > 0)
                {
                    totalLeft++;
                    ordersCombined[1]--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < bag1; i++)
            {
                if (ordersCombined[2] > 0)
                {
                    totalLeft++;
                    ordersCombined[2]--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < bag2; i++)
            {
                if (ordersCombined[3] > 0)
                {
                    totalLeft++;
                    ordersCombined[3]--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < bag3; i++)
            {
                if (ordersCombined[4] > 0)
                {
                    totalLeft++;
                    ordersCombined[4]--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < bag4; i++)
            {
                if (ordersCombined[5] > 0)
                {
                    totalLeft++;
                    ordersCombined[5]--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < cookedLSmall; i++)
            {
                if (ordersCombined[6] > 0)
                {
                    totalLeft++;
                    ordersCombined[6]--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < cookedWSmall; i++)
            {
                if (ordersCombined[7] > 0)
                {
                    totalLeft++;
                    ordersCombined[7]--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < cookedLBig; i++)
            {
                if (ordersCombined[8] > 0)
                {
                    totalLeft++;
                    ordersCombined[8]--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < cookedWBig; i++)
            {
                if (ordersCombined[9] > 0)
                {
                    totalLeft++;
                    ordersCombined[9]--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < jar; i++)
            {
                if (ordersCombined[10] > 0)
                {
                    totalLeft++;
                    ordersCombined[10]--;
                }
                else
                {
                    break;
                }
            }

            ordersSent = new int[11];

            float percentLeft = totalLeft / (float)total;
            failedPercentAvg += percentLeft;

            DailyLog.Add($"- {(int)Timer} seconds: Order {order + 1} failed");
            ordersFailed[order] = true;
            OrderX[order].SetActive(true);
            OrderFailed.Play();
        }
    }

    IEnumerator StartRunningMonster()
    {
        switchLight(1);
        yield return new WaitForSeconds(Random.Range(10, 15));
        switchLight(2);
        yield return new WaitForSeconds(Random.Range(10, 15));
        switchLight(3);
        yield return new WaitForSeconds(Random.Range(10, 15));
        switchLight(4);
        yield return new WaitForSeconds(2.5f);
        if (lightsOff && doorClosed) { switchLight(0); yield break; }
        yield return new WaitForSeconds(2.5f);
        if (lightsOff && doorClosed) { switchLight(0); yield break; }
        yield return new WaitForSeconds(2.5f);
        if (lightsOff && doorClosed) { switchLight(0); yield break; }
        yield return new WaitForSeconds(2.5f);
        if (lightsOff && doorClosed) { switchLight(0); yield break; }
        yield return new WaitForSeconds(2.5f);
        if (lightsOff && doorClosed) { switchLight(0); yield break; }
        yield return new WaitForSeconds(2.5f);
        if (lightsOff && doorClosed) { switchLight(0); yield break; }
        MonsterCall.Play();
        yield return new WaitForSeconds(2.5f);
        if (lightsOff && doorClosed) { switchLight(0); yield break; }
        yield return new WaitForSeconds(2.5f);
        if (lightsOff && doorClosed) { switchLight(0); yield break; }

        MonsterHumanoid.SetActive(true);
        enableHumanoid = true;
    }

    IEnumerator RandomizeRunner()
    {
        while (runnerInEffect)
        {
            yield return new WaitForSeconds(Random.Range(40, 80));
            StartCoroutine("StartRunningMonster");
            yield return new WaitForSeconds(70 + (20 * difficulty));
        }
    }

    public IEnumerator DoorTimer()
    {
        if (day < 2)
        {
            yield break;
        }
        yield return new WaitUntil(() => dayIsGoing);
        yield return new WaitForSeconds(2.5f); 
        if (doorClosed) { yield break; }
        yield return new WaitForSeconds(2.5f);
        if (doorClosed) { yield break; }
        MonsterCall.Play();
        yield return new WaitForSeconds(2.5f);
        if (doorClosed) { yield break; }
        yield return new WaitForSeconds(2.5f);
        if (doorClosed) { yield break; }

        if (!doorClosed && day > 1 && dayIsGoing)
        { 
            MonsterHumanoid.SetActive(true);
            enableHumanoid = true;
        }
    }

    IEnumerator SpiderMonster()
    {
        while (spiderInEffect)
        {
            yield return new WaitForSeconds(10 + (2.5f * difficulty));
            if (spiderStage == 0)
            {
                if (foodFed > 0)
                {
                    foodFed--;
                    yield return new WaitForSeconds(20 + (2.5f * difficulty));
                }
                else
                {
                    SpiderRoar.Play();
                    spiderStage++;
                }
            }
            else if (spiderStage > 0 && spiderStage < 5)
            {
                if (foodFed > 0)
                {
                    spiderStage = 0;
                }
                else
                {
                    spiderStage++;
                    if (spiderStage == 3)
                    {
                        SpiderInRoom.Play();
                    }
                }
            }
            else if (spiderStage == 5)
            {
                if (foodFed > 0)
                {
                    spiderStage = 0;
                }
                else
                {
                    spiderStage = 5;
                }
            }
        }
    }

    IEnumerator WormMonster()
    {
        while (wormInEffect)
        {
            yield return new WaitForSeconds(Random.Range(40 + (10 * difficulty), 75 + (10 * difficulty)));
            if (day == 4)
            {
                int num = Random.Range(0, 3);
                int i = 0;
                while (i < 3)
                {
                    if (!WormMonsters[num].activeSelf)
                    {
                        if (num == 2)
                        {
                            WoodInCooker.Clear();
                            cookerOn = false;
                            CookerSwitch.transform.localRotation = Quaternion.Euler(-90f, 0, 0);
                            GameObject.Find("Player").GetComponent<Interact>().cookerSwitchFlipped = false;
                            FireEffect.SetActive(false);
                            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("WoodPiece"))
                            {
                                Destroy(obj);
                            }
                        }
                        if (num == 1)
                        {
                            wormBaggerChildren[0].health = 5;
                            wormBaggerChildren[1].health = 5;
                            wormBaggerChildren[0].gameObject.SetActive(true);
                            wormBaggerChildren[1].gameObject.SetActive(true);
                        }
                        else if (num == 2)
                        {
                            wormCookerChildren[0].health = 5;
                            wormCookerChildren[1].health = 5;
                            wormCookerChildren[0].gameObject.SetActive(true);
                            wormCookerChildren[1].gameObject.SetActive(true);
                        }
                        else
                        {
                            WormMonsters[num].GetComponent<Worm>().health = 5;
                        }
                        WormMonsters[num].SetActive(true);
                        break;
                    }
                    else
                    {
                        num = (num + 1) % 3;
                    }
                    i++;
                }
            }
            else if (day == 5)
            {
                int num = Random.Range(0, 4);
                int i = 0;
                while (i < 4)
                {
                    if (!WormMonsters[num].activeSelf)
                    {
                        if (num == 2)
                        {
                            WoodInCooker.Clear();
                            cookerOn = false;
                            CookerSwitch.transform.localRotation = Quaternion.Euler(-90f, 0, 0);
                            GameObject.Find("Player").GetComponent<Interact>().cookerSwitchFlipped = false;
                            FireEffect.SetActive(false);
                            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("WoodPiece"))
                            {
                                Destroy(obj);
                            }
                        }
                        if (num == 1)
                        {
                            wormBaggerChildren[0].health = 5;
                            wormBaggerChildren[1].health = 5;
                            wormBaggerChildren[0].gameObject.SetActive(true);
                            wormBaggerChildren[1].gameObject.SetActive(true);
                        }
                        else if (num == 2)
                        {
                            wormCookerChildren[0].health = 5;
                            wormCookerChildren[1].health = 5;
                            wormCookerChildren[0].gameObject.SetActive(true);
                            wormCookerChildren[1].gameObject.SetActive(true);
                        }
                        else
                        {
                            WormMonsters[num].GetComponent<Worm>().health = 5;
                        }
                        WormMonsters[num].SetActive(true);
                        break;
                    }
                    else
                    {
                        num = (num + 1) % 4;
                    }
                    i++;
                }
            }
        }
    }

    public void playSpiderEat()
    {
        spiderEat.Play();
    }

    int[] requestedByTimes = new int[16];
    IEnumerator Day1()
    {
        GameObject.Find("Player").GetComponent<Interact>().ExtUnclogBagger();

        failedPercentAvg = 0;

        TimeText.gameObject.SetActive(true);

        conveyorAudioSources[0].Play();
        yield return new WaitForSeconds(conveyorAudioSources[0].clip.length - 0.1f);
        conveyorAudioSources[1].Play();

        Controls.SetBool("PauseText", true);
        yield return new WaitForSeconds(3f);
        Controls.SetBool("PauseText", false);

        Day1Text();


        // ORDER 1
        yield return new WaitForSeconds(2f);
        dailyLog.Add($"- {(int)timer} seconds: Order 1 requested");
        computerScript.updateLog();
        orderContents[0] += $"";
        RetryButton.SetActive(true);
        OrderTags[0].SetActive(true);
        Orders[0, 1] = 1;
        OrdersCombined[1] += 1;
        yield return new WaitForSeconds(1f);


        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());

        while (!tutorialOrderComplete)
        {
            if (RetryTutorialOrder)
            {
                yield return new WaitForSeconds(1f);
                ordersCombined[1] = 1;
                ordersSent[1] = 0;
                clearMeatFromScene();
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
                Alarm.Play();
                StartCoroutine(switchRed());
                RetryTutorialOrder = false;
            }
            yield return null;
        }
        yield return new WaitUntil(() => tutorialOrderComplete);
        tutorialOrderComplete = false;


        // ORDER 2
        yield return new WaitForSeconds(5f);
        dailyLog.Add($"- {(int)timer} seconds: Order 2 requested");
        computerScript.updateLog();
        orderContents[1] += $"";
        OrderTags[1].SetActive(true);
        Orders[1, 0] = 2;
        OrdersCombined[0] += 2;

        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());

        while (!tutorialOrderComplete)
        {
            if (RetryTutorialOrder)
            {
                yield return new WaitForSeconds(1f);
                ordersCombined[0] = 2;
                ordersSent[0] = 0;
                clearMeatFromScene();
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
                Alarm.Play();
                StartCoroutine(switchRed());
                RetryTutorialOrder = false;
            }
            yield return null;
        }
        yield return new WaitUntil(() => tutorialOrderComplete);
        tutorialOrderComplete = false;


        // ORDER 3
        yield return new WaitForSeconds(5f);
        dailyLog.Add($"- {(int)timer} seconds: Order 3 requested");
        computerScript.updateLog();
        orderContents[2] += $"";
        OrderTags[2].SetActive(true);
        Orders[2, 4] = 1;
        OrdersCombined[4] += 1;

        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(1f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));

        while (!tutorialOrderComplete)
        {
            if (RetryTutorialOrder)
            {
                yield return new WaitForSeconds(1f);
                OrdersCombined[4] = 1;
                ordersSent[4] = 0;
                clearMeatFromScene();
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
                Alarm.Play();
                StartCoroutine(switchRed());
                yield return new WaitForSeconds(1f);
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
                RetryTutorialOrder = false;
            }
            yield return null;
        }
        yield return new WaitUntil(() => tutorialOrderComplete);
        tutorialOrderComplete = false;


        // ORDER 4
        yield return new WaitForSeconds(5f);
        dailyLog.Add($"- {(int)timer} seconds: Order 4 requested");
        computerScript.updateLog();
        orderContents[3] += $"";
        OrderTags[3].SetActive(true);
        Orders[3, 3] = 1;
        Orders[3, 0] = 1;
        OrdersCombined[3] += 1;
        OrdersCombined[0] += 1;

        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(1f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));

        while (!tutorialOrderComplete)
        {
            if (RetryTutorialOrder)
            {
                yield return new WaitForSeconds(1f);
                ordersCombined[3] = 1;
                ordersCombined[0] = 1;
                ordersSent[3] = 0;
                ordersSent[0] = 0;
                clearMeatFromScene();
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
                Alarm.Play();
                StartCoroutine(switchRed());
                yield return new WaitForSeconds(1f);
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
                RetryTutorialOrder = false;
            }
            yield return null;
        }
        yield return new WaitUntil(() => tutorialOrderComplete);
        tutorialOrderComplete = false;

        finishDay(0);
    }

    public void Day1Text()
    {
        orderContents[0] = $"Order Contents: \n\n- 1 Uncut Piece Of Meat {(ordersComplete[0] ? 1 : ordersSent[1])}/1\n\nRequested By: Unlimited (Training)";
        orderContents[1] = $"Order Contents: \n\n- 2 Half Pieces Of Meat {(ordersComplete[1] ? 2 : ordersSent[0])}/2\n\nRequested By: Unlimited (Training)";
        orderContents[2] = $"Order Contents: \n\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[2] ? 1 : ordersSent[4])}/1\n\nRequested By: Unlimited (Training)";
        orderContents[3] = $"Order Contents: \n\n- 1 Bag Containing 1 Uncut Piece Of Meat {(ordersComplete[3] ? 1 : ordersSent[3])}/1\n- 1 Half Piece Of Meat {(ordersComplete[3] ? 1 : ordersSent[0])}/1\n\n Requested By: Unlimited (Training)";
        ordersComplete[4] = true;
        ordersComplete[5] = true;
        ordersComplete[6] = true;
        ordersComplete[7] = true;
        ordersComplete[8] = true;
        ordersComplete[9] = true;
        ordersComplete[10] = true;
        ordersComplete[11] = true;
        ordersComplete[12] = true;
        ordersComplete[13] = true;
        ordersComplete[14] = true;
        ordersComplete[15] = true;
    }

    IEnumerator Day2()
    {
        GameObject.Find("Player").GetComponent<Interact>().ExtUnclogBagger();

        failedPercentAvg = 0;

        if (skipTo > 0)
        {
            failedPercentAvg = PlayerPrefs.GetFloat("FailedPercentAvg", 0);
            GameObject.Find("Player").GetComponent<Interact>().StartCoroutine("LoadFromBench", false);
            yield return new WaitForSeconds(6f);
        }

        runnerInEffect = true;
        StartCoroutine("RandomizeRunner");
         
        TimeText.gameObject.SetActive(true);

        if (skipTo == 0)
        {
            conveyorAudioSources[0].Play();
            yield return new WaitForSeconds(conveyorAudioSources[0].clip.length - 0.1f);
        }
        conveyorAudioSources[1].Play();

        Day2Text();


        // ORDER 1
        OrderTags[0].SetActive(true);
        if (skipTo < 1)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 0;
            dailyLog.Add($"- {(int)timer} seconds: Order 1 requested");
            computerScript.updateLog();
            requestedByTimes[0] = 65 + (15 * difficulty) + (int)timer;
            Day2Text();
            Orders[0, 0] = 3;
            OrdersCombined[0] += 3;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(65 + (15 * difficulty), 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(63f + (15 * difficulty));
        }
        else
        {
            timer += 67f + (15f * difficulty);
            OrdersComplete[0] = true;
            if (ordersFailed[0])
            {
                OrderX[0].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(0, true);
            }
        }



        // ORDER 2
        OrderTags[1].SetActive(true);
        if (skipTo < 2)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 1;
            dailyLog.Add($"- {(int)timer} seconds: Order 2 requested");
            computerScript.updateLog();
            requestedByTimes[1] = 85 + (15 * difficulty) + (int)timer;
            Day2Text();
            Orders[1, 4] = 1;
            OrdersCombined[4] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(85 + (15 * difficulty), 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(83f + (15 * difficulty));
        }
        else
        {
            timer += 87f + (15f * difficulty);
            OrdersComplete[1] = true;

            if (ordersFailed[1])
            {
                OrderX[1].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(1, true);
            }

            if (skipTo == 1)
            {
                yield return new WaitForSeconds(7f);
            }
        }



        // ORDER 3
        OrderTags[2].SetActive(true);
        if (skipTo < 3)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 2;
            dailyLog.Add($"- {(int)timer} seconds: Order 3 requested");
            computerScript.updateLog();
            requestedByTimes[2] = 95 + (15 * difficulty) + (int)timer;
            Day2Text();
            Orders[2, 0] = 2;
            Orders[2, 5] = 1;
            OrdersCombined[0] += 2;
            OrdersCombined[5] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(95 + (15 * difficulty), 2, 2, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(87f + (15 * difficulty));
        }
        else
        {
            timer += 97f + (15f * difficulty);
            OrdersComplete[2] = true;

            if (ordersFailed[2])
            {
                OrderX[2].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(2, true);
            }

            if (skipTo == 2)
            {
                yield return new WaitForSeconds(7f);
            }
        }



        // ORDER 4
        OrderTags[3].SetActive(true);
        if (skipTo < 4)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 3;
            dailyLog.Add($"- {(int)timer} seconds: Order 4 requested");
            requestedByTimes[3] = 110 + (15 * difficulty) + (int)timer;
            Day2Text();
            Orders[3, 3] = 3;
            OrdersCombined[3] += 3;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(110 + (15 * difficulty), 3, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(105f + (15 * difficulty));
        }
        else
        {
            timer += 112f + (15f * difficulty);
            OrdersComplete[3] = true;

            if (ordersFailed[3])
            {
                OrderX[3].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(3, true);
            }

            if (skipTo == 3)
            {
                yield return new WaitForSeconds(7f);
            }
        }


        /*
                // ORDER 5
                dailyLog.Add($"- {(int)timer} seconds: Order 5 requested");
                computerScript.updateLog();
                requestedByTimes[4] = 110 + (15 * difficulty) + (int)timer;
                Day2Text();
                OrderTags[4].SetActive(true);
                Orders[4, 0] = 3;
                Orders[4, 4] = 2;
                OrdersCombined[0] += 3;
                OrdersCombined[4] += 2;
                yield return new WaitForSeconds(1f);

                StartCoroutine(OrderTimer(110 + (15 * difficulty), 4, 3, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0));
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
                Alarm.Play();
                StartCoroutine(switchRed());
                yield return new WaitForSeconds(3f);
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
                yield return new WaitForSeconds(3f);
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
                yield return new WaitForSeconds(3f);
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
                yield return new WaitForSeconds(3f);
                Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



                yield return new WaitForSeconds(99f + (15 * difficulty));*/



        /*// ORDER 6
        dailyLog.Add($"- {(int)timer} seconds: Order 6 requested");
        computerScript.updateLog();
        orderContents[5] += $"";
        OrderTags[5].SetActive(true);
        Orders[5, 0] = 2;
        Orders[5, 1] = 2;
        Orders[5, 2] = 1;
        OrdersCombined[0] += 2;
        OrdersCombined[1] += 2;
        OrdersCombined[2] += 1;
        yield return new WaitForSeconds(1f);

        StartCoroutine(OrderTimer(90 + (15 * difficulty), 5, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0));
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



        yield return new WaitForSeconds(78f + (15 * difficulty));*/

        for (int i = 0; i < 11; i++)
        {
            errors[i] += errorsMidOrder[i];
        }
        errorsMidOrder = new int[11];
        finishDay(1);
    }

    public void Day2Text()
    {
        /*orderContents[0] = $"Order Contents: \n\n- 3 Half Pieces Of Meat {(ordersComplete[0] ? 3 : ordersSent[0])}/3\n\n Requested By: {requestedByTimes[0]} Seconds";
        orderContents[1] = $"Order Contents: \n\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[1] ? 1 : ordersSent[4])}/1\n\n Requested By: {requestedByTimes[1]} Seconds";
        orderContents[2] = $"Order Contents: \n\n- 1 Bag Containing 2 Uncut Pieces Of Meat {(ordersComplete[2] ? 1 : ordersSent[5])}/1\n- 2 Half Pieces Of Meat {(ordersComplete[2] ? 2 : ordersSent[0])}/2\n\nRequested By: {requestedByTimes[2]} Seconds";
        orderContents[3] = $"Order Contents: \n\n- 3 Bags Containing 1 Uncut Piece Of Meat {(ordersComplete[3] ? 3 : ordersSent[3])}/3\n\n Requested By: {requestedByTimes[3]} Seconds";
        orderContents[4] = $"Order Contents: \n\n- 2 Bags Containing 1 And A Half Pieces Of Meat {(ordersComplete[4] ? 2 : ordersSent[4])}/2\n- 3 Half Pieces Of Meat {(ordersComplete[4] ? 3 : ordersSent[0])}/3\n\n Requested By: {requestedByTimes[4]} Seconds";*/
        orderContents[0] = $"Order Contents: \n\n- 3 Half Pieces Of Meat {(ordersComplete[0] ? 3 : ordersFailed[0] ? 0 : ordersSent[0])}/3\n\n Requested By: {requestedByTimes[0]} Seconds";
        orderContents[1] = $"Order Contents: \n\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[1] ? 1 : ordersFailed[1] ? 0 : ordersSent[4])}/1\n\n Requested By: {requestedByTimes[1]} Seconds";
        orderContents[2] = $"Order Contents: \n\n- 1 Bag Containing 2 Uncut Pieces Of Meat {(ordersComplete[2] ? 1 : ordersFailed[2] ? 0 : ordersSent[5])}/1\n- 2 Half Pieces Of Meat {(ordersComplete[2] ? 2 : ordersFailed[2] ? 0 : ordersSent[0])}/2\n\nRequested By: {requestedByTimes[2]} Seconds";
        orderContents[3] = $"Order Contents: \n\n- 3 Bags Containing 1 Uncut Piece Of Meat {(ordersComplete[3] ? 3 : ordersFailed[3] ? 0 : ordersSent[3])}/3\n\n Requested By: {requestedByTimes[3]} Seconds";
        //orderContents[4] = $"Order Contents: \n\n- 2 Bags Containing 1 And A Half Pieces Of Meat {(ordersComplete[4] ? 2 : ordersFailed[4] ? 0 : ordersSent[4])}/2\n- 3 Half Pieces Of Meat {(ordersComplete[4] ? 3 : ordersFailed[4] ? 0 : ordersSent[0])}/3\n\n Requested By: {requestedByTimes[4]} Seconds";
        ordersComplete[4] = true;
        ordersComplete[5] = true;
        ordersComplete[6] = true;
        ordersComplete[7] = true;
        ordersComplete[8] = true;
        ordersComplete[9] = true;
        ordersComplete[10] = true;
        ordersComplete[11] = true;
        ordersComplete[12] = true;
        ordersComplete[13] = true;
        ordersComplete[14] = true;
        ordersComplete[15] = true;
    }

    IEnumerator Day3()
    {
        GameObject.Find("Player").GetComponent<Interact>().ExtUnclogBagger();

        failedPercentAvg = 0;

        runnerInEffect = true;
        StartCoroutine("RandomizeRunner");

        TimeText.gameObject.SetActive(true);

        if (skipTo == 0)
        {
            conveyorAudioSources[0].Play();
            yield return new WaitForSeconds(conveyorAudioSources[0].clip.length - 0.1f);
        }
        conveyorAudioSources[1].Play();

        Day3Text();

        if (skipTo > 0)
        {
            GameObject.Find("Player").GetComponent<Interact>().StartCoroutine("LoadFromBench", false);
            yield return new WaitForSeconds(10f);
            yield return new WaitForSeconds(7f);
        }


        yield return new WaitForSeconds(2f);

        // ORDER 1
        OrderTags[0].SetActive(true);
        if (skipTo < 1)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 0;
            dailyLog.Add($"- {(int)timer} seconds: Order 1 requested");
            computerScript.updateLog();
            requestedByTimes[0] = 60 + (15 * difficulty) + (int)timer;
            Day3Text();
            Orders[0, 0] = 1;
            Orders[0, 1] = 1;
            OrdersCombined[0] += 1;
            OrdersCombined[1] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(60 + (15 * difficulty), 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(55f + (15 * difficulty));
        }
        else
        {
            timer += 62f + (15f * difficulty);
            OrdersComplete[0] = true;

            if (ordersFailed[0])
            {
                OrderX[0].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(0, true);
            }
        }



        // ORDER 2
        OrderTags[1].SetActive(true);
        if (skipTo < 2)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 1;
            dailyLog.Add($"- {(int)timer} seconds: Order 2 requested");
            computerScript.updateLog();
            requestedByTimes[1] = 110 + (15 * difficulty) + (int)timer;
            Day3Text();
            Orders[1, 6] = 1;
            Orders[1, 7] = 1;
            OrdersCombined[6] += 1;
            OrdersCombined[7] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(110 + (15 * difficulty), 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            spiderInEffect = true;
            StartCoroutine("SpiderMonster");



            yield return new WaitForSeconds(105f + (15 * difficulty));
        }
        else
        {
            timer += 112f + (15f * difficulty);
            OrdersComplete[1] = true;

            if (ordersFailed[1])
            {
                OrderX[1].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(1, true);
            }

            StartCoroutine("SpiderMonster");
        }



        // ORDER 3
        OrderTags[2].SetActive(true);
        if (skipTo < 3)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 2;
            dailyLog.Add($"- {(int)timer} seconds: Order 3 requested");
            computerScript.updateLog();
            requestedByTimes[2] = 120 + (15 * difficulty) + (int)timer;
            Day3Text();
            Orders[2, 1] = 1;
            Orders[2, 3] = 1;
            Orders[2, 8] = 1;
            OrdersCombined[1] += 1;
            OrdersCombined[3] += 1;
            OrdersCombined[8] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(120 + (15 * difficulty), 2, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(112f + (15 * difficulty));
        }
        else
        {
            timer += 122f + (15f * difficulty);
            OrdersComplete[2] = true;

            if (ordersFailed[2])
            {
                OrderX[2].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(2, true);
            }
        }



        // ORDER 4
        OrderTags[3].SetActive(true);
        if (skipTo < 4)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 3;
            dailyLog.Add($"- {(int)timer} seconds: Order 4 requested");
            computerScript.updateLog();
            requestedByTimes[3] = 120 + (15 * difficulty) + (int)timer;
            Day3Text();
            Orders[3, 0] = 3;
            Orders[3, 2] = 1;
            Orders[3, 9] = 2;
            OrdersCombined[0] += 3;
            OrdersCombined[2] += 1;
            OrdersCombined[9] += 2;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(120 + (15 * difficulty), 3, 3, 0, 1, 0, 0, 0, 0, 0, 0, 2, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(106f + (15 * difficulty));
        }
        else
        {
            timer += 122f + (15f * difficulty);
            OrdersComplete[3] = true;

            if (ordersFailed[3])
            {
                OrderX[3].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(3, true);
            }
        }



        // ORDER 5
        OrderTags[4].SetActive(true);
        if (skipTo < 5)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 4;
            dailyLog.Add($"- {(int)timer} seconds: Order 5 requested");
            computerScript.updateLog();
            requestedByTimes[4] = 120 + (15 * difficulty) + (int)timer;
            Day3Text();
            Orders[4, 6] = 1;
            Orders[4, 7] = 1;
            Orders[4, 8] = 1;
            Orders[4, 9] = 1;
            OrdersCombined[6] += 1;
            OrdersCombined[7] += 1;
            OrdersCombined[8] += 1;
            OrdersCombined[9] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(120 + (15 * difficulty), 4, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(106f + (15 * difficulty));
        }
        else
        {
            timer += 119f + (15f * difficulty);
            OrdersComplete[4] = true;

            if (ordersFailed[4])
            {
                OrderX[4].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(4, true);
            }
        }



        // ORDER 6
        OrderTags[5].SetActive(true);
        if (skipTo < 6)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 5;
            dailyLog.Add($"- {(int)timer} seconds: Order 6 requested");
            computerScript.updateLog();
            requestedByTimes[5] = 100 + (15 * difficulty) + (int)timer;
            Day3Text();
            Orders[5, 0] = 2;
            Orders[5, 1] = 1;
            Orders[5, 2] = 1;
            OrdersCombined[0] += 2;
            OrdersCombined[1] += 1;
            OrdersCombined[2] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(100 + (15 * difficulty), 5, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(89f + (15 * difficulty));
        }
        else
        {
            timer += 99f + (15f * difficulty);
            OrdersComplete[5] = true;

            if (ordersFailed[5])
            {
                OrderX[5].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(5, true);
            }
        }



        /*// ORDER 7
        dailyLog.Add($"- {(int)timer} seconds: Order 7 requested");
        computerScript.updateLog();
        requestedByTimes[6] = 140 + (15 * difficulty) + (int)timer;
        Day3Text();
        OrderTags[6].SetActive(true);
        Orders[6, 4] = 1;
        Orders[6, 8] = 2;
        Orders[6, 9] = 3;
        OrdersCombined[4] += 1;
        OrdersCombined[8] += 2;
        OrdersCombined[9] += 3;
        yield return new WaitForSeconds(1f);

        StartCoroutine(OrderTimer(140 + (15 * difficulty), 6, 0, 0, 0, 0, 1, 0, 0, 0, 2, 3, 0));
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



        yield return new WaitForSeconds(123f + (15 * difficulty));*/



        // ORDER 8
        for (int i = 0; i < 11; i++)
        {
            errors[i] += errorsMidOrder[i];
        }
        errorsMidOrder = new int[11];
        curOrder = 6;
        OrderTags[6].SetActive(true);
        dailyLog.Add($"- {(int)timer} seconds: Order 8 requested");
        computerScript.updateLog();
        requestedByTimes[6] = 45 + (int)timer;
        Day3Text();
        yield return new WaitForSeconds(1f);

        Instantiate(Arm, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(3f);
        Instantiate(Arm, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



        yield return new WaitForSeconds(32f);
        ordersComplete[6] = true;
        checkmarkOrder(6, true);
        yield return new WaitForSeconds(10f);

        for (int i = 0; i < 11; i++)
        {
            errors[i] += errorsMidOrder[i];
        }
        errorsMidOrder = new int[11];
        finishDay(2);
    }

    public void Day3Text()
    {
        /*orderContents[0] = $"Order Contents: \n\n- 1 Half Piece Of Meat {(ordersComplete[0] ? 1 : ordersSent[0])}/1\n- 1 Uncut Piece Of Meat {(ordersComplete[0] ? 1 : ordersSent[1])}/1\n\n Requested By: {requestedByTimes[0]} Seconds";
        orderContents[1] = $"Order Contents: \n\n- 1 Well Cooked Half Piece Of Meat {(ordersComplete[1] ? 1 : ordersSent[7])}/1\n- 1 Lightly Cooked Half Piece Of Meat {(ordersComplete[1] ? 1 : ordersSent[6])}/1\n\n Requested By: {requestedByTimes[1]} Seconds";
        orderContents[2] = $"Order Contents: \n\n- 1 Bag Containing 1 Uncut Piece Of Meat {(ordersComplete[2] ? 1 : ordersSent[3])}/1\n- 1 Uncut Piece Of Meat {(ordersComplete[2] ? 1 : ordersSent[1])}/1\n- 1 Lightly Cooked Uncut Piece Of Meat {(ordersComplete[2] ? 1 : ordersSent[8])}/1\n\nRequested By: {requestedByTimes[2]} Seconds";
        orderContents[3] = $"Order Contents: \n\n- 2 Well Cooked Uncut Pieces Of Meat {(ordersComplete[3] ? 2 : ordersSent[9])}/2\n- 3 Half Pieces Of Meat {(ordersComplete[3] ? 3 : ordersSent[0])}/3\n- 1 Bag Containing A Half Piece Of Meat {(ordersComplete[3] ? 1 : ordersSent[2])}/1\n\n Requested By: {requestedByTimes[3]} Seconds";
        orderContents[4] = $"Order Contents: \n\n- 1 Well Cooked Uncut Piece Of Meat {(ordersComplete[4] ? 1 : ordersSent[9])}/1\n- 1 Lightly Cooked Uncut Piece Of Meat {(ordersComplete[4] ? 1 : ordersSent[8])}/1\n- 1 Well Cooked Half Piece Of Meat {(ordersComplete[4] ? 1 : ordersSent[7])}/1\n- 1 Lightly Cooked Half Piece Of Meat {(ordersComplete[4] ? 1 : ordersSent[6])}/1\n\n Requested By: {requestedByTimes[4]} Seconds";
        orderContents[5] = $"Order Contents: \n\n- 2 Half Pieces Of Meat {(ordersComplete[5] ? 2 : ordersSent[0])}/2\n- 1 Uncut Piece Of Meat {(ordersComplete[5] ? 1 : ordersSent[1])}/1\n- 1 Bag Containing A Half Piece Of Meat {(ordersComplete[5] ? 1 : ordersSent[2])}/1\n\nRequested By: {requestedByTimes[5]} Seconds";
        orderContents[6] = $"Order Contents: \n\n- 2 Lightly Cooked Uncut Pieces Of Meat {(ordersComplete[6] ? 2 : ordersSent[8])}/2\n- 3 Well Cooked Uncut Pieces Of Meat {(ordersComplete[6] ? 3 : ordersSent[9])}/3\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[6] ? 1 : ordersSent[4])}/1\n\nRequested By: {requestedByTimes[6]} Seconds";
        orderContents[7] = $"Order Contents: \n\n- 2 Arms\n\nRequested By: {requestedByTimes[7]} Seconds";*/
        orderContents[0] = $"Order Contents: \n\n- 1 Half Piece Of Meat {(ordersComplete[0] ? 1 : ordersFailed[0] ? 0 : ordersSent[0])}/1\n- 1 Uncut Piece Of Meat {(ordersComplete[0] ? 1 : ordersFailed[0] ? 0 : ordersSent[1])}/1\n\n Requested By: {requestedByTimes[0]} Seconds";
        orderContents[1] = $"Order Contents: \n\n- 1 Well Cooked Half Piece Of Meat {(ordersComplete[1] ? 1 : ordersFailed[1] ? 0 : ordersSent[7])}/1\n- 1 Lightly Cooked Half Piece Of Meat {(ordersComplete[1] ? 1 : ordersFailed[1] ? 0 : ordersSent[6])}/1\n\n Requested By: {requestedByTimes[1]} Seconds";
        orderContents[2] = $"Order Contents: \n\n- 1 Bag Containing 1 Uncut Piece Of Meat {(ordersComplete[2] ? 1 : ordersFailed[2] ? 0 : ordersSent[3])}/1\n- 1 Uncut Piece Of Meat {(ordersComplete[2] ? 1 : ordersFailed[2] ? 0 : ordersSent[1])}/1\n- 1 Lightly Cooked Uncut Piece Of Meat {(ordersComplete[2] ? 1 : ordersFailed[2] ? 0 : ordersSent[8])}/1\n\nRequested By: {requestedByTimes[2]} Seconds";
        orderContents[3] = $"Order Contents: \n\n- 2 Well Cooked Uncut Pieces Of Meat {(ordersComplete[3] ? 2 : ordersFailed[3] ? 0 : ordersSent[9])}/2\n- 3 Half Pieces Of Meat {(ordersComplete[3] ? 3 : ordersFailed[3] ? 0 : ordersSent[0])}/3\n- 1 Bag Containing A Half Piece Of Meat {(ordersComplete[3] ? 1 : ordersFailed[3] ? 0 : ordersSent[2])}/1\n\n Requested By: {requestedByTimes[3]} Seconds";
        orderContents[4] = $"Order Contents: \n\n- 1 Well Cooked Uncut Piece Of Meat {(ordersComplete[4] ? 1 : ordersFailed[4] ? 0 : ordersSent[9])}/1\n- 1 Lightly Cooked Uncut Piece Of Meat {(ordersComplete[4] ? 1 : ordersFailed[4] ? 0 : ordersSent[8])}/1\n- 1 Well Cooked Half Piece Of Meat {(ordersComplete[4] ? 1 : ordersFailed[4] ? 0 : ordersSent[7])}/1\n- 1 Lightly Cooked Half Piece Of Meat {(ordersComplete[4] ? 1 : ordersFailed[4] ? 0 : ordersSent[6])}/1\n\n Requested By: {requestedByTimes[4]} Seconds";
        orderContents[5] = $"Order Contents: \n\n- 2 Half Pieces Of Meat {(ordersComplete[5] ? 2 : ordersFailed[5] ? 0 : ordersSent[0])}/2\n- 1 Uncut Piece Of Meat {(ordersComplete[5] ? 1 : ordersFailed[5] ? 0 : ordersSent[1])}/1\n- 1 Bag Containing A Half Piece Of Meat {(ordersComplete[5] ? 1 : ordersFailed[5] ? 0 : ordersSent[2])}/1\n\nRequested By: {requestedByTimes[5]} Seconds";
        //orderContents[6] = $"Order Contents: \n\n- 2 Lightly Cooked Uncut Pieces Of Meat {(ordersComplete[6] ? 2 : ordersFailed[6] ? 0 : ordersSent[8])}/2\n- 3 Well Cooked Uncut Pieces Of Meat {(ordersComplete[6] ? 3 : ordersFailed[6] ? 0 : ordersSent[9])}/3\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[6] ? 1 : ordersFailed[6] ? 0 : ordersSent[4])}/1\n\nRequested By: {requestedByTimes[6]} Seconds";
        orderContents[6] = $"Order Contents: \n\n- 2 Arms\n\nRequested By: {requestedByTimes[6]} Seconds";
        ordersComplete[7] = true;
        ordersComplete[8] = true;
        ordersComplete[9] = true;
        ordersComplete[10] = true;
        ordersComplete[11] = true;
        ordersComplete[12] = true;
        ordersComplete[13] = true;
        ordersComplete[14] = true;
        ordersComplete[15] = true;
    }

    IEnumerator Day4()
    {
        GameObject.Find("Player").GetComponent<Interact>().ExtUnclogBagger();

        failedPercentAvg = 0;

        runnerInEffect = true;
        StartCoroutine("RandomizeRunner");

        TimeText.gameObject.SetActive(true);

        if (skipTo == 0)
        {
            conveyorAudioSources[0].Play();
            yield return new WaitForSeconds(conveyorAudioSources[0].clip.length - 0.1f);
        }
        conveyorAudioSources[1].Play();

        Day4Text();

        if (skipTo > 0)
        {
            GameObject.Find("Player").GetComponent<Interact>().StartCoroutine("LoadFromBench", false);
            yield return new WaitForSeconds(10f);
            yield return new WaitForSeconds(7f);
        }


        yield return new WaitForSeconds(2f);

        // ORDER 1
        OrderTags[0].SetActive(true);
        if (skipTo < 1)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 0;
            yield return new WaitForSeconds(2f);
            dailyLog.Add($"- {(int)timer} seconds: Order 1 requested");
            computerScript.updateLog();
            requestedByTimes[0] = 50 + (15 * difficulty) + (int)timer;
            Day4Text();
            Orders[0, 0] = 1;
            Orders[0, 1] = 2;
            OrdersCombined[0] += 1;
            OrdersCombined[1] += 2;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(50 + (15 * difficulty), 0, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(39f + (15 * difficulty));
        }
        else
        {
            timer += 52f + (15f * difficulty);
            OrdersComplete[0] = true;

            if (ordersFailed[0])
            {
                OrderX[0].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(0, true);
            }
        }



        // ORDER 2
        OrderTags[1].SetActive(true);
        if (skipTo < 2)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 1;
            dailyLog.Add($"- {(int)timer} seconds: Order 2 requested");
            computerScript.updateLog();
            requestedByTimes[1] = 90 + (15 * difficulty) + (int)timer;
            Day4Text();
            Orders[1, 0] = 1;
            Orders[1, 8] = 2;
            OrdersCombined[0] += 1;
            OrdersCombined[8] += 2;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(90 + (15 * difficulty), 1, 1, 0, 1, 0, 0, 0, 0, 0, 2, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            spiderInEffect = true;
            wormInEffect = true;
            StartCoroutine("SpiderMonster");
            StartCoroutine("WormMonster");



            yield return new WaitForSeconds(82f + (15 * difficulty));
        }
        else
        {
            timer += 92f + (15f * difficulty);
            OrdersComplete[1] = true;

            if (ordersFailed[1])
            {
                OrderX[1].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(1, true);
            }

            StartCoroutine("SpiderMonster");
            StartCoroutine("WormMonster");
        }



        // ORDER 3
        OrderTags[2].SetActive(true);
        if (skipTo < 3)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 2;
            dailyLog.Add($"- {(int)timer} seconds: Order 3 requested");
            computerScript.updateLog();
            requestedByTimes[2] = 110 + (15 * difficulty) + (int)timer;
            Day4Text();
            Orders[2, 2] = 1;
            Orders[2, 4] = 1;
            OrdersCombined[2] += 1;
            OrdersCombined[4] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(110 + (15 * difficulty), 2, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(102f + (15 * difficulty));
        }
        else
        {
            timer += 112f + (15f * difficulty);
            OrdersComplete[2] = true;

            if (ordersFailed[2])
            {
                OrderX[2].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(2, true);
            }
        }



        // ORDER 4
        OrderTags[3].SetActive(true);
        if (skipTo < 4)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 3;
            dailyLog.Add($"- {(int)timer} seconds: Order 4 requested");
            computerScript.updateLog();
            requestedByTimes[3] = 130 + (15 * difficulty) + (int)timer;
            Day4Text();
            Orders[3, 1] = 1;
            Orders[3, 2] = 2;
            Orders[3, 7] = 1;
            OrdersCombined[1] += 1;
            OrdersCombined[2] += 2;
            OrdersCombined[7] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(130 + (15 * difficulty), 3, 0, 1, 2, 0, 0, 0, 0, 1, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(122f + (15 * difficulty));
        }
        else
        {
            timer += 132f + (15f * difficulty);
            OrdersComplete[3] = true;

            if (ordersFailed[3])
            {
                OrderX[3].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(3, true);
            }
        }



        // ORDER 5
        OrderTags[4].SetActive(true);
        if (skipTo < 5)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 4;
            dailyLog.Add($"- {(int)timer} seconds: Order 5 requested");
            computerScript.updateLog();
            requestedByTimes[4] = 135 + (15 * difficulty) + (int)timer;
            Day4Text();
            Orders[4, 0] = 3;
            Orders[4, 4] = 1;
            Orders[4, 9] = 1;
            OrdersCombined[0] += 3;
            OrdersCombined[4] += 1;
            OrdersCombined[9] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(135 + (15 * difficulty), 4, 3, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(124f + (15 * difficulty));
        }
        else
        {
            timer += 137f + (15f * difficulty);
            OrdersComplete[4] = true;

            if (ordersFailed[4])
            {
                OrderX[4].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(4, true);
            }
        }



        // ORDER 6
        OrderTags[5].SetActive(true);
        if (skipTo < 6)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 5;
            dailyLog.Add($"- {(int)timer} seconds: Order 6 requested");
            computerScript.updateLog();
            requestedByTimes[5] = 135 + (15 * difficulty) + (int)timer;
            Day4Text();
            Orders[5, 1] = 2;
            Orders[5, 5] = 1;
            Orders[5, 6] = 2;
            OrdersCombined[1] += 2;
            OrdersCombined[5] += 1;
            OrdersCombined[6] += 2;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(135 + (15 * difficulty), 5, 0, 2, 0, 1, 0, 1, 2, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(118f + (15 * difficulty));
        }
        else
        {
            timer += 137f + (15f * difficulty);
            OrdersComplete[5] = true;

            if (ordersFailed[5])
            {
                OrderX[5].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(5, true);
            }
        }



        /*// ORDER 7
        dailyLog.Add($"- {(int)timer} seconds: Order 7 requested");
        computerScript.updateLog();
        orderContents[6] += $"{120 + (15 * difficulty) + (int)timer} Seconds";
        OrderTags[6].SetActive(true);
        Orders[6, 2] = 1;
        Orders[6, 3] = 3;
        Orders[6, 6] = 1;
        OrdersCombined[2] += 1;
        OrdersCombined[3] += 3;
        OrdersCombined[6] += 1;
        yield return new WaitForSeconds(1f);

        StartCoroutine(OrderTimer(120 + (15 * difficulty), 6, 0, 0, 1, 3, 0, 0, 1, 0, 0, 0, 0));
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



        yield return new WaitForSeconds(95f + (15 * difficulty));*/



        // ORDER 8
        OrderTags[6].SetActive(true);
        if (skipTo < 7)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 6;
            dailyLog.Add($"- {(int)timer} seconds: Order 7 requested");
            computerScript.updateLog();
            requestedByTimes[6] = 85 + (15 * difficulty) + (int)timer;
            Day4Text();
            Orders[6, 0] = 4;
            Orders[6, 1] = 3;
            OrdersCombined[0] += 4;
            OrdersCombined[1] += 3;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(85 + (15 * difficulty), 6, 4, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(71f + (15 * difficulty));
        }
        else
        {
            timer += 87f + (15f * difficulty);
            OrdersComplete[6] = true;

            if (ordersFailed[6])
            {
                OrderX[6].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(6, true);
            }
        }



        // ORDER 9
        OrderTags[7].SetActive(true);
        if (skipTo < 8)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 7;
            dailyLog.Add($"- {(int)timer} seconds: Order 8 requested");
            computerScript.updateLog();
            requestedByTimes[7] = 150 + (15 * difficulty) + (int)timer;
            Day4Text();
            Orders[7, 6] = 2;
            Orders[7, 7] = 2;
            Orders[7, 9] = 1;
            OrdersCombined[6] += 2;
            OrdersCombined[7] += 2;
            OrdersCombined[9] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(150 + (15 * difficulty), 7, 0, 0, 0, 0, 0, 0, 2, 2, 0, 1, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));




            yield return new WaitForSeconds(142f + (15 * difficulty));
        }
        else
        {
            timer += 152f + (15f * difficulty);
            OrdersComplete[7] = true;

            if (ordersFailed[7])
            {
                OrderX[7].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(7, true);
            }
        }



        /*// ORDER 10
        dailyLog.Add($"- {(int)timer} seconds: Order 9 requested");
        computerScript.updateLog();
        requestedByTimes[8] = 140 + (15 * difficulty) + (int)timer;
        Day4Text();
        OrderTags[8].SetActive(true);
        Orders[8, 0] = 8;
        Orders[8, 1] = 1;
        Orders[8, 5] = 2;
        OrdersCombined[0] += 8;
        OrdersCombined[1] += 1;
        OrdersCombined[5] += 2;
        yield return new WaitForSeconds(1f);

        StartCoroutine(OrderTimer(140 + (15 * difficulty), 8, 8, 1, 0, 0, 0, 2, 0, 0, 0, 0, 0));
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



        yield return new WaitForSeconds(114f + (15 * difficulty));*/



        // ORDER 11
        for (int i = 0; i < 11; i++)
        {
            errors[i] += errorsMidOrder[i];
        }
        errorsMidOrder = new int[11];
        curOrder = 8;
        OrderTags[8].SetActive(true);
        OrdersLeftArrow.SetActive(true);
        OrdersRightArrow.SetActive(true);
        dailyLog.Add($"- {(int)timer} seconds: Order 10 requested");
        computerScript.updateLog();
        requestedByTimes[8] = 45  + (int)timer;
        Day4Text();
        yield return new WaitForSeconds(1f);

        Instantiate(Liver, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(2f);
        Instantiate(Arm, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(2f);
        Instantiate(Arm, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(2f);
        Instantiate(Arm, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(2f);
        Instantiate(Arm, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(2f);
        Instantiate(Arm, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));

        yield return new WaitForSeconds(34f);
        ordersComplete[8] = true;
        checkmarkOrder(8, true);
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 11; i++)
        {
            errors[i] += errorsMidOrder[i];
        }
        errorsMidOrder = new int[11];
        finishDay(3);
    }

    public void Day4Text()
    {
        /*orderContents[0] = $"Order Contents: \n\n- 1 Half Piece Of Meat {(ordersComplete[0] ? 1 : ordersFailed[0] ? 0 : ordersSent[0])}/1\n- 2 Uncut Pieces Of Meat {(ordersComplete[0] ? 2 : ordersSent[1])}/2\n\n Requested By: {requestedByTimes[0]} Seconds";
        orderContents[1] = $"Order Contents: \n\n- 1 Half Piece Of Meat {(ordersComplete[1] ? 1 : ordersSent[0])}/1\n- 2 Lightly Cooked Uncut Pieces Of Meat {(ordersComplete[1] ? 2 : ordersSent[8])}/2\n- 1 Bag Containing A Half Piece Of Meat {(ordersComplete[1] ? 1 : ordersSent[2])}/1\n\n Requested By: {requestedByTimes[1]} Seconds";
        orderContents[2] = $"Order Contents: \n\n- 1 Bag Containing 1 Uncut Piece Of Meat {(ordersComplete[2] ? 1 : ordersSent[3])}/1\n- 1 Bag Containing A Half Piece Of Meat {(ordersComplete[2] ? 1 : ordersSent[2])}/1\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[2] ? 1 : ordersSent[4])}/1\n\nRequested By: {requestedByTimes[2]} Seconds";
        orderContents[3] = $"Order Contents: \n\n- 1 Uncut Piece Of Meat {(ordersComplete[3] ? 1 : ordersSent[1])}/1\n- 2 Bags Containing A Half Piece Of Meat {(ordersComplete[3] ? 2 : ordersSent[2])}/2\n- 1 Well Cooked Half Piece Of Meat {(ordersComplete[3] ? 1 : ordersSent[7])}/1\n\n Requested By: {requestedByTimes[3]} Seconds";
        orderContents[4] = $"Order Contents: \n\n- 3 Half Pieces Of Meat {(ordersComplete[4] ? 3 : ordersSent[0])}/3\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[4] ? 1 : ordersSent[4])}/1\n- 1 Well Cooked Uncut Piece Of Meat {(ordersComplete[4] ? 1 : ordersSent[9])}/1\n\n Requested By: {requestedByTimes[4]} Seconds";
        orderContents[5] = $"Order Contents: \n\n- 2 Uncut Pieces Of Meat {(ordersComplete[5] ? 2 : ordersFailed[5] ? 0 : ordersSent[1])}/2\n- 1 Bag Containing 1 Uncut Piece Of Meat {(ordersComplete[5] ? 1 : ordersSent[3])}/1\n- 1 Bag Containing 2 Uncut Pieces Of Meat {(ordersComplete[5] ? 1 : ordersSent[5])}/1\n- 2 Lightly Cooked Half Pieces Of Meat {(ordersComplete[5] ? 2 : ordersSent[6])}/2\n\nRequested By: {requestedByTimes[5]} Seconds";
        //orderContents[6] = $"Order Contents: \n\n- 1 Bag Containing A Half Piece Of Meat\n- 3 Bags Containing 1 Uncut Piece Of Meat\n- 1 Lightly Cooked Half Piece Of Meat\n\nRequested By: ";
        orderContents[6] = $"Order Contents: \n\n- 3 Uncut Pieces Of Meat {(ordersComplete[6] ? 3 : ordersSent[1])}/3\n- 4 Half Pieces Of Meat {(ordersComplete[6] ? 4 : ordersSent[0])}/4\n\nRequested By: {requestedByTimes[6]} Seconds";
        orderContents[7] = $"Order Contents: \n\n- 2 Lightly Cooked Half Pieces Of Meat {(ordersComplete[7] ? 2 : ordersSent[6])}/2\n- 2 Well Cooked Half Pieces Of Meat {(ordersComplete[7] ? 2 : ordersSent[7])}/2\n- 1 Well Cooked Uncut Piece Of Meat {(ordersComplete[7] ? 1 : ordersSent[9])}/1\n\nRequested By: {requestedByTimes[7]} Seconds";
        orderContents[8] = $"Order Contents: \n\n- 8 Half Pieces Of Meat {(ordersComplete[8] ? 8 : ordersSent[0])}/8\n- 1 Uncut Piece Of Meat {(ordersComplete[8] ? 1 : ordersSent[1])}/1\n- 2 Bags Containing 2 Uncut Pieces Of Meat {(ordersComplete[8] ? 2 : ordersSent[5])}/2\n\nRequested By: {requestedByTimes[8]} Seconds";
        orderContents[9] = $"Order Contents: \n\n- 5 Arms\n- 1 Liver\n\nRequested By: {requestedByTimes[9]} Seconds";*/
        orderContents[0] = $"Order Contents: \n\n- 1 Half Piece Of Meat {(ordersComplete[0] ? 1 : ordersFailed[0] ? 0 : ordersSent[0])}/1\n- 2 Uncut Pieces Of Meat {(ordersComplete[0] ? 2 : ordersFailed[0] ? 0 : ordersSent[1])}/2\n\n Requested By: {requestedByTimes[0]} Seconds";
        orderContents[1] = $"Order Contents: \n\n- 1 Half Piece Of Meat {(ordersComplete[1] ? 1 : ordersFailed[1] ? 0 : ordersSent[0])}/1\n- 2 Lightly Cooked Uncut Pieces Of Meat {(ordersComplete[1] ? 2 : ordersFailed[1] ? 0 : ordersSent[8])}/2\n\n Requested By: {requestedByTimes[1]} Seconds";
        orderContents[2] = $"Order Contents: \n\n- 1 Bag Containing A Half Piece Of Meat {(ordersComplete[2] ? 1 : ordersFailed[2] ? 0 : ordersSent[2])}/1\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[2] ? 1 : ordersFailed[2] ? 0 : ordersSent[4])}/1\n\nRequested By: {requestedByTimes[2]} Seconds";
        orderContents[3] = $"Order Contents: \n\n- 1 Uncut Piece Of Meat {(ordersComplete[3] ? 1 : ordersFailed[3] ? 0 : ordersSent[1])}/1\n- 2 Bags Containing A Half Piece Of Meat {(ordersComplete[3] ? 2 : ordersFailed[3] ? 0 : ordersSent[2])}/2\n- 1 Well Cooked Half Piece Of Meat {(ordersComplete[3] ? 1 : ordersFailed[3] ? 0 : ordersSent[7])}/1\n\n Requested By: {requestedByTimes[3]} Seconds";
        orderContents[4] = $"Order Contents: \n\n- 3 Half Pieces Of Meat {(ordersComplete[4] ? 3 : ordersFailed[4] ? 0 : ordersSent[0])}/3\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[4] ? 1 : ordersFailed[4] ? 0 : ordersSent[4])}/1\n- 1 Well Cooked Uncut Piece Of Meat {(ordersComplete[4] ? 1 : ordersFailed[4] ? 0 : ordersSent[9])}/1\n\n Requested By: {requestedByTimes[4]} Seconds";
        orderContents[5] = $"Order Contents: \n\n- 2 Uncut Pieces Of Meat {(ordersComplete[5] ? 2 : ordersFailed[5] ? 0 : ordersSent[1])}/2\n- 1 Bag Containing 2 Uncut Pieces Of Meat {(ordersComplete[5] ? 1 : ordersFailed[5] ? 0 : ordersSent[5])}/1\n- 2 Lightly Cooked Half Pieces Of Meat {(ordersComplete[5] ? 2 : ordersFailed[5] ? 0 : ordersSent[6])}/2\n\nRequested By: {requestedByTimes[5]} Seconds";
        orderContents[6] = $"Order Contents: \n\n- 3 Uncut Pieces Of Meat {(ordersComplete[6] ? 3 : ordersFailed[6] ? 0 : ordersSent[1])}/3\n- 4 Half Pieces Of Meat {(ordersComplete[6] ? 4 : ordersFailed[6] ? 0 : ordersSent[0])}/4\n\nRequested By: {requestedByTimes[6]} Seconds";
        orderContents[7] = $"Order Contents: \n\n- 2 Lightly Cooked Half Pieces Of Meat {(ordersComplete[7] ? 2 : ordersFailed[7] ? 0 : ordersSent[6])}/2\n- 2 Well Cooked Half Pieces Of Meat {(ordersComplete[7] ? 2 : ordersFailed[7] ? 0 : ordersSent[7])}/2\n- 1 Well Cooked Uncut Piece Of Meat {(ordersComplete[7] ? 1 : ordersFailed[7] ? 0 : ordersSent[9])}/1\n\nRequested By: {requestedByTimes[7]} Seconds";
        //orderContents[8] = $"Order Contents: \n\n- 8 Half Pieces Of Meat {(ordersComplete[8] ? 8 : ordersFailed[8] ? 0 : ordersSent[0])}/8\n- 1 Uncut Piece Of Meat {(ordersComplete[8] ? 1 : ordersFailed[8] ? 0 : ordersSent[1])}/1\n- 2 Bags Containing 2 Uncut Pieces Of Meat {(ordersComplete[8] ? 2 : ordersFailed[8] ? 0 : ordersSent[5])}/2\n\nRequested By: {requestedByTimes[8]} Seconds";
        orderContents[8] = $"Order Contents: \n\n- 5 Arms\n- 1 Liver\n\nRequested By: {requestedByTimes[8]} Seconds";
        ordersComplete[9] = true;
        ordersComplete[10] = true;
        ordersComplete[11] = true;
        ordersComplete[12] = true;
        ordersComplete[13] = true;
        ordersComplete[14] = true;
        ordersComplete[15] = true;
    }

    IEnumerator Day5()
    {
        GameObject.Find("Player").GetComponent<Interact>().ExtUnclogBagger();

        failedPercentAvg = 0;

        runnerInEffect = true;
        StartCoroutine("RandomizeRunner");

        TimeText.gameObject.SetActive(true);

        if (skipTo == 0)
        {
            conveyorAudioSources[0].Play();
            yield return new WaitForSeconds(conveyorAudioSources[0].clip.length - 0.1f);
        }
        conveyorAudioSources[1].Play();

        Day5Text();

        if (skipTo > 0)
        {
            GameObject.Find("Player").GetComponent<Interact>().StartCoroutine("LoadFromBench", false);
            yield return new WaitForSeconds(10f);
            yield return new WaitForSeconds(7f);
        }


        yield return new WaitForSeconds(2f);

        // ORDER 1
        OrderTags[0].SetActive(true);
        if (skipTo < 1)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 0;
            yield return new WaitForSeconds(2f);
            dailyLog.Add($"- {(int)timer} seconds: Order 1 requested");
            computerScript.updateLog();
            requestedByTimes[0] = 30 + (15 * difficulty) + (int)timer;
            Day5Text();
            Orders[0, 1] = 2;
            OrdersCombined[1] += 2;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(30 + (15 * difficulty), 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(22f + (15 * difficulty));
        }
        else
        {
            timer += 32f + (15f * difficulty);
            OrdersComplete[0] = true;

            if (ordersFailed[0])
            {
                OrderX[0].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(0, true);
            }
        }



        // ORDER 2
        OrderTags[1].SetActive(true);
        if (skipTo < 2)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 1;
            dailyLog.Add($"- {(int)timer} seconds: Order 2 requested");
            computerScript.updateLog();
            requestedByTimes[1] = 40 + (15 * difficulty) + (int)timer;
            Day5Text();
            Orders[1, 0] = 1;
            Orders[1, 1] = 1;
            OrdersCombined[0] += 1;
            OrdersCombined[1] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(40 + (15 * difficulty), 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            spiderInEffect = true;
            wormInEffect = true;
            StartCoroutine("SpiderMonster");
            StartCoroutine("WormMonster");



            yield return new WaitForSeconds(35f + (15 * difficulty));
        }
        else
        {
            timer += 42f + (15f * difficulty);
            OrdersComplete[1] = true;

            if (ordersFailed[1])
            {
                OrderX[1].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(1, true);
            }

            StartCoroutine("SpiderMonster");
            StartCoroutine("WormMonster");
        }



        // ORDER 3
        OrderTags[2].SetActive(true);
        if (skipTo < 3)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 2;
            dailyLog.Add($"- {(int)timer} seconds: Order 3 requested");
            computerScript.updateLog();
            requestedByTimes[2] = 65 + (15 * difficulty) + (int)timer;
            Day5Text();
            Orders[2, 10] = 2;
            OrdersCombined[10] += 2;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(65 + (15 * difficulty), 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(63f + (15 * difficulty));
        }
        else
        {
            timer += 67f + (15f * difficulty);
            OrdersComplete[2] = true;

            if (ordersFailed[2])
            {
                OrderX[2].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(2, true);
            }
        }



        // ORDER 4
        OrderTags[3].SetActive(true);
        if (skipTo < 4)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 3;
            dailyLog.Add($"- {(int)timer} seconds: Order 4 requested");
            computerScript.updateLog();
            requestedByTimes[3] = 110 + (15 * difficulty) + (int)timer;
            Day5Text();
            Orders[3, 7] = 1;
            Orders[3, 2] = 2;
            OrdersCombined[7] += 1;
            OrdersCombined[2] += 2;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(110 + (15 * difficulty), 3, 1, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(108f + (15 * difficulty));
        }
        else
        {
            timer += 112f + (15f * difficulty);
            OrdersComplete[3] = true;

            if (ordersFailed[3])
            {
                OrderX[3].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(3, true);
            }
        }



        // ORDER 5
        OrderTags[4].SetActive(true);
        if (skipTo < 5)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 4;
            dailyLog.Add($"- {(int)timer} seconds: Order 5 requested");
            computerScript.updateLog();
            requestedByTimes[4] = 95 + (15 * difficulty) + (int)timer;
            Day5Text();
            Orders[4, 3] = 1;
            Orders[4, 4] = 1;
            OrdersCombined[3] += 1;
            OrdersCombined[4] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(95 + (15 * difficulty), 4, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f); 
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(90f + (15 * difficulty));
        }
        else
        {
            timer += 97f + (15f * difficulty);
            OrdersComplete[4] = true;

            if (ordersFailed[4])
            {
                OrderX[4].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(4, true);
            }
        }



        /*// ORDER 6
        dailyLog.Add($"- {(int)timer} seconds: Order 6 requested");
        computerScript.updateLog();
        requestedByTimes[5] = 110 + (15 * difficulty) + (int)timer;
        Day5Text();
        OrderTags[5].SetActive(true);
        Orders[5, 0] = 2;
        Orders[5, 1] = 1;
        Orders[5, 5] = 2;
        OrdersCombined[0] += 2;
        OrdersCombined[1] += 1;
        OrdersCombined[5] += 2;
        yield return new WaitForSeconds(1f);

        StartCoroutine(OrderTimer(110 + (15 * difficulty), 5, 2, 1, 0, 0, 0, 2, 0, 0, 0, 0, 0));
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



        yield return new WaitForSeconds(93f + (15 * difficulty));*/



        // ORDER 7
        OrderTags[5].SetActive(true);
        if (skipTo < 6)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 5;
            dailyLog.Add($"- {(int)timer} seconds: Order 7 requested");
            computerScript.updateLog();
            requestedByTimes[5] = 100 + (15 * difficulty) + (int)timer;
            Day5Text();
            Orders[5, 0] = 1;
            Orders[5, 2] = 1;
            Orders[5, 10] = 3;
            OrdersCombined[0] += 1;
            OrdersCombined[2] += 1;
            OrdersCombined[10] += 3;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(100 + (15 * difficulty), 5, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 3));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(92f + (15 * difficulty));
        }
        else
        {
            timer += 102f + (15f * difficulty);
            OrdersComplete[5] = true;

            if (ordersFailed[5])
            {
                OrderX[5].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(5, true);
            }
        }



        // ORDER 8
        OrderTags[6].SetActive(true);
        if (skipTo < 7)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 6;
            dailyLog.Add($"- {(int)timer} seconds: Order 8 requested");
            computerScript.updateLog();
            requestedByTimes[6] = 85 + (15 * difficulty) + (int)timer;
            Day5Text();
            Orders[6, 6] = 1;
            Orders[6, 9] = 1;
            OrdersCombined[6] += 1;
            OrdersCombined[9] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(85 + (15 * difficulty), 6, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(83f + (15 * difficulty));
        }
        else
        {
            timer += 87f + (15f * difficulty);
            OrdersComplete[6] = true;

            if (ordersFailed[6])
            {
                OrderX[6].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(6, true);
            }
        }



        // ORDER 9
        OrderTags[7].SetActive(true);
        if (skipTo < 8)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 7;
            dailyLog.Add($"- {(int)timer} seconds: Order 9 requested");
            computerScript.updateLog();
            requestedByTimes[7] = 90 + (15 * difficulty) + (int)timer;
            Day5Text();
            Orders[7, 10] = 5;
            OrdersCombined[10] += 5;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(90 + (15 * difficulty), 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(92f + (15 * difficulty));
        }
        else
        {
            timer += 32f + (15f * difficulty);
            OrdersComplete[7] = true;

            if (ordersFailed[7])
            {
                OrderX[7].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(7, true);
            }
        }



        // ORDER 10
        OrdersLeftArrow.SetActive(true);
        OrdersRightArrow.SetActive(true);
        OrderTags[8].SetActive(true);
        if (skipTo < 9)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 8;
            dailyLog.Add($"- {(int)timer} seconds: Order 10 requested");
            computerScript.updateLog();
            requestedByTimes[8] = 140 + (15 * difficulty) + (int)timer;
            Day5Text();
            Orders[8, 1] = 1;
            Orders[8, 4] = 1;
            Orders[8, 7] = 3;
            Orders[8, 9] = 1;
            OrdersCombined[1] += 1;
            OrdersCombined[4] += 1;
            OrdersCombined[7] += 3;
            OrdersCombined[9] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(140 + (15 * difficulty), 8, 0, 1, 0, 0, 1, 0, 0, 3, 0, 1, 0));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(126f + (15 * difficulty));
        }
        else
        {
            timer += 142f + (15f * difficulty);
            OrdersComplete[8] = true;

            if (ordersFailed[8])
            {
                OrderX[8].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(8, true);
            }
        }



        // ORDER 11
        OrderTags[9].SetActive(true);
        if (skipTo < 10)
        {
            for (int i = 0; i < 11; i++)
            {
                errors[i] += errorsMidOrder[i];
            }
            errorsMidOrder = new int[11];
            curOrder = 9;
            dailyLog.Add($"- {(int)timer} seconds: Order 11 requested");
            computerScript.updateLog();
            requestedByTimes[9] = 150 + (15 * difficulty) + (int)timer;
            Day5Text();
            Orders[9, 0] = 3;
            Orders[9, 3] = 2;
            Orders[9, 7] = 1;
            Orders[9, 9] = 1;
            Orders[9, 10] = 1;
            OrdersCombined[0] += 3;
            OrdersCombined[3] += 2;
            OrdersCombined[7] += 1;
            OrdersCombined[9] += 1;
            OrdersCombined[10] += 1;
            yield return new WaitForSeconds(1f);

            StartCoroutine(OrderTimer(150 + (15 * difficulty), 9, 3, 0, 0, 2, 0, 0, 0, 1, 0, 1, 1));
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            Alarm.Play();
            StartCoroutine(switchRed());
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
            yield return new WaitForSeconds(3f);
            Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



            yield return new WaitForSeconds(136f + (15 * difficulty));
        }
        else
        {
            timer += 155f + (15f * difficulty);
            OrdersComplete[9] = true;

            if (ordersFailed[9])
            {
                OrderX[9].SetActive(true);
                curOrder++;
            }
            else
            {
                checkmarkOrder(9, true);
            }
        }



        /*// ORDER 12
        dailyLog.Add($"- {(int)timer} seconds: Order 12 requested");
        computerScript.updateLog();
        orderContents[11] += $"";
        OrderTags[11].SetActive(true);
        Orders[11, 0] = 3;
        Orders[11, 5] = 1;
        Orders[11, 6] = 2;
        Orders[11, 8] = 1;
        Orders[11, 10] = 1;
        OrdersCombined[0] += 3;
        OrdersCombined[5] += 1;
        OrdersCombined[6] += 2;
        OrdersCombined[8] += 1;
        OrdersCombined[10] += 1;
        yield return new WaitForSeconds(1f);

        StartCoroutine(OrderTimer(110 + (15 * difficulty), 11, 3, 0, 0, 0, 0, 1, 2, 0, 1, 0, 1));
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



        yield return new WaitForSeconds(79f + (15 * difficulty));*/



        /*// ORDER 13
        dailyLog.Add($"- {(int)timer} seconds: Order 13 requested");
        computerScript.updateLog();
        orderContents[12] += $"";
        OrderTags[12].SetActive(true);
        Orders[12, 1] = 3;
        Orders[12, 2] = 1;
        Orders[12, 3] = 1;
        Orders[12, 4] = 1;
        OrdersCombined[1] += 3;
        OrdersCombined[2] += 1;
        OrdersCombined[3] += 1;
        OrdersCombined[4] += 1;
        yield return new WaitForSeconds(1f);

        StartCoroutine(OrderTimer(120 + (15 * difficulty), 12, 0, 3, 1, 1, 1, 0, 0, 0, 0, 0, 0));
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));



        yield return new WaitForSeconds(92f + (15 * difficulty));*/


        /*// ORDER 14
        dailyLog.Add($"- {(int)timer} seconds: Order 14 requested");
        computerScript.updateLog();
        orderContents[13] += $"";
        OrderTags[13].SetActive(true);
        Orders[13, 1] = 1;
        Orders[13, 2] = 1;
        Orders[13, 3] = 1;
        Orders[13, 6] = 3;
        Orders[13, 9] = 2;
        Orders[13, 10] = 5;
        OrdersCombined[1] += 3;
        OrdersCombined[2] += 1;
        OrdersCombined[3] += 1;
        OrdersCombined[6] += 3;
        OrdersCombined[9] += 2;
        OrdersCombined[10] += 5;
        yield return new WaitForSeconds(1f);

        StartCoroutine(OrderTimer(140 + (15 * difficulty), 12, 0, 1, 1, 1, 0, 0, 3, 0, 0, 2, 5));
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        Alarm.Play();
        StartCoroutine(switchRed());
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));
        yield return new WaitForSeconds(3f);
        Instantiate(Meat, new Vector3(-45, 2, 1.5f), Quaternion.Euler(0, Random.Range(90, 270), 0));

        yield return new WaitForSeconds(111f + (15 * difficulty));*/

        for (int i = 0; i < 11; i++)
        {
            errors[i] += errorsMidOrder[i];
        }
        errorsMidOrder = new int[11];
        finishDay(4);

        if (TotalDayErrors >= (5 + (2 * Difficulty))) // failed
        {
            computerScript.clockOut(false, true);
        }
        else
        {
            FinalMessage.SetActive(true);
            DoorCollider.enabled = false;
            ClockOut.color = new Color32(0, 125, 0, 255);
            finalMessageNotif.SetActive(true);
        }
    }
     
    public void Day5Text()
    {
        /*orderContents[0] = $"Order Contents: \n\n- 2 Uncut Pieces Of Meat {(ordersComplete[0] ? 2 : ordersSent[1])}/2\n\n Requested By: {requestedByTimes[0]} Seconds";
        orderContents[1] = $"Order Contents: \n\n- 1 Half Piece Of Meat {(ordersComplete[1] ? 1 : ordersSent[0])}/1\n- 1 Uncut Piece Of Meat {(ordersComplete[1] ? 1 : ordersSent[1])}/1\n\n Requested By: {requestedByTimes[1]} Seconds";
        orderContents[2] = $"Order Contents: \n\n- 2 Jars Of Meat {(ordersComplete[2] ? 2 : ordersSent[10])}/2\n\nRequested By: {requestedByTimes[2]} Seconds";
        orderContents[3] = $"Order Contents: \n\n- 1 Half Piece Of Meat {(ordersComplete[3] ? 1 : ordersSent[0])}/1\n- 2 Bags Containing A Half Piece Of Meat {(ordersComplete[3] ? 2 : ordersSent[2])}/2\n\n Requested By: {requestedByTimes[3]} Seconds";
        orderContents[4] = $"Order Contents: \n\n- 1 Bag Containing 1 Uncut Piece Of Meat {(ordersComplete[4] ? 1 : ordersSent[3])}/1\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[4] ? 1 : ordersSent[4])}/1\n\n Requested By: {requestedByTimes[4]} Seconds";
        orderContents[5] = $"Order Contents: \n\n- 2 Bags Containing 2 Uncut Pieces Of Meat {(ordersComplete[5] ? 2 : ordersSent[5])}/2\n- 2 Half Piece Of Meat {(ordersComplete[5] ? 2 : ordersSent[0])}/2\n- 1 Uncut Piece Of Meat {(ordersComplete[5] ? 1 : ordersSent[1])}/1\n\nRequested By: {requestedByTimes[5]} Seconds";
        orderContents[6] = $"Order Contents: \n\n- 3 Jars Of Meat {(ordersComplete[6] ? 3 : ordersSent[10])}/3\n- 1 Half Piece Of Meat {(ordersComplete[6] ? 1 : ordersSent[0])}/1\n- 1 Bag Containing A Half Piece Of Meat {(ordersComplete[6] ? 1 : ordersSent[2])}/1\n\nRequested By: {requestedByTimes[6]} Seconds";
        orderContents[7] = $"Order Contents: \n\n- 1 Well Cooked Uncut Piece Of Meat {(ordersComplete[7] ? 1 : ordersSent[9])}/1\n- 1 Lightly Cooked Half Piece Of Meat {(ordersComplete[7] ? 1 : ordersSent[6])}/1\n\nRequested By: {requestedByTimes[7]} Seconds";
        orderContents[8] = $"Order Contents: \n\n- 5 Jars Of Meat {(ordersComplete[8] ? 5 : ordersSent[10])}/5\n\nRequested By: {requestedByTimes[8]} Seconds";
        orderContents[9] = $"Order Contents: \n\n- 3 Well Cooked Half Pieces Of Meat {(ordersComplete[9] ? 3 : ordersSent[7])}/3\n- 1 Well Cooked Uncut Piece Of Meat {(ordersComplete[9] ? 1 : ordersSent[9])}/1\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[9] ? 1 : ordersSent[4])}/1\n- 1 Uncut Piece Of Meat {(ordersComplete[9] ? 1 : ordersSent[1])}/1\n\nRequested By: {requestedByTimes[9]} Seconds";
        orderContents[10] = $"Order Contents: \n\n- 2 Bags Containing 1 Uncut Piece Of Meat {(ordersComplete[10] ? 2 : ordersSent[3])}/2\n- 3 Half Pieces Of Meat {(ordersComplete[10] ? 3 : ordersSent[0])}/3\n- 1 Jar Of Meat {(ordersComplete[10] ? 1 : ordersSent[10])}/1\n- 1 Well Cooked Uncut Piece Of Meat Of Meat {(ordersComplete[9] ? 1 : ordersSent[9])}/1\n- 1 Well Cooked Half Piece Of Meat {(ordersComplete[7] ? 1 : ordersSent[7])}/1\n\nRequested By: {requestedByTimes[10]} Seconds";*/
        orderContents[0] = $"Order Contents: \n\n- 2 Uncut Pieces Of Meat {(ordersComplete[0] ? 2 : ordersFailed[0] ? 0 : ordersSent[1])}/2\n\n Requested By: {requestedByTimes[0]} Seconds";
        orderContents[1] = $"Order Contents: \n\n- 1 Half Piece Of Meat {(ordersComplete[1] ? 1 : ordersFailed[1] ? 0 : ordersSent[0])}/1\n- 1 Uncut Piece Of Meat {(ordersComplete[1] ? 1 : ordersFailed[1] ? 0 : ordersSent[1])}/1\n\n Requested By: {requestedByTimes[1]} Seconds";
        orderContents[2] = $"Order Contents: \n\n- 2 Jars Of Meat {(ordersComplete[2] ? 2 : ordersFailed[2] ? 0 : ordersSent[10])}/2\n\nRequested By: {requestedByTimes[2]} Seconds";
        orderContents[3] = $"Order Contents: \n\n- 1 Well Cooked Half Piece Of Meat {(ordersComplete[3] ? 1 : ordersFailed[3] ? 0 : ordersSent[7])}/1\n- 2 Bags Containing A Half Piece Of Meat {(ordersComplete[3] ? 2 : ordersFailed[3] ? 0 : ordersSent[2])}/2\n\n Requested By: {requestedByTimes[3]} Seconds";
        orderContents[4] = $"Order Contents: \n\n- 1 Bag Containing 1 Uncut Piece Of Meat {(ordersComplete[4] ? 1 : ordersFailed[4] ? 0 : ordersSent[3])}/1\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[4] ? 1 : ordersFailed[4] ? 0 : ordersSent[4])}/1\n\n Requested By: {requestedByTimes[4]} Seconds";
        //orderContents[5] = $"Order Contents: \n\n- 2 Bags Containing 2 Uncut Pieces Of Meat {(ordersComplete[5] ? 2 : ordersFailed[5] ? 0 : ordersSent[5])}/2\n- 2 Half Piece Of Meat {(ordersComplete[5] ? 2 : ordersFailed[5] ? 0 : ordersSent[0])}/2\n- 1 Uncut Piece Of Meat {(ordersComplete[5] ? 1 : ordersFailed[5] ? 0 : ordersSent[1])}/1\n\nRequested By: {requestedByTimes[5]} Seconds";
        orderContents[5] = $"Order Contents: \n\n- 3 Jars Of Meat {(ordersComplete[5] ? 3 : ordersFailed[5] ? 0 : ordersSent[10])}/3\n- 1 Half Piece Of Meat {(ordersComplete[5] ? 1 : ordersFailed[5] ? 0 : ordersSent[0])}/1\n- 1 Bag Containing A Half Piece Of Meat {(ordersComplete[5] ? 1 : ordersFailed[5] ? 0 : ordersSent[2])}/1\n\nRequested By: {requestedByTimes[5]} Seconds";
        orderContents[6] = $"Order Contents: \n\n- 1 Well Cooked Uncut Piece Of Meat {(ordersComplete[6] ? 1 : ordersFailed[6] ? 0 : ordersSent[9])}/1\n- 1 Lightly Cooked Half Piece Of Meat {(ordersComplete[6] ? 1 : ordersFailed[6] ? 0 : ordersSent[6])}/1\n\nRequested By: {requestedByTimes[6]} Seconds";
        orderContents[7] = $"Order Contents: \n\n- 5 Jars Of Meat {(ordersComplete[7] ? 5 : ordersFailed[7] ? 0 : ordersSent[10])}/5\n\nRequested By: {requestedByTimes[7]} Seconds";
        orderContents[8] = $"Order Contents: \n\n- 3 Well Cooked Half Pieces Of Meat {(ordersComplete[8] ? 3 : ordersFailed[8] ? 0 : ordersSent[7])}/3\n- 1 Well Cooked Uncut Piece Of Meat {(ordersComplete[8] ? 1 : ordersFailed[8] ? 0 : ordersSent[9])}/1\n- 1 Bag Containing 1 And A Half Pieces Of Meat {(ordersComplete[8] ? 1 : ordersFailed[8] ? 0 : ordersSent[4])}/1\n- 1 Uncut Piece Of Meat {(ordersComplete[8] ? 1 : ordersFailed[8] ? 0 : ordersSent[1])}/1\n\nRequested By: {requestedByTimes[8]} Seconds";
        orderContents[9] = $"Order Contents: \n\n- 2 Bags Containing 1 Uncut Piece Of Meat {(ordersComplete[9] ? 2 : ordersFailed[9] ? 0 : ordersSent[3])}/2\n- 3 Half Pieces Of Meat {(ordersComplete[9] ? 3 : ordersFailed[9] ? 0 : ordersSent[0])}/3\n- 1 Jar Of Meat {(ordersComplete[9] ? 1 : ordersFailed[9] ? 0 : ordersSent[10])}/1\n- 1 Well Cooked Uncut Piece Of Meat Of Meat {(ordersComplete[9] ? 1 : ordersFailed[9] ? 0 : ordersSent[9])}/1\n- 1 Well Cooked Half Piece Of Meat {(ordersComplete[9] ? 1 : ordersFailed[9] ? 0 : ordersSent[7])}/1\n\nRequested By: {requestedByTimes[9]} Seconds";
        ordersComplete[10] = true;
        //orderContents[11] = $"Order Contents: \n\n- 1 Lightly Cooked Uncut Piece Of Meat\n- 2 Lightly Cooked Half Pieces Of Meat\n- 3 Half Pieces Of Meat Unbagged\n- 1 Bag Containing 2 Uncut Pieces Of Meat\n- 1 Jar Of Meat\n\nRequested By: {110 + (15 * difficulty)} Seconds";
        //orderContents[12] = $"Order Contents: \n\n- 1 Bag Containing A Half Piece Of Meat\n- 1 Bag Containing 1 Uncut Piece Of Meat\n- 1 Bag Containing 1 And A Half Pieces Of Meat\n- 3 Uncut Pieces Of Meat Unbagged\n\nRequested By: {120 + (15 * difficulty)} Seconds";
        //orderContents[13] = $"Order Contents: \n\n- 5 Jars Of Meat\n- 1 Bag Containing 1 Uncut Piece Of Meat\n- 2 Well Cooked Uncut Pieces Of Meat\n- 3 Lightly Cooked Half Pieces Of Meat\n- 1 Bag Containing A Half Piece Of Meat\n- 1 Uncut Piece Of Meat Unbagged\n\nRequested By: {140 + (15 * difficulty)} Seconds";
        ordersComplete[11] = true;
        ordersComplete[12] = true;
        ordersComplete[13] = true;
        ordersComplete[14] = true;
        ordersComplete[15] = true;
    }
}