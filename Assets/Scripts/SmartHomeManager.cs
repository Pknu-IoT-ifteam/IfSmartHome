using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmartHomeManager : MonoBehaviour
{
    public GameObject[] products;

    public ProductData[] productDatas;
    void Start()
    {
        GetProductDataList();
    }
    private void GetProductDataList()
    {
        productDatas = new ProductData[products.Length];
        for (int i = 0; i < products.Length; i++)
        {
            productDatas[i] = products[i].GetComponent<Product>().productData;
        }
    }
}
