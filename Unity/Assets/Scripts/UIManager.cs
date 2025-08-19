using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject userPanel;
    public GameObject homePanel;
    public GameObject viewerPanel;
    public GameObject chartPanel;
    public GameObject solarPowerPanel;
    public GameObject aiElecticalPanel;
    public GameObject doorLockPanel;

    [Header("UI Lists")]
    public GameObject doorLockList;
    public Image cctvImage;

    [Header("UI Prefabs")]
    public GameObject doorLockEntryPrefab;

    [Header("Widget")]
    public GameObject sliderToggle;
    public GameObject chartCalendarPanel;
    public GameObject pictrueWindow;
    public GameObject doorLockCalendarPanel;

    [Header("Charts")]
    public ElecticUsageChart electricUsageChart;

    [Header("Managers")]
    public SmartHomeManager smartHomeManager;

    private void Start()
    {
        if (isPanel())
        {
            ShowHomePanel();
            InitializeChartDate();
            InitializeDoorLockDate();
        }
    }

    public void ShowPanel(string panelName)
    {
        userPanel.SetActive(panelName == "User");
        homePanel.SetActive(panelName == "Home");
        if (panelName == "Viewer")
        {
            viewerPanel.SetActive(panelName == "Viewer");
            viewerPanel.GetComponent<Image>().raycastTarget = false;
        }
        chartPanel.SetActive(panelName == "Chart");
        solarPowerPanel.SetActive(panelName == "SolarPower");
        aiElecticalPanel.SetActive(panelName == "AIElectrical");
        doorLockPanel.SetActive(panelName == "DoorLock");
        sliderToggle.SetActive(false);
        chartCalendarPanel.SetActive(false);
        doorLockCalendarPanel.SetActive(false);
        pictrueWindow.SetActive(false);
    }

    public void ShowHomePanel()
    {
        if (isPanel())
        {
            ShowPanel("Home");

            sliderToggle.SetActive(false);

            chartCalendarPanel.SetActive(false);
        }
    }
    public void SetSliderToggleView(bool isSelect, Product product)
    {
        if (sliderToggle)
        {
            if (isSelect)
            {
                sliderToggle.SetActive(true);
                if (!sliderToggle.GetComponent<SlideToggle>()) return;
                sliderToggle.GetComponent<SlideToggle>().OnToggleChanged(product.Data.isOn, 0f);
            }
            else
            {
                sliderToggle.SetActive(false);
            }
        }
    }

    private void InitializeChartDate()
    {
        DateController dateController = chartCalendarPanel.GetComponent<DateController>();
        if (!dateController) return;

        DateTime startDate = DateTime.Now.AddDays(-6);
        string start = startDate.Year.ToString() + "-" + startDate.Month.ToString() + "-" + startDate.Day.ToString();

        DateTime endDate = DateTime.Now;
        string end = endDate.Year.ToString() + "-" + endDate.Month.ToString() + "-" + endDate.Day.ToString();

        dateController.Initialize(startDate, endDate);
    }

    private void InitializeDoorLockDate()
    {
        DateController dateController = doorLockCalendarPanel.GetComponent<DateController>();
        if (!dateController) return;

        DateTime startDate = DateTime.Now.AddDays(-6);
        string start = startDate.Year.ToString() + "-" + startDate.Month.ToString() + "-" + startDate.Day.ToString();

        DateTime endDate = DateTime.Now;
        string end = endDate.Year.ToString() + "-" + endDate.Month.ToString() + "-" + endDate.Day.ToString();

        dateController.Initialize(startDate, endDate);
        InitDoorLockEntry();
    }

    private bool isPanel()
    {
        return userPanel && homePanel && viewerPanel && chartPanel && solarPowerPanel && aiElecticalPanel && doorLockPanel;
    }

    public void ShowPictrueWidget(Text date, Text time)
    {
        if (pictrueWindow)
        {
            // REST API -> DB에서 해당 날짜/시간의 사진을 가져오는 로직
            string text = date.text + " " + time.text;
            pictrueWindow.GetComponentInChildren<Text>().text = text;
            pictrueWindow.SetActive(true);
            pictrueWindow.GetComponent<Image>().raycastTarget = true;
        }
    }

    public void InitDoorLockEntry()
    {
        if (smartHomeManager.doorLockEntries.Length == 0) return;

        DateController dateController = doorLockCalendarPanel.GetComponent<DateController>();
        if (!dateController) return;

        FindObjectsOfType<DoorLockEntryUI>().ToList().ForEach(entry => Destroy(entry.gameObject));
        for (int i = smartHomeManager.doorLockEntries.Length - 1; i >= 0; i--)
        {
            DoorLockEntry entry = smartHomeManager.doorLockEntries[i];
            Debug.Log(entry.GetAccessDateTime() + " " + dateController.endDateTime);
            if (entry.GetAccessDateTime() < dateController.startDateTime || entry.GetAccessDateTime() > dateController.endDateTime) continue;

            GameObject entryObj = Instantiate(doorLockEntryPrefab, doorLockList.transform);
            DoorLockEntryUI doorLockEntryUI = entryObj.GetComponent<DoorLockEntryUI>();

            doorLockEntryUI.doorLockEntry = entry;
            doorLockEntryUI.dateText.text = entry.date;
            doorLockEntryUI.timeText.text = entry.time;

            entryObj.SetActive(true);
        }
    }
}
