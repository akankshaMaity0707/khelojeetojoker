using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class TimerController_TitliSorat : MonoBehaviour
{
    public static TimerController_TitliSorat inst;
    [SerializeField] private float timeLeft = 25f;
    public Text timerText;
    public Text DrawTime;
    private bool canStart = false;
    //public Text placeyourbet;
    public Color cDefaultCol;
    public Color cWarningColor;

    public UnityAction TimeUp;
    public UnityAction WheelSpinTime;
    float timeSpend;

    public bool isWheelRunning;

    private bool stopBetting = false;
   // private bool firstTime = true;

    //new code
    private DateTime STartDateTime;
    private void Awake()
    {
        inst = this;
        if (Application.isMobilePlatform)
        {
            Application.runInBackground = true;
        }

    }
    // Use this for initialization
    void Start()
    {
        if (timeLeft > 0)
            StartTimer(timeLeft);

        isWheelRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canStart && timeLeft > 0)
        {
            timeSpend = timeLeft - (float)((DateTime.Now - STartDateTime).Duration().TotalSeconds);



            timerText.text = timeSpend.ToString("00");
            if (timeSpend <= 10 && timeSpend > 07)
            {
                timerText.color = cWarningColor;
            }


            if (timeSpend <= 07)
            {
                if (!stopBetting)
                {
                    stopBetting = true;
                    timerText.color = Color.red;

                    TittliSorat_GameManager.instance.Block.SetActive(true);
                    TimeUp?.Invoke();

                }
            }
            if (timeSpend <= 0)
            {
                timeSpend = 0;
                canStart = false;
                WheelSpinTime?.Invoke();
            }
        }
    }

    public void StartTimer(float totalTime)
    {
        TittliSorat_GameManager.instance.Block.SetActive(false);
        stopBetting = false;
        STartDateTime = DateTime.Now;
        timeLeft = totalTime;
        canStart = true;
        timerText.color = cDefaultCol;
        double dateReturn = Math.Round((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
       // dateReturn += 90000;
        dateReturn += 15000;
        //Debug.LogError("dateReturn " + dateReturn);

        long time = long.Parse(dateReturn.ToString()); ;
        DateTimeOffset dateTimeOffSet = DateTimeOffset.FromUnixTimeMilliseconds(time);
        DateTime _datTime = dateTimeOffSet.DateTime;

        //Debug.Log("OnDrawTimeReceive : " + _datTime.Hour.ToString("00") + ":" + _datTime.Minute.ToString("00"));
        DrawTime.text = _datTime.Hour.ToString("00") + ":" + _datTime.Minute.ToString("00");
        TittliSorat_GameManager.instance.Time = DrawTime.text;
    }


    public void StopTimer()
    {
        stopBetting = false;
        canStart = false;
        timeSpend = 90f;
        timerText.text = "90";
        timerText.color = Color.red;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {

        }
        else
        {
            // LevelManager.inst.ShowUnexpectedQuitPanel();
            //Reset();
            //if (isWheelRunning)
            //{
            //   // DataCollector.instance.ManualWheelsTOPAfterpAUSE();
            //}
        }
    }

    private void Reset()
    {
        if ((timeSpend) <= 0)
        {
            StopTimer();
        }
    }

}

