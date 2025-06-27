using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "ProductData", menuName = "ScriptableObjects/ProductData")]
public class ProductData : ScriptableObject
{
    [Header("Product Information")]
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
               productType == ProductType.Light ||
               productType == ProductType.MicrowaveOven ||
               productType == ProductType.Television;
    }

    private bool showSoundVolume()
    {
        return productType == ProductType.AirConditioner ||
               productType == ProductType.Cleaner ||
               productType == ProductType.ElectricFan ||
               productType == ProductType.Humidifier ||
               productType == ProductType.Refrigerator ||
               productType == ProductType.WashingMachine;
    }
}   

public enum ProductType
{
    AirConditioner, // ������
    Cleaner, // û�ұ�
    Computer, // ��ǻ��
    ElectricFan, // ��ǳ��
    Humidifier, // ������
    Lamp, // ����
    Light, // �����
    MicrowaveOven, // ���ڷ�����
    Refrigerator, // �����
    Television, // TV
    WashingMachine // ��Ź��
}