using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComputerScript : MonoBehaviour
{
    [SerializeField] Image[] currentMessageOverlays;
    [SerializeField] Image[] currentSummaryOverlays;
    [SerializeField] Image[] unreads;
    [SerializeField] Image[] unreadsSummary;
    [SerializeField] string[] messages;
    [SerializeField] TMP_Text messageBody;
    [SerializeField] TMP_Text tabText;
    [SerializeField] TMP_Text date;
    [SerializeField] TMP_Text ordersPageNum;
    [SerializeField] TMP_Text summariesPageNum;
    [SerializeField] TMP_Text messagesPageNum;
    [SerializeField] TMP_Text textBox;
    [SerializeField] TMP_Text[] logLines;
    [SerializeField] GameObject finalMessageNotif;
    [SerializeField] GameObject newMessageNotif;
    [SerializeField] GameObject Stick;
    [SerializeField] GameObject WaitUntilClick;
    [SerializeField] GameObject logScrollBar;
    [SerializeField] GameObject LogTab;
    [SerializeField] GameObject LogButton; 
    [SerializeField] GameObject BlackScreen;
    [SerializeField] GameObject FiredText;
    [SerializeField] GameObject MessageTab;
    [SerializeField] GameObject OrdersTab;
    [SerializeField] GameObject SummaryTab;
    [SerializeField] GameObject StartDayButton;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject PlayerCam;
    [SerializeField] GameObject CamObject;
    [SerializeField] GameObject FireCam;
    [SerializeField] GameObject FirstOrdersPage;
    [SerializeField] GameObject SecondOrdersPage;
    [SerializeField] GameObject FirstSummariesPage;
    [SerializeField] GameObject SecondSummariesPage;
    [SerializeField] GameObject FirstMessagesPage;
    [SerializeField] GameObject SecondMessagesPage;
    [SerializeField] GameObject[] MessageObjects;
    [SerializeField] GameObject[] SummaryObjects;
    [SerializeField] Animator StartUI;
    [SerializeField] AudioSource GunSound;
    [SerializeField] AudioSource Thud;
    [SerializeField] AudioSource Talk;

    //Bagger Display
    [SerializeField] TMP_Text numPiecesInBag;

    int scroll;
    bool historyTab;
    int orderGlobal;

    private string[] lines = {
        "I have no need for useless employees",
        "I thought I told you to make no errors",
        "No wonder you couldn't find a job anywhere else"
    };
    private string lastLine = "Oh well, I'll find someone new";

    private void Start()
    {
        changeTab(1);
    }

    private void Update()
    {
        numPiecesInBag.text = EventHandler.i.PiecesInBagger.ToString();

        float scr = Input.mouseScrollDelta.y;

        if (scr != 0 && historyTab)
        {
            if (scr > 0)
            {
                if (scroll > 0)
                {
                    scroll--;
                }
                updateLog();
            }
            else
            {
                scroll++;
                updateLog();
            }
        }
    }

    public void changeTab(int tab) // 1 : Chat, 2 : Orders, 3 : Buying
    {
        if (tab == 1)
        {
            orderGlobal = -1;
            tabText.text = "MESSAGES";
            MessageTab.SetActive(true);
            OrdersTab.SetActive(false);
            SummaryTab.SetActive(false);
            clearPreviousTabInfo();
        }
        else if (tab == 2)
        {
            orderGlobal = -1;
            tabText.text = "ORDERS";
            MessageTab.SetActive(false);
            OrdersTab.SetActive(true);
            SummaryTab.SetActive(false);
            clearPreviousTabInfo();
        }
        else if (tab == 3)
        {
            orderGlobal = -1;
            tabText.text = "SUMMARY";
            MessageTab.SetActive(false);
            OrdersTab.SetActive(false);
            SummaryTab.SetActive(true);
            clearPreviousTabInfo();
        }
    }

    public void updateMessageBody()
    {
        if (orderGlobal != -1)
        {
            messageBody.text = EventHandler.i.OrderContents[orderGlobal - 1];
        }
    }

    private void clearPreviousTabInfo()
    {
        historyTab = false;
        LogTab.SetActive(false);

        for (int i = 0; i < currentMessageOverlays.Length; i++)
        {
            currentMessageOverlays[i].color = new Color(0f, 255f, 0f, 0f);
        }

        for (int i = 0; i < currentSummaryOverlays.Length; i++)
        {
            currentSummaryOverlays[i].color = new Color(0f, 255f, 0f, 0f);
        }

        messageBody.text = "";
    }

    public void changeMessage(int message)
    {
        if (message == 10)
        {
            finalMessageNotif.SetActive(false);
        }

        historyTab = false;

        // Remove all overlays
        for (int i = 0; i < currentMessageOverlays.Length; i++)
        {
            currentMessageOverlays[i].color = new Color(currentMessageOverlays[message - 1].color.r, currentMessageOverlays[message - 1].color.g, currentMessageOverlays[message - 1].color.b, 0f);
        }

        // Current message
        messageBody.text = messages[message - 1];
        currentMessageOverlays[message - 1].color = new Color(currentMessageOverlays[message - 1].color.r, currentMessageOverlays[message - 1].color.g, currentMessageOverlays[message - 1].color.b, 0.14f);

        // Remove unread marker
        if (unreads[message - 1].enabled)
        {
            unreads[message - 1].enabled = false;
        }

        // Check if every message has been read
        bool allRead = true;
        for (int i = 0; i < unreads.Length; i++)
        {
            print(unreads[i].enabled);
            if (unreads[i].enabled && unreads[i].gameObject.transform.parent.gameObject.activeSelf)
            {
                allRead = false;
                break;
            }
        }
        if (allRead)
        {
            newMessageNotif.SetActive(false);
        }
    }

    public void changeSummary(int summary)
    {
        historyTab = false;

        // Remove all overlays
        for (int i = 0; i < currentSummaryOverlays.Length; i++)
        {
            currentSummaryOverlays[i].color = new Color(currentSummaryOverlays[summary - 1].color.r, currentSummaryOverlays[summary - 1].color.g, currentSummaryOverlays[summary - 1].color.b, 0f);
        }

        // Current summary
        messageBody.text = EventHandler.i.Summaries[summary - 1];
        currentSummaryOverlays[summary - 1].color = new Color(currentSummaryOverlays[summary - 1].color.r, currentSummaryOverlays[summary - 1].color.g, currentSummaryOverlays[summary - 1].color.b, 0.14f);

        // Remove unread marker
        if (unreadsSummary[summary - 1].enabled)
        {
            unreadsSummary[summary - 1].enabled = false;
        }
    }

    public void changeOrder(int order)
    {
        orderGlobal = order;
        historyTab = false;
        LogTab.SetActive(false);
        updateMessageBody();
    }

    public void changeLog()
    {
        orderGlobal = -1;
        updateLog();
        clearPreviousTabInfo();
        historyTab = true;
        LogTab.SetActive(true);
    }

    public void startDay()
    {
        EventHandler.i.startDay();
        StartDayButton.SetActive(false);
        LogButton.SetActive(true);
    }

    public void retryOrderTutorial()
    {
        EventHandler.i.OrdersSent[0] = 0;
        EventHandler.i.OrdersSent[1] = 0;
        EventHandler.i.OrdersSent[2] = 0;
        EventHandler.i.OrdersSent[3] = 0;
        EventHandler.i.OrdersSent[4] = 0;
        EventHandler.i.OrdersSent[5] = 0;
        EventHandler.i.RetryTutorialOrder = true;
    }

    public void ClockOut(bool day5)
    {
        clockOut(false, day5);
    }

    public void clockOut(bool starting, bool day5)
    {
        if ((EventHandler.i.DayDone && EventHandler.i.Day != 5) || (starting == false && day5 == true))
        {
            newMessageNotif.SetActive(true);

            Player.GetComponent<Interact>().clickedClockOut = true;

            EventHandler.i.NotInDay = true;
            bool failed = true;

            StartDayButton.SetActive(true);
            LogButton.SetActive(false);
            EventHandler.i.DailyLog.Clear();

            if (EventHandler.i.Day == 1)
            {
                failed = false;
                EventHandler.i.Day++;
            }
            else if (EventHandler.i.Day == 2)
            {
                if (EventHandler.i.TotalDayErrors < (5 + (2 * EventHandler.i.Difficulty)))
                {
                    failed = false;
                    EventHandler.i.Day++;
                }
            }
            else if (EventHandler.i.Day == 3)
            {
                if (EventHandler.i.TotalDayErrors < (5 + (2 * EventHandler.i.Difficulty)))
                {
                    failed = false;
                    EventHandler.i.Day++;
                }
            }
            else if (EventHandler.i.Day == 4)
            {
                if (EventHandler.i.TotalDayErrors < (5 + (2 * EventHandler.i.Difficulty)))
                {
                    failed = false;
                    EventHandler.i.Day++;
                }
            }
            else if (EventHandler.i.Day == 5)
            {
                if (EventHandler.i.TotalDayErrors < (5 + (2 * EventHandler.i.Difficulty)))
                {
                    failed = false;
                    EventHandler.i.Day++;
                }
            }
            
            EventHandler.i.resetForNewDay();
            if (starting)
            {
                BlackScreen.SetActive(true);
                StartUI.SetBool("FadeOut", true);
            }
            else
            {
                StartUI.SetBool("FadeOut", true);
            }
            StartCoroutine(WaitForFadeOut(EventHandler.i.Day, failed, starting));
        }
    }

    IEnumerator WaitForFadeOut(int day, bool failed, bool starting)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(1.5f);
        if (starting)
        {
            BlackScreen.SetActive(false);
        }
        FirstOrdersPage.SetActive(true);
        SecondOrdersPage.SetActive(false);
        ordersPageNum.text = "Page 1";
        FirstSummariesPage.SetActive(true);
        SecondSummariesPage.SetActive(false);
        summariesPageNum.text = "Page 1";
        FirstMessagesPage.SetActive(true);
        SecondMessagesPage.SetActive(false);
        messagesPageNum.text = "Page 1";

        if (EventHandler.i.Day >= 2)
        {
            PlayerPrefs.SetInt("Day", 2);
            MessageObjects[2].SetActive(true);
            MessageObjects[3].SetActive(true);
            SummaryObjects[1].SetActive(true);
        }
        if (EventHandler.i.Day >= 3)
        {
            PlayerPrefs.SetInt("Day", 3);
            MessageObjects[4].SetActive(true);
            MessageObjects[5].SetActive(true);
            SummaryObjects[2].SetActive(true);
        }
        if (EventHandler.i.Day >= 4)
        {
            PlayerPrefs.SetInt("Day", 4);
            Stick.SetActive(true);
            MessageObjects[6].SetActive(true);
            MessageObjects[7].SetActive(true);
            SummaryObjects[3].SetActive(true);
        }
        if (EventHandler.i.Day >= 5)
        {
            PlayerPrefs.SetInt("Day", 5);
            MessageObjects[8].SetActive(true);
            SummaryObjects[4].SetActive(true);
        }

        if (!starting && Player.GetComponent<Interact>().isComputer)
        {
            Player.GetComponent<Interact>().Access();
        }

        yield return new WaitForSeconds(3f);

        if (failed)
        {
            EventHandler.i.Summaries[day - 1] = "No data for the fifth day";

            Player.GetComponent<PlayerMove>().enabled = false;
            Player.GetComponent<CameraRotation>().enabled = false;

            CamObject.SetActive(false);
            FireCam.SetActive(true);

            StartUI.SetBool("FadeOut", false);
            StartCoroutine(EventHandler.i.StartingUIAnim("NONE", true));

            yield return new WaitForSeconds(6f);

            int random = Random.Range(0, 3);

            textBox.enabled = true;
            textBox.text = "";
            for (int i = 0; i < lines[random].Length; i++)
            {
                Talk.Play();
                textBox.text += lines[random][i];
                if (lines[random][i] != ' ') { yield return new WaitForSeconds(0.085f); }
            }
            WaitUntilClick.SetActive(true);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            WaitUntilClick.SetActive(false);
            textBox.text = "";
            for (int i = 0; i < lastLine.Length; i++)
            {
                Talk.Play();
                textBox.text += lastLine[i];
                if (lastLine[i] != ' ') { yield return new WaitForSeconds(0.085f); }
            }

            yield return new WaitForSeconds(2f);
            textBox.enabled = false;

            GunSound.Play();
            yield return new WaitForSeconds(0.1f);
            BlackScreen.SetActive(true);

            yield return new WaitForSeconds(2.9f);

            Thud.Play();
            FiredText.SetActive(true);

            yield return new WaitForSeconds(1f);

            StartUI.SetBool("FadeOut", true);

            yield return new WaitForSeconds(3f);
            FiredText.SetActive(false);
            BlackScreen.SetActive(false);

            Player.GetComponent<PlayerMove>().enabled = true;
            Player.GetComponent<CameraRotation>().enabled = true;

            CamObject.SetActive(true);
            FireCam.SetActive(false);
        }

        EventHandler.i.TotalDayErrors = 0;

        PlayerCam.transform.rotation = Quaternion.Euler(0, 90f, 0);
        Player.GetComponent<CameraRotation>().yRot = -90f;
        Player.transform.position = new Vector3(-19f, 2, 21.6f);

        EventHandler.i.NotInDay = false;
        StartUI.SetBool("FadeOut", false);
        if (day == 1)
        {
            StartCoroutine(EventHandler.i.StartingUIAnim("MONDAY", false));
            date.text = "MONDAY";
        }
        else if (day == 2)
        {
            StartCoroutine(EventHandler.i.StartingUIAnim("TUESDAY", false));
            date.text = "TUESDAY";
        }
        else if (day == 3)
        {
            StartCoroutine(EventHandler.i.StartingUIAnim("WEDNESDAY", false));
            date.text = "WEDNESDAY";
        }
        else if (day == 4)
        {
            StartCoroutine(EventHandler.i.StartingUIAnim("THURSDAY", false));
            date.text = "THURSDAY";
        }
        else if (day == 5)
        {
            StartCoroutine(EventHandler.i.StartingUIAnim("FRIDAY", false));
            date.text = "FRIDAY";
        }
        else if (day == 6)
        {
            StartCoroutine(EventHandler.i.StartingUIAnim("SATURDAY", false));
            date.text = "SATURDAY";
        }
        else if (day == 7)
        {
            StartCoroutine(EventHandler.i.StartingUIAnim("SUNDAY", false));
            date.text = "SUNDAY";
        }
        Player.GetComponent<Interact>().clickedClockOut = false;
    }

    public void changeOrderPage(int dir) // 0 left, 1 right
    {
        if (dir == 0)
        {
            FirstOrdersPage.SetActive(true);
            SecondOrdersPage.SetActive(false);
            ordersPageNum.text = "Page 1";
        }
        else
        {
            FirstOrdersPage.SetActive(false);
            SecondOrdersPage.SetActive(true);
            ordersPageNum.text = "Page 2";
        }
    }

    public void changeSummaryPage(int dir) // 0 left, 1 right
    {
        if (dir == 0)
        {
            FirstSummariesPage.SetActive(true);
            SecondSummariesPage.SetActive(false);
            summariesPageNum.text = "Page 1";
        }
        else
        {
            FirstSummariesPage.SetActive(false);
            SecondSummariesPage.SetActive(true);
            summariesPageNum.text = "Page 2";
        }
    }

    public void changeMessagesPage(int dir) // 0 left, 1 right
    {
        if (dir == 0)
        {
            FirstMessagesPage.SetActive(true);
            SecondMessagesPage.SetActive(false);
            messagesPageNum.text = "Page 1";
        }
        else
        {
            FirstMessagesPage.SetActive(false);
            SecondMessagesPage.SetActive(true);
            messagesPageNum.text = "Page 2";
        }
    }

    public void updateLog()
    {
        if (50f - 3 * scroll > -650f)
        {
            logScrollBar.transform.localPosition = new Vector3(839f, 50f - 3 * scroll, 1f);
        }
        EventHandler handler = EventHandler.i;
        for (int i = 0; i < logLines.Length; i++)
        {
            int a = i + scroll;

            if (a >= handler.DailyLog.Count)
            {
                logLines[i].text = "";
            }
            else
            {
                logLines[i].text = handler.DailyLog[a];
            }

        }
    }
}
