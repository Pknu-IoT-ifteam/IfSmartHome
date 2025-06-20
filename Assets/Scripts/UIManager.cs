using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject homePanel;
    public GameObject modelPanel;
    public GameObject ChartPanel;
    public GameObject userPanel;
    public GameObject settingsPanel;

    private void Start()
    {
        if (homePanel && modelPanel && userPanel && settingsPanel)
        {
            ShowHomePanel();
        }
    }
    public void ShowHomePanel()
    {
        if (homePanel && modelPanel && userPanel && settingsPanel)
        {
            homePanel.SetActive(true);
            modelPanel.SetActive(false);
            ChartPanel.SetActive(false);
            userPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
    }

    public void ShowModelPanel()
    {
        if (homePanel && modelPanel && userPanel && settingsPanel)
        {
            homePanel.SetActive(false);
            modelPanel.SetActive(true);
            ChartPanel.SetActive(false);
            userPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
    }

    public void ShowChartPanel()
    {
        if (homePanel && modelPanel && userPanel && settingsPanel)
        {
            homePanel.SetActive(false);
            modelPanel.SetActive(true);
            ChartPanel.SetActive(true);
            userPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
    }

    public void ShowUserPanel()
    {
        if (homePanel && modelPanel && userPanel && settingsPanel)
        {
            homePanel.SetActive(false);
            modelPanel.SetActive(false);
            ChartPanel.SetActive(false);
            userPanel.SetActive(true);
            settingsPanel.SetActive(false);
        }
    }

    public void ShowSettingsPanel()
    {
        if (homePanel && modelPanel && userPanel && settingsPanel)
        {
            homePanel.SetActive(false);
            modelPanel.SetActive(false);
            ChartPanel.SetActive(false);
            userPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }
    }
}
