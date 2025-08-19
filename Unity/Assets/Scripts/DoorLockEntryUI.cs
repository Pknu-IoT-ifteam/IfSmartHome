using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorLockEntryUI : MonoBehaviour
{
    public Button button;
    public DoorLockEntry doorLockEntry;

    [Header("UI Elements")]
    public Text dateText;
    public Text timeText;
    private UIManager uiManager;

    private SmartHomeManager smartHomeManager;

    private void Start()
    {
        if (button)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        smartHomeManager = FindObjectOfType<SmartHomeManager>();
    }

    private void OnButtonClicked()
    {
        if (!uiManager)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        uiManager.ShowPictrueWidget(dateText, timeText);
        if (LoadImageFromBase64())
        {
            uiManager.cctvImage.sprite = LoadImageFromBase64();
        }
    }

    public Sprite LoadImageFromBase64()
    {
        try
        {
            byte[] imageBytes = Convert.FromBase64String(doorLockEntry.image);
            Texture2D texture = new Texture2D(2, 2);

            if (texture.LoadImage(imageBytes))
            {
                Sprite sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                return sprite;
            }
            else
            {
                return null;
            }
        }
        catch (System.Exception e)
        {
            return null;
        }
    }
}

[System.Serializable]
public class DoorLockEntry
{
    public int id;
    public string image;
    public string date;
    public string time;

    public DoorLockEntry(int id, string date, string time, string image = "")
    {
        this.id = id;
        this.date = date;
        this.time = time;
        this.image = image;
    }
    public DateTime GetAccessDateTime()
    {
        if (DateTime.TryParse($"{date} {time}", out DateTime accessDateTime))
        {
            return accessDateTime;
        }
        return DateTime.MinValue;
    }
}


