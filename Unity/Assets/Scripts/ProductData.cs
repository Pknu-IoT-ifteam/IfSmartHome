using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "ProductData", menuName = "ScriptableObjects/ProductData")]
public class ProductData : ScriptableObject
{
    [Header("Product Information")]
    public int productId;
    public string productName;
    public string productDescription;
    public ProductType productType;

    [Header("Product State")]
    public bool isOn;
    [ShowIf("ShowBrightness")]
    public float brightness;
    [ShowIf("showSoundVolume")]
    public AudioSource sound;
    private bool ShowBrightness()
    {
        return productType == ProductType.Computer ||
               productType == ProductType.Lamp ||
               productType == ProductType.MicrowaveOven ||
               productType == ProductType.Television;
    }

    private bool showSoundVolume()
    {
        return productType == ProductType.AirConditioner ||
               productType == ProductType.Cleaner ||
               productType == ProductType.ElectricFan ||
               productType == ProductType.Refrigerator ||
               productType == ProductType.WashingMachine;
    }
}   

public enum ProductType
{
    ElectricFan, // 선풍기
    AirConditioner, // 에어컨
    WashingMachine, // 세탁기
    Refrigerator, // 냉장고
    Computer, // 컴퓨터
    MicrowaveOven, // 전자레인지
    Lamp, // 전등빛
    Television, // TV
    Cleaner // 청소기
}