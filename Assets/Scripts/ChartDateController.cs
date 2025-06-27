using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class ChartDateController : MonoBehaviour
{
    [Header("Calendar")]
    [SerializeField] private GameObject startDate;
    [SerializeField] private GameObject endDate;

    public GameObject currDateObj;
    private DateTime startDateTime = DateTime.Now;
    private DateTime endDateTime = DateTime.Now;

    public DateTime currDate;

    private string mode = "start";
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
        currDateObj.GetComponent<Text>().text = selected;
        switch (mode)
        {
            case "start":
                startDateTime = selectedDate;
                break;
            case "end":
                endDateTime = selectedDate;
                break;
            default:
                break;
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
