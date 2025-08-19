using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class SlideToggle : MonoBehaviour
{
    public Toggle toggle;
    public RectTransform handle; 
    public float slideDistance = 50f;

    Vector3 leftPos, rightPos;

    [Header("Managers")]
    public SmartHomeManager smartHomeManager;

    private bool isUpdatingFromExternal = false;

    void Start()
    {
        leftPos = handle.anchoredPosition;
        rightPos = leftPos + Vector3.right * slideDistance;

        toggle.onValueChanged.AddListener((isOn) => { OnToggleChanged(isOn, 0.3f); });

        smartHomeManager = GameObject.Find("SmartHomeManager").GetComponent<SmartHomeManager>();
    }

    public void OnToggleChanged(bool isOn, float animTime)
    {
        if (isUpdatingFromExternal) return; // 외부에서 업데이트 중이면 중단

        toggle.isOn = isOn;
        Vector3 targetPos = isOn ? rightPos : leftPos;
        GetComponentInChildren<Image>().color = isOn ? Color.green : Color.red;
        if (animTime <= 0f || !gameObject.activeInHierarchy || !smartHomeManager.isCalled)
        {
            handle.anchoredPosition = targetPos;
            smartHomeManager.isCalled = true;
        }
        else
        {
            StartCoroutine(SlideTo(targetPos, animTime));
        }

        if (smartHomeManager && smartHomeManager.selectProduct)
        {
            smartHomeManager.selectProduct.ChangeIsOn(isOn);
        }
    }

    IEnumerator SlideTo(Vector3 target, float animTime)
    {
        float time = 0;
        Vector3 start = handle.anchoredPosition;

        while (time < animTime)
        {
            time += Time.deltaTime;
            handle.anchoredPosition = Vector3.Lerp(start, target, time / animTime);
            yield return null;
        }
    }
}