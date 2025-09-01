using UnityEngine;

public class Jar : MonoBehaviour
{
    ComputerScript computerScript;
    Rigidbody rb;
    private bool disableExtraTriggers = false;

    [SerializeField] AudioSource Glass1;
    [SerializeField] AudioSource Glass2;
    private void Awake()
    {
        computerScript = GameObject.FindGameObjectWithTag("Computer").GetComponent<ComputerScript>();

        rb = GetComponent<Rigidbody>();

        if (transform.position != new Vector3(-31f, 2.5f, 9.925f))
        {
            rb.AddForce(transform.forward * 8, ForceMode.Impulse);
            transform.rotation = Quaternion.Euler(0, Random.Range(75, 105), 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BaggerCollector") && !disableExtraTriggers)
        {
            EventHandler.i.PiecesInBagger += 1;
            disableExtraTriggers = true;
            Destroy(gameObject);
        }
        else if (other.CompareTag("EndOfBelt") && !disableExtraTriggers && EventHandler.i.DayIsGoing)
        {
            disableExtraTriggers = true;

            if (EventHandler.i.OrdersCombined[10] > 0)
            {
                EventHandler.i.OrdersSent[10]++;
                EventHandler.i.OrdersCombined[10]--;
                EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Successfully received a jar of meat");
            }
            else
            {
                EventHandler.i.Errors[10]++;
                EventHandler.i.DailyLog.Add($"- {(int)EventHandler.i.Timer} seconds: Error, received an unwanted jar of meat");
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
                        EventHandler.i.checkmarkOrder(i);

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
            EventHandler.i.FoodFed += 1;
            disableExtraTriggers = true;
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Belt") && EventHandler.i.DayIsGoing)
        {
            rb.AddForce(other.transform.up * -0.65f, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int val = Random.Range(1, 3);
        if (val == 1)
        {
            Glass1.Play();
        }
        else if (val == 2)
        {
            Glass2.Play();
        }
    }
}
