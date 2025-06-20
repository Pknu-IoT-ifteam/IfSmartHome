using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideMenuManager : MonoBehaviour
{
    [Header("Manager")]
    public GameObject UIManager;

    [Header("Buttons")]
    public Button homeButton;
    public Button modelButton;
    public Button chartButton;
    public Button userButton;
    public Button settingsButton;

    private UIManager uiManager;
    void Start()
    {
        uiManager = UIManager.GetComponent<UIManager>();
        if (!uiManager) return;

        homeButton.onClick.AddListener(() => uiManager.ShowHomePanel());
        modelButton.onClick.AddListener(() => uiManager.ShowModelPanel());
        chartButton.onClick.AddListener(() => uiManager.ShowChartPanel());
        userButton.onClick.AddListener(() => uiManager.ShowUserPanel());
        settingsButton.onClick.AddListener(() => uiManager.ShowSettingsPanel());
    }

    void Update()
    {
        
    }
}
