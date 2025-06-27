using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [Header("Text")]
    public GameObject homeTemp;
    public GameObject homeTemp2;

    private void Update()
    {
        SetTempText("ddd", "ddd");
    }
    private void SetTempText(string temp, string temp2)
    {
        if (!homeTemp || !homeTemp2) return;
        homeTemp.GetComponent<Text>().text = temp;
        homeTemp2.GetComponent<Text>().text = temp2;   
    }

}
