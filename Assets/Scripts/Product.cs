using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Product : MonoBehaviour
{
    [SerializeField] private ProductData productDataTemplate;
    public GameObject lampLight;

    private Outline outline;
    private ProductData productData;

    public static event System.Action<ProductType, bool> OnProductStateChanged;

    public ProductData Data => productData;
    void Awake()
    {
        productData = Instantiate(productDataTemplate);
        outline = GetComponent<Outline>();
        if (!outline) return;
        outline.enabled = true;
        ChangeIsOn(productData.isOn);
    }

    void Update()
    {
        
    }

    public void ChangeIsOn(bool isTurn)
    {
        productData.isOn = isTurn;
        ChangeColorOfOutline(productData.isOn);
        if (lampLight)
        {
            if (productData.productType == ProductType.Lamp || productData.productType == ProductType.Television || productData.productType == ProductType.Computer
                || productData.productType == ProductType.MicrowaveOven)
            {
                TurnOffLamp(isTurn);
            }
        }
        OnProductStateChanged?.Invoke(productData.productType, isTurn);
    }
    public void ChangeColorOfOutline(bool isTurn)
    {
        if (isTurn)
        {
            outline.OutlineColor = new Color(0f, 1f, 0f, 1f); 
        }
        else
        {
            outline.OutlineColor = Color.red;
        }
    }

    public void TurnOffLamp(bool isTurn)
    {
        if (isTurn)
        {
            lampLight.GetComponent<Light>().intensity = 1f;
        }
        else
        {
            lampLight.GetComponent<Light>().intensity = 0f;
        }
    }
}
