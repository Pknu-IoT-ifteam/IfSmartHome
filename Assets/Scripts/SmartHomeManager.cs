using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SmartHomeManager : MonoBehaviour
{
    public GameObject[] products;

    public ProductData[] productDatas;

    [Header("Camera")]
    public Camera camera;

    [Header("Managers")]
    public GameObject UIManager;

    [HideInInspector] public Product selectProduct;
    private UIManager uiManager;

    [HideInInspector] public bool isCalled = false;
    void Start()
    {
        GetProductDataList();
        if (UIManager)
        {
            uiManager = UIManager.GetComponent<UIManager>();
        }
    }
    private void Update()
    {
        ClickProduct();
    }
    private void GetProductDataList()
    {
        productDatas = new ProductData[products.Length];
        for (int i = 0; i < products.Length; i++)
        {
            productDatas[i] = products[i].GetComponent<Product>().Data;
        }
    }
    
    private void ClickProduct()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            isCalled = false;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                selectProduct = hit.collider.GetComponent<Product>();
                if (selectProduct)
                {
                    Debug.Log($"선택한 가구 : {selectProduct} | 현재 상태 : {selectProduct.Data.isOn}");
                    uiManager.SetSliderToggleView(true, selectProduct);
                    return;
                }
            }
            uiManager.SetSliderToggleView(false, null);
        }
    }
}
