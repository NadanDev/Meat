using UnityEngine;

public class Meat : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    public GameObject burntObj;
    public GameObject Smoke;
    ComputerScript computerScript;

    private bool disableExtraTriggers = false;
    private bool cookingLow;
    private bool cookingHigh;
    public bool cookedWell = false;
    public bool cookedLight = false;

    private float lowCookTime = 0;
    private float highCookTime = 0;

    private float lowCookTimeReq;
    private float highCookTimeReq;
    private float lowCookTimeBurn;
    private float highCookTimeBurn;

    public AudioSource cookedSound;

    private void Awake()
    {
        lowCookTimeReq = Random.Range(9f, 12f);
        highCookTimeReq = Random.Range(12f, 15f);
        lowCookTimeBurn = Random.Range(22f, 26f);
        highCookTimeBurn = Random.Range(28f, 32f);

    computerScript = GameObject.FindGameObjectWithTag("Computer").GetComponent<ComputerScript>();

        rb = GetComponent<Rigidbody>();

        if (transform.position != new Vector3(-45, 2, 1.5f))
        {
            rb.AddForce(transform.forward * 10, ForceMode.Impulse);
        }

        transform.rotation = Quaternion.Euler(0, Random.Range(75, 105), 0);
    }

    private void Start()
    {
        if (cookedWell)
        {
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = new Color32(56, 36, 17, 255);
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.3f);
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(3f, 0.25f);
            Smoke.SetActive(true);
        }
        else if (cookedLight)
        {
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = new Color32(135, 70, 0, 255);
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.4f);
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(2f, 0.25f);
            Smoke.SetActive(true);
        }
    }

    private void Update()
    {
        if (cookingLow)
        {
            lowCookTime += Time.deltaTime;

            if (lowCookTime > lowCookTimeBurn)
            {
                Instantiate(burntObj, transform.position, transform.rotation).GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 255);
                cookedSound.Play();
                Destroy(gameObject);
            }
            else if (lowCookTime > lowCookTimeReq && !cookedWell)
            {
                cookedWell = true;
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = new Color32(56, 36, 17, 255);
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.3f);
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(3f, 0.25f);
                Smoke.SetActive(true);
                cookedSound.Play();
            }
        }
        else if (cookingHigh)
        {
            highCookTime += Time.deltaTime;

            if (highCookTime > highCookTimeBurn)
            {
                Instantiate(burntObj, transform.position, transform.rotation).GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 255);
                cookedSound.Play();
                Destroy(gameObject);
            }
            else if (highCookTime > highCookTimeReq && !cookedWell && !cookedLight)
            {
                cookedLight = true;
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = new Color32(135, 70, 0, 255);
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.4f);
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(2f, 0.25f);
                Smoke.SetActive(true);
                cookedSound.Play();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Belt") && EventHandler.i.DayIsGoing)
        {
            rb.AddForce(other.transform.up * -0.3f, ForceMode.Force);
        }
        else if (other.CompareTag("LowerCooker"))
        {
            if (EventHandler.i.CookerOn)
            {
                cookingLow = true;
            }
            else
            {
                cookingLow = false;
            }
        }
        else if (other.CompareTag("UpperCooker"))
        {
            if (EventHandler.i.CookerOn)
            {
                cookingHigh = true;
            }
            else
            {
                cookingHigh = false;
            }
        }
        else if (other.CompareTag("Board"))
        {
            EventHandler.i.MeatOnBoard = true;
            EventHandler.i.BoardMeat = gameObject;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BaggerCollector") && !disableExtraTriggers)
        {
            if (EventHandler.i.PiecesInBagger + 2 < 10)
            {
                EventHandler.i.PiecesInBagger += 2;
            }
            disableExtraTriggers = true;
            Destroy(gameObject);
        }
        else if (other.CompareTag("EndOfBelt") && !disableExtraTriggers && EventHandler.i.DayIsGoing)
        {
            disableExtraTriggers = true;

            if (cookedWell)
            {
                if (EventHandler.i.OrdersCombined[9] > 0)
                {
                    EventHandler.i.OrdersSent[9]++;
                    EventHandler.i.OrdersCombined[9]--;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Successfully received a well cooked uncut piece of meat");
                }
                else
                {
                    EventHandler.i.ErrorsMidOrder[9]++;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Error, received an unwanted well cooked uncut piece of meat");
                }
                computerScript.updateLog();
            }
            else if (cookedLight)
            {
                if (EventHandler.i.OrdersCombined[8] > 0)
                {
                    EventHandler.i.OrdersSent[8]++;
                    EventHandler.i.OrdersCombined[8]--;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Successfully received a lightly cooked uncut piece of meat");
                }
                else
                {
                    EventHandler.i.ErrorsMidOrder[8]++;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Error, received an unwanted lightly cooked uncut piece of meat");
                }
                computerScript.updateLog();
            }
            else
            {
                if (EventHandler.i.OrdersCombined[1] > 0)
                {
                    EventHandler.i.OrdersSent[1]++;
                    EventHandler.i.OrdersCombined[1]--;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Successfully received an uncut piece of meat");
                }
                else
                {
                    EventHandler.i.ErrorsMidOrder[1]++;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Error, received an unwanted uncut piece of meat");
                }
                computerScript.updateLog();
            }

            /*for (int i = 0; i < EventHandler.i.Errors.Length; i++)
            {
                print("Error for " + i + ": " + EventHandler.i.Errors[i]);
            }
            print("\n");
            for (int i = 0; i < EventHandler.i.OrdersCombined.Length; i++)
            {
                print("Combined for " + i + ": " + EventHandler.i.OrdersCombined[i]);
            }
            print("\n");
            for (int i = 0; i < EventHandler.i.Orders.GetLength(0); i++)  // Loop through rows
            {
                for (int j = 0; j < EventHandler.i.Orders.GetLength(1); j++)  // Loop through columns
                {
                    print("Orders for " + i + "," + j + ": " + EventHandler.i.Orders[i, j]);
                }
                print("\n");
            }
            print("\n");
            for (int i = 0; i < EventHandler.i.OrdersSent.Length; i++)
            {
                print("Sent for " + i + ": " + EventHandler.i.OrdersSent[i]);
            }
            print("Next");*/

            for (int i = 0; i < 16; i++)
            {
                if (!EventHandler.i.OrdersComplete[i] && !EventHandler.i.OrdersFailed[i])
                {
                    bool orderComplete = true;
                    int validOrderCheck = 0;
                    for (int j = 0; j < 11; j++)
                    {
                        if (EventHandler.i.Orders[i, j] > EventHandler.i.OrdersSent[j])
                        {
                            orderComplete = false;
                            break;
                        }
                        validOrderCheck += EventHandler.i.Orders[i, j];
                    }

                    if (orderComplete && validOrderCheck != 0)
                    {
                        for (int j = 0; j < 11; j++)
                        {
                            EventHandler.i.OrdersSent[j] -= EventHandler.i.Orders[i, j];
                        }
                        EventHandler.i.OrdersComplete[i] = true;
                        EventHandler.i.checkmarkOrder(i, false);

                        EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Order {i + 1} completed");
                        computerScript.updateLog();

                        if (EventHandler.i.Day == 1)
                        {
                            EventHandler.i.TutorialOrderComplete = true;
                        }
                    }
                }
            }

            if (EventHandler.i.Day == 1)
            {
                EventHandler.i.Day1Text();
            }
            else if (EventHandler.i.Day == 2)
            {
                EventHandler.i.Day2Text();
            }
            else if (EventHandler.i.Day == 3)
            {
                EventHandler.i.Day3Text();
            }
            else if (EventHandler.i.Day == 4)
            {
                EventHandler.i.Day4Text();
            }
            else if (EventHandler.i.Day == 5)
            {
                EventHandler.i.Day5Text();
            }
            computerScript.updateMessageBody();

            Destroy(gameObject);
        }
        else if (other.CompareTag("Trash") && !disableExtraTriggers)
        {
            EventHandler.i.FoodFed += 2;
            EventHandler.i.playSpiderEat();
            disableExtraTriggers = true;
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Board"))
        {
            EventHandler.i.MeatOnBoard = false;
            EventHandler.i.BoardMeat = null;
        }
        else if (other.CompareTag("LowerCooker") && EventHandler.i.CookerOn)
        {
            cookingLow = false;
        }
        else if (other.CompareTag("UpperCooker") && EventHandler.i.CookerOn)
        {
            cookingHigh = false;
        }
    }

    [SerializeField] AudioSource MeatSound1;
    [SerializeField] AudioSource MeatSound2;
    [SerializeField] AudioSource MeatSound3;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }

        int val = Random.Range(1, 4);
        if (val == 1)
        {
            MeatSound1.Play();
        }
        else if (val == 2)
        {
            MeatSound2.Play();
        }
        else if (val == 3)
        {
            MeatSound3.Play();
        }
    }
}
