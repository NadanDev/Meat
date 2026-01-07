using UnityEngine;
using System.Collections;

public class Bag : MonoBehaviour
{
    Rigidbody rb;

    public int numPieces = 0;
    ComputerScript computerScript;

    private bool disableExtraTriggers = false;
    private bool spawnCooldown = false;

    private void Awake()
    {
        computerScript = GameObject.FindGameObjectWithTag("Computer").GetComponent<ComputerScript>();

        rb = GetComponent<Rigidbody>();

        if (transform.position == new Vector3(-32f, 1.5f, 34.5f))
        {
            numPieces = EventHandler.i.PiecesInBagger;
            EventHandler.i.PiecesInBagger = 0;
        }
        else
        {
            numPieces = EventHandler.i.PiecesInHeldBag;
            EventHandler.i.PiecesInHeldBag = 0;

            rb.AddForce(transform.forward * 5, ForceMode.Impulse);
        }

        if (numPieces == 1)
        {
            transform.localScale = new Vector3(150, 250, 150);
        }
        else if (numPieces == 2)
        {
            transform.localScale = new Vector3(200, 300, 200);
        }
        else if (numPieces == 3)
        {
            transform.localScale = new Vector3(250, 350, 250);
        }
        else
        {
            transform.localScale = new Vector3(300, 400, 300);
        }

        StartCoroutine(SpawnCooldown());
    }

    IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(1f);
        spawnCooldown = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndOfBelt") && EventHandler.i.DayIsGoing)
        {
            if (numPieces == 1 && !disableExtraTriggers)
            {
                disableExtraTriggers = true;

                if (EventHandler.i.OrdersCombined[2] > 0)
                {
                    EventHandler.i.OrdersSent[2]++;
                    EventHandler.i.OrdersCombined[2]--;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Successfully received a bag containing a half piece worth of meat");
                }
                else
                {
                    EventHandler.i.ErrorsMidOrder[2]++;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Error, received an unwanted bag containing a half piece worth of meat");
                }
                computerScript.updateLog();

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
            else if (numPieces == 2 && !disableExtraTriggers)
            {
                disableExtraTriggers = true;

                if (EventHandler.i.OrdersCombined[3] > 0)
                {
                    EventHandler.i.OrdersSent[3]++;
                    EventHandler.i.OrdersCombined[3]--;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Successfully received a bag containing 1 piece worth of meat");
                }
                else
                {
                    EventHandler.i.ErrorsMidOrder[3]++;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Error, received an unwanted bag containing 1 piece worth of meat");
                }
                computerScript.updateLog();

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
            else if (numPieces == 3 && !disableExtraTriggers)
            {
                disableExtraTriggers = true;

                if (EventHandler.i.OrdersCombined[4] > 0)
                {
                    EventHandler.i.OrdersSent[4]++;
                    EventHandler.i.OrdersCombined[4]--;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Successfully received a bag containing 1 and a half pieces worth of meat");
                }
                else
                {
                    EventHandler.i.ErrorsMidOrder[4]++;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Error, received an unwanted bag containing 1 and a half pieces worth of meat");
                }
                computerScript.updateLog();

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
            else if (numPieces == 4 && !disableExtraTriggers)
            {
                disableExtraTriggers = true;

                if (EventHandler.i.OrdersCombined[5] > 0)
                {
                    EventHandler.i.OrdersSent[5]++;
                    EventHandler.i.OrdersCombined[5]--;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Successfully received a bag containing 2 pieces worth of meat");
                }
                else
                {
                    EventHandler.i.ErrorsMidOrder[5]++;
                    EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Error, received an unwanted bag containing 2 pieces worth of meat");
                }
                computerScript.updateLog();

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
            else
            {
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

                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("BaggerCollector") && spawnCooldown)
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Trash") && !disableExtraTriggers)
        {
            EventHandler.i.playSpiderEat();
            EventHandler.i.FoodFed += numPieces;
            disableExtraTriggers = true;
            Destroy(gameObject);
        }
    } 

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Belt") && EventHandler.i.DayIsGoing)
        {
            rb.AddForce(other.transform.up * -0.51f, ForceMode.Force);
        }
    }

    [SerializeField] AudioSource BagSound1;
    [SerializeField] AudioSource BagSound2;
    [SerializeField] AudioSource BagSound3;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }

        int val = Random.Range(1, 4);
        if (val == 1)
        {
            BagSound1.Play();
        }
        else if (val == 2)
        {
            BagSound2.Play();
        }
        else if (val == 3)
        {
            BagSound3.Play();
        }
    }
}
