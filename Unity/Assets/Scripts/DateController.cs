using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class DateController : MonoBehaviour
{
    [Header("Calendar")]
    [SerializeField] private GameObject startDate;
    [SerializeField] private GameObject endDate;

    [Header("Charts")]
    [SerializeField] private ElecticUsageChart electricUsageChart;

    [Header("Panel")]
    [SerializeField] private GameObject doorLockPanel;

    [Header("Managers")]
    [SerializeField] private UIManager uiManager;

    public GameObject currDateObj;
    [HideInInspector] public DateTime startDateTime = DateTime.Now;
    [HideInInspector] public DateTime endDateTime = DateTime.Now;

    public DateTime currDate;

    [HideInInspector] public string mode = "start";

    private void Awake()
    {
        //Debug.Log(endDateTime);
    }
    public void OnButtonClick(string info)
    {
        switch (info)
        {
            case ("start"):
                currDateObj = startDate;
                currDate = startDateTime;
                break;
            case ("end"):
                currDateObj = endDate;
                currDate = endDateTime;
                break;
            default:
                break;
        }
        mode = info;
        this.gameObject.SetActive(true);
    }

    public void SetDate(DateTime selectedDate)
    {
        string selected = selectedDate.Year.ToString() + "-" + selectedDate.Month.ToString() + "-" + selectedDate.Day.ToString();
        switch (mode)
        {
            case "start":
                if (selectedDate > endDateTime)
                {
                    Debug.LogWarning("Start date cannot be after end date.");
                    return;
                }
                startDateTime = selectedDate;
                break;
            case "end":
                if (selectedDate < startDateTime)
                {
                    Debug.LogWarning("End date cannot be before start date.");
                    return;
                }
                endDateTime = selectedDate;
                break;
            default:
                break;
        }
        currDateObj.GetComponent<Text>().text = selected;

        if (electricUsageChart)
        {
            //Debug.Log($"startDateTime: {startDateTime}, endDateTime: {endDateTime}");   
            electricUsageChart.ShowUsageChart(startDateTime, endDateTime);
        }
        if (doorLockPanel && uiManager)
        {
            uiManager.InitDoorLockEntry();
        }
    }
    public void Initialize(DateTime start, DateTime end)
    {
        string startTime = start.Year.ToString() + "-" + start.Month.ToString() + "-" + start.Day.ToString();
        string endTime = end.Year.ToString() + "-" + end.Month.ToString()+ "-" + end.Day.ToString();
        startDateTime = start;
        endDateTime = end;
        startDate.GetComponent<Text>().text = startTime;
        endDate.GetComponent<Text>().text = endTime;
    }
}
