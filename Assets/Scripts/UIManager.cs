using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject homePanel;
    public GameObject viewerPanel;
    public GameObject chartPanel;
    public GameObject userPanel;
    public GameObject settingsPanel;

    [Header("Widget")]
    public GameObject sliderToggle;
    public GameObject calendarPanel;

    private void Start()
    {
        if (homePanel && viewerPanel && chartPanel && userPanel && settingsPanel && calendarPanel)
        {
            ShowHomePanel();
            InitializeChartDate();
        }
    }
    public void ShowHomePanel()
    {
        if (homePanel && viewerPanel && chartPanel && userPanel && settingsPanel)
        {
            homePanel.SetActive(true);
            viewerPanel.SetActive(false);
            chartPanel.SetActive(false);
            userPanel.SetActive(false);
            settingsPanel.SetActive(false);
            sliderToggle.SetActive(false);

            calendarPanel.SetActive(false);
        }
    }

    public void ShowViewerPanel()
    {
        if (homePanel && viewerPanel && chartPanel && userPanel && settingsPanel)
        {
            homePanel.SetActive(false);
            viewerPanel.SetActive(true);
            chartPanel.SetActive(false);
            userPanel.SetActive(false);
            settingsPanel.SetActive(false);
            sliderToggle.SetActive(false);

            calendarPanel.SetActive(false);

            viewerPanel.GetComponent<Image>().raycastTarget = false;
        }
    }

    public void ShowChartPanel()
    {
        if (homePanel && viewerPanel && chartPanel && userPanel && settingsPanel)
        {
            homePanel.SetActive(false);
            viewerPanel.SetActive(false);
            chartPanel.SetActive(true);
            userPanel.SetActive(false);
            settingsPanel.SetActive(false);
            sliderToggle.SetActive(false);

            calendarPanel.SetActive(false);
        }
    }

    public void ShowUserPanel()
    {
        if (homePanel && viewerPanel && chartPanel && userPanel && settingsPanel)
        {
            homePanel.SetActive(false);
            viewerPanel.SetActive(false);
            chartPanel.SetActive(false);
            userPanel.SetActive(true);
            settingsPanel.SetActive(false);
            sliderToggle.SetActive(false);

            calendarPanel.SetActive(false);
        }
    }

    public void ShowSettingsPanel()
    {
        if (homePanel && viewerPanel && chartPanel && userPanel && settingsPanel)
        {
            homePanel.SetActive(false);
            viewerPanel.SetActive(false);
            chartPanel.SetActive(false);
            userPanel.SetActive(false);
            settingsPanel.SetActive(true);
            sliderToggle.SetActive(false);

            calendarPanel.SetActive(false);
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
        ChartDateController charDateController = calendarPanel.GetComponent<ChartDateController>();
        if (!charDateController) return;

        DateTime startDate = DateTime.Now;
        string start = startDate.Year.ToString() + "-" + startDate.Month.ToString() + "-" + startDate.Day.ToString();

        DateTime endDate = startDate.AddDays(7);
        string end = endDate.Year.ToString() + "-" + endDate.Month.ToString() + "-" + endDate.Day.ToString();

        charDateController.Initialize(startDate, endDate);
    }
}
