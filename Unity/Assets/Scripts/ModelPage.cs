using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ModelPage : MonoBehaviour
{
    [Header("Lights")]
    public GameObject[] lights;

    [Header("UI Elements")]
    public GameObject productList;
    public GameObject productToggle;

    [Header("Manager")]
    public GameObject SmartHomeManager;

    private SmartHomeManager smartHomeManager;

    private void Start()
    {
        if (!SmartHomeManager) return;

        smartHomeManager = SmartHomeManager.GetComponent<SmartHomeManager>();
        InitalizeModelUI();
    }

    public void InitalizeModelUI()
    {
        if (!productList || !productToggle) return;
        ProductData[] productDatas = smartHomeManager.productDatas;
        for (int i = 0; i < productDatas.Length; i++)
        {
            GameObject product = Instantiate(productToggle, productList.transform);
            product.GetComponentInChildren<Text>().text = smartHomeManager.productDatas[i].productName;
            product.GetComponent<Toggle>().isOn = smartHomeManager.productDatas[i].isOn;
            Product productScript = smartHomeManager.products[i].GetComponent<Product>();
            product.GetComponent<Toggle>().onValueChanged.AddListener((isOn) => productScript.ChangeIsOn(isOn));
        }
    }
}
