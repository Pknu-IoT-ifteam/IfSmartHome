using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class SlideHomeToggle : MonoBehaviour
{
    [SerializeField] private ProductType productType;

    public Toggle toggle;
    public RectTransform handle; 
    public float slideDistance = 50f;

    Vector3 leftPos, rightPos;

    [Header("Managers")]
    public SmartHomeManager smartHomeManager;

    private List<GameObject> productGameObjects = new List<GameObject>();

    private bool isInitialized = false;

    void Start()
    {
        leftPos = handle.anchoredPosition;
        rightPos = leftPos + Vector3.right * slideDistance;
        toggle.onValueChanged.AddListener((isOn) => { OnToggleChanged(isOn, 0.3f); });
        smartHomeManager = GameObject.Find("SmartHomeManager").GetComponent<SmartHomeManager>();
        isInitialized = false;
        GetProducts();
    }

    private void OnEnable()
    {
        isInitialized = false; 
        GetProducts();
        isInitialized = true;
        Product.OnProductStateChanged += OnProductStateUpdated;
    }

    private void OnDisable()
    {
        Product.OnProductStateChanged -= OnProductStateUpdated;
    }

    void OnDestroy()
    {
        Product.OnProductStateChanged -= OnProductStateUpdated;
    }
    private void OnProductStateUpdated(ProductType changedProductType, bool newState)
    {
        // 현재 토글과 관련된 제품 타입만 처리
        if (changedProductType != productType) return;

        // 홈 화면 토글 상태 업데이트
        StartCoroutine(UpdateNextFrame());
    }
    private IEnumerator UpdateNextFrame()
    {
        yield return null; // 한 프레임 대기
        UpdateToggleState();
    }

    private void UpdateToggleState()
    {
        if (productGameObjects.Count <= 0) return;

        bool newToggleState;

        if (productType == ProductType.Lamp)
        {
            // 모든 램프가 켜져있어야 true
            newToggleState = true;
            foreach (GameObject lampObj in productGameObjects)
            {
                if (!lampObj.GetComponent<Product>().Data.isOn)
                {
                    newToggleState = false;
                    break;
                }
            }
        }
        else
        {
            newToggleState = productGameObjects[0].GetComponent<Product>().Data.isOn;
        }
        
        // 무한 루프 방지: 현재 상태와 다를 때만 업데이트
        if (toggle.isOn != newToggleState || !isInitialized)
        {
            toggle.isOn = newToggleState;
            OnToggleChangedUIOnly(newToggleState, 0f);
            isInitialized = true; 
        }
    }
    public void OnToggleChanged(bool isOn, float animTime)
    {
        UpdateToggleUI(isOn, animTime);

        if (smartHomeManager && productGameObjects.Count > 0)
        {
            for (int i = 0; i < productGameObjects.Count; i++)
            {
                Product product = productGameObjects[i].GetComponent<Product>();
                if (product)
                {
                    product.ChangeIsOn(isOn);
                }
            }
        }
    }

    private void OnToggleChangedUIOnly(bool isOn, float animTime)
    {
        UpdateToggleUI(isOn, animTime);
    }

    private void UpdateToggleUI(bool isOn, float animTime)
    {
        toggle.isOn = isOn;
        Vector3 targetPos = isOn ? rightPos : leftPos;
        GetComponentInChildren<Image>().color = isOn ? Color.green : Color.red;
        if (animTime <= 0f || !gameObject.activeInHierarchy)
        {
            handle.anchoredPosition = targetPos;
        }
        else
        {
            StartCoroutine(SlideTo(targetPos, animTime));
        }
    }

    IEnumerator SlideTo(Vector3 target, float animTime)
    {
        if (animTime <= 0f)
        {
            handle.anchoredPosition = target;
            yield break;
        }
        float time = 0;
        Vector3 start = handle.anchoredPosition;

        while (time < animTime)
        {
            time += Time.deltaTime;
            handle.anchoredPosition = Vector3.Lerp(start, target, time / animTime);
            yield return null;
        }
        handle.anchoredPosition = target; 
    }

    private void GetProducts()
    {
        if (!smartHomeManager) return;

        productGameObjects.Clear(); // 기존 리스트 초기화

        // 1. 제품 필터링 (GetComponent 호출 최소화)
        foreach (GameObject productObj in smartHomeManager.products)
        {
            if (productObj == null) continue;

            Product product = productObj.GetComponent<Product>();
            if (product?.Data == null) continue;

            if (product.Data.productType == productType)
            {
                productGameObjects.Add(productObj);
            }
        }

        if (productGameObjects.Count <= 0) return;

        UpdateToggleState();
    }
}