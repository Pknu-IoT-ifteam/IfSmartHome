using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// 5일 예보 데이터 구조 (3시간 간격)
[System.Serializable]
public class ForecastData
{
    public ForecastList[] list;
}

[System.Serializable]
public class ForecastList
{
    public Main main;
    public Weather[] weather;
    public string dt_txt; // 날짜 시간
}

[System.Serializable]
public class WeatherData
{
    public Weather[] weather;
    public Main main;
    public string name; // 도시명
}

[System.Serializable]
public class Weather
{
    public string main;        // 날씨 주요 정보 (Rain, Snow, Clear 등)
    public string description; // 날씨 상세 설명
    public string icon;        // 날씨 아이콘 ID
}

[System.Serializable]
public class Main
{
    public float temp;      // 현재 온도 (켈빈)
    public float temp_min;  // 최저 온도
    public float temp_max;  // 최고 온도
    public int humidity;    // 습도 (%)
}

public class WeatherManager : MonoBehaviour
{
    [Header("API 설정")]
    public string apiKey = "867503d55e3a4aeb4c44a97a4dbdc46d"; // OpenWeatherMap API 키

    [Header("자동 위치 감지")]
    public bool useCurrentLocation = true;  // 현재 위치 사용 여부
    public bool locationFound = false;      // 위치 감지 완료 여부

    [Header("위치 설정")]
    public string cityName = "Seoul"; // 기본 도시명 (위치 실패시 사용)
    [Header("좌표로 요청")]
    public bool useCoordinates = false;
    public float latitude = 37.5665f;  // 서울 위도
    public float longitude = 126.9780f; // 서울 경도

    [Header("UI 설정")]
    public Text weatherInfoText; // UI Text 컴포넌트

    [Header("하루 최저/최고 온도 가져오기")]
    public bool getDailyMinMax = true;  // 실제 하루 최저/최고 온도 사용
    public WeatherData currentWeather;
    public ForecastData forecastWeather; // 5일 예보 데이터

    private string baseURL = "https://api.openweathermap.org/data/2.5/weather";
    private string forecastURL = "https://api.openweathermap.org/data/2.5/forecast"; // 5일 예보 API (무료)

    [Header("에디터 테스트용")]
    public bool simulateLocationInEditor = true;
    public string testLocationName = "Busan"; // 테스트할 도시명

    void Start()
    {
        // API 키 확인
        //Debug.Log($"현재 설정된 API 키: {apiKey}");
        //Debug.Log($"API 키 길이: {apiKey.Length}");

#if UNITY_EDITOR
        // Unity Editor에서는 GPS 미지원
        if (useCurrentLocation && simulateLocationInEditor)
        {
            //Debug.Log("=== Unity Editor GPS 시뮬레이션 ===");
            //Debug.Log("실제 기기에서는 GPS로 현재 위치를 감지합니다.");
            //Debug.Log($"에디터에서는 '{testLocationName}' 위치로 시뮬레이션합니다.");

            // 테스트 도시 좌표 설정
            SetTestLocation(testLocationName);
            GetWeatherData();
        }
        else if (useCurrentLocation)
        {
            Debug.LogWarning("Unity Editor에서는 GPS를 사용할 수 없습니다.");
            Debug.LogWarning("실제 기기에서 테스트하거나 'Simulate Location In Editor'를 활성화하세요.");
            GetWeatherData(); // 기본 설정으로 실행
        }
        else
        {
            //Debug.Log("설정된 위치로 날씨 정보를 가져옵니다...");
            GetWeatherData();
        }
#else
        // 실제 기기에서는 정상 동작
        if (useCurrentLocation)
        {
            Debug.Log("현재 위치를 감지하여 날씨 정보를 가져옵니다...");
            GetWeatherByCurrentLocation();
        }
        else
        {
            Debug.Log("설정된 위치로 날씨 정보를 가져옵니다...");
            GetWeatherData();
        }
#endif
    }

#if UNITY_EDITOR
    void SetTestLocation(string locationName)
    {
        // 테스트용 도시 좌표 매핑
        var testLocations = new System.Collections.Generic.Dictionary<string, (float lat, float lon, string city)>
        {
            {"busan", (35.1796f, 129.0756f, "Busan")},
            {"seoul", (37.5665f, 126.9780f, "Seoul")},
            {"incheon", (37.4563f, 126.7052f, "Incheon")},
            {"daegu", (35.8714f, 128.6014f, "Daegu")},
            {"tokyo", (35.6762f, 139.6503f, "Tokyo")},
            {"newyork", (40.7128f, -74.0060f, "New York")},
            {"london", (51.5074f, -0.1278f, "London")}
        };

        string key = locationName.ToLower().Replace(" ", "");

        if (testLocations.ContainsKey(key))
        {
            var location = testLocations[key];
            latitude = location.lat;
            longitude = location.lon;
            cityName = location.city;
            useCoordinates = true;
            locationFound = true;

            //Debug.Log($"📍 테스트 위치 설정: {location.city} ({latitude}, {longitude})");
        }
        else
        {
            Debug.LogWarning($"'{locationName}' 테스트 위치를 찾을 수 없습니다. 기본 설정을 사용합니다.");
        }
    }
#endif

    public void GetWeatherData()
    {
        if (getDailyMinMax)
        {
            // 현재 날씨 + 하루 최저/최고 온도 둘 다 가져오기
            StartCoroutine(FetchBothWeatherData());
        }
        else
        {
            // 현재 날씨만 가져오기 (기존 방식)
            StartCoroutine(FetchWeatherData());
        }
    }

    // 현재 날씨 + 하루 최저/최고 온도 모두 가져오기
    IEnumerator FetchBothWeatherData()
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY_HERE")
        {
            Debug.LogError("API 키가 설정되지 않았습니다! Inspector에서 올바른 API 키를 입력해주세요.");
            UpdateWeatherUI("API 키가 설정되지 않았습니다.");
            yield break;
        }

        // 1. 현재 날씨 가져오기
        string currentUrl;
        if (useCoordinates)
        {
            currentUrl = $"{baseURL}?lat={latitude}&lon={longitude}&appid={apiKey}&lang=kr";
        }
        else
        {
            currentUrl = $"{baseURL}?q={cityName}&appid={apiKey}&lang=kr";
        }

        //Debug.Log($"현재 날씨 요청: {currentUrl}");

        using (UnityWebRequest currentRequest = UnityWebRequest.Get(currentUrl))
        {
            yield return currentRequest.SendWebRequest();

            if (currentRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonData = currentRequest.downloadHandler.text;
                currentWeather = JsonUtility.FromJson<WeatherData>(jsonData);
                //Debug.Log("현재 날씨 데이터 가져오기 성공!");
            }
            else
            {
                Debug.LogError($"현재 날씨 요청 실패: {currentRequest.error}");
                yield break;
            }
        }

        // 2. 5일 예보에서 오늘 최저/최고 온도 계산
        string forecastUrl;
        if (useCoordinates)
        {
            forecastUrl = $"{forecastURL}?lat={latitude}&lon={longitude}&appid={apiKey}&lang=kr";
        }
        else
        {
            forecastUrl = $"{forecastURL}?q={cityName}&appid={apiKey}&lang=kr";
        }

        //Debug.Log($"5일 예보 요청: {forecastUrl}");

        using (UnityWebRequest forecastRequest = UnityWebRequest.Get(forecastUrl))
        {
            yield return forecastRequest.SendWebRequest();

            if (forecastRequest.result == UnityWebRequest.Result.Success)
            {
                string forecastJsonData = forecastRequest.downloadHandler.text;
                //Debug.Log("5일 예보 데이터 가져오기 성공!");

                forecastWeather = JsonUtility.FromJson<ForecastData>(forecastJsonData);

                // 두 데이터 모두 성공적으로 가져온 경우 처리
                ProcessCombinedWeatherData();
                //Debug.Log("현재 날씨 + 5일 예보 데이터를 성공적으로 가져왔습니다!");
            }
            else
            {
                Debug.LogError($"5일 예보 요청 실패: {forecastRequest.error}");
                Debug.LogError($"HTTP 응답 코드: {forecastRequest.responseCode}");
                Debug.LogError($"응답 내용: {forecastRequest.downloadHandler.text}");
                Debug.LogWarning("현재 날씨 데이터만 사용합니다.");
                ProcessWeatherData(); // 현재 날씨만 처리
            }
        }
    }

    // 현재 날씨 + 5일 예보에서 오늘 최저/최고 온도 계산
    void ProcessCombinedWeatherData()
    {
        if (currentWeather != null && currentWeather.weather.Length > 0)
        {
            // 현재 온도 (켈빈을 섭씨로 변환)
            float currentTempCelsius = currentWeather.main.temp - 273.15f;

            // 5일 예보에서 오늘 최저/최고 온도 계산
            float dailyMinTemp = currentTempCelsius;
            float dailyMaxTemp = currentTempCelsius;

            if (forecastWeather != null && forecastWeather.list.Length > 0)
            {
                string today = System.DateTime.Now.ToString("yyyy-MM-dd");

                // 오늘 날짜의 모든 예보 데이터에서 최저/최고 찾기
                foreach (var forecast in forecastWeather.list)
                {
                    if (forecast.dt_txt.StartsWith(today))
                    {
                        float temp = forecast.main.temp - 273.15f;
                        if (temp < dailyMinTemp) dailyMinTemp = temp;
                        if (temp > dailyMaxTemp) dailyMaxTemp = temp;
                    }
                }

                //Debug.Log($"오늘 예보 데이터에서 최저/최고 온도 계산 완료");
            }
            else
            {
                // 예보 API 실패시 현재 온도 기반으로 추정
                dailyMinTemp = currentWeather.main.temp_min - 273.15f;
                dailyMaxTemp = currentWeather.main.temp_max - 273.15f;
                Debug.LogWarning("예보 데이터가 없어 현재 온도 기반으로 추정합니다.");
            }

            //Debug.Log("=== 완전한 날씨 정보 ===");
            //Debug.Log($"위치: {currentWeather.name}");
            //Debug.Log($"현재 온도: {currentTempCelsius:F1}°C");
            //Debug.Log($"오늘 최고: {dailyMaxTemp:F1}°C");
            //Debug.Log($"오늘 최저: {dailyMinTemp:F1}°C");
            //Debug.Log($"날씨: {currentWeather.weather[0].description}");
            //Debug.Log($"습도: {currentWeather.main.humidity}%");

            // UI 업데이트
            string weatherInfo = $"위치: {currentWeather.name}\n" +
                               $"현재 온도: {currentTempCelsius:F1}°C\n" +
                               $"오늘 최고: {dailyMaxTemp:F1}°C\n" +
                               $"오늘 최저: {dailyMinTemp:F1}°C\n" +
                               $"날씨: {currentWeather.weather[0].description}\n" +
                               $"습도: {currentWeather.main.humidity}%";

            UpdateWeatherUI(weatherInfo);

            if (locationFound)
            {
                //Debug.Log($"GPS 위치: 위도 {latitude:F4}, 경도 {longitude:F4}");
            }

            // 날씨에 따른 게임 로직 처리
            HandleWeatherEffects(currentWeather.weather[0].main, currentTempCelsius);
        }
    }

    void HandleWeatherEffects(string weatherType, float temperature)
    {
        // 날씨에 따른 게임 효과 적용 예시
        switch (weatherType.ToLower())
        {
            case "rain":
                //Debug.Log("비가 와서 우산 효과 적용!");
                // 비 파티클 효과, 캐릭터 이동속도 감소 등
                break;

            case "snow":
                //Debug.Log("눈이 와서 미끄러짐 효과 적용!");
                // 눈 파티클 효과, 캐릭터 미끄러짐 등
                break;

            case "clear":
                //Debug.Log("맑은 날씨로 기본 상태!");
                // 기본 상태
                break;

            case "clouds":
                //Debug.Log("흐린 날씨로 어두운 효과 적용!");
                // 조명 어둡게 등
                break;
        }

        // 온도에 따른 효과
        if (temperature < 0)
        {
            //Debug.Log("영하로 얼음 효과 적용!");
        }
        else if (temperature > 30)
        {
            //Debug.Log("고온으로 더위 효과 적용!");
        }
    }

    // 특정 도시 날씨 가져오기 (좌표 사용)
    public void GetWeatherByCity(string city)
    {
        // 주요 도시 좌표 매핑
        var cityCoordinates = new System.Collections.Generic.Dictionary<string, (float lat, float lon)>
        {
            {"seoul", (37.5665f, 126.9780f)},
            {"busan", (35.1796f, 129.0756f)},
            {"incheon", (37.4563f, 126.7052f)},
            {"daegu", (35.8714f, 128.6014f)},
            {"daejeon", (36.3504f, 127.3845f)},
            {"gwangju", (35.1595f, 126.8526f)},
            {"ulsan", (35.5384f, 129.3114f)},
            {"tokyo", (35.6762f, 139.6503f)},
            {"osaka", (34.6937f, 135.5023f)},
            {"beijing", (39.9042f, 116.4074f)},
            {"shanghai", (31.2304f, 121.4737f)},
            {"newyork", (40.7128f, -74.0060f)},
            {"london", (51.5074f, -0.1278f)},
            {"paris", (48.8566f, 2.3522f)}
        };

        string cityKey = city.ToLower().Replace(" ", "");

        if (cityCoordinates.ContainsKey(cityKey))
        {
            var coords = cityCoordinates[cityKey];
            latitude = coords.lat;
            longitude = coords.lon;
            useCoordinates = true;

            Debug.Log($"{city} 좌표로 날씨 요청: {latitude}, {longitude}");
            GetWeatherData();
        }
        else
        {
            // 좌표가 없으면 도시명으로 시도
            cityName = city;
            useCoordinates = false;
            Debug.Log($"{city} 도시명으로 날씨 요청");
            GetWeatherData();
        }
    }

    // 기존 방식 (현재 날씨만) - 호환성을 위해 유지
    IEnumerator FetchWeatherData()
    {
        // API 키 유효성 검사
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY_HERE")
        {
            Debug.LogError("API 키가 설정되지 않았습니다! Inspector에서 올바른 API 키를 입력해주세요.");
            yield break;
        }

        // API URL 구성
        string url;
        if (useCoordinates)
        {
            url = $"{baseURL}?lat={latitude}&lon={longitude}&appid={apiKey}&lang=kr";
            Debug.Log($"좌표로 요청: 위도 {latitude}, 경도 {longitude}");
        }
        else
        {
            url = $"{baseURL}?q={cityName}&appid={apiKey}&lang=kr";
            Debug.Log($"도시명으로 요청: {cityName}");
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonData = request.downloadHandler.text;
                currentWeather = JsonUtility.FromJson<WeatherData>(jsonData);
                ProcessWeatherData();
                Debug.Log("현재 날씨 데이터를 가져왔습니다!");
            }
            else
            {
                Debug.LogError($"날씨 데이터 가져오기 실패: {request.error}");
            }
        }
    }

    // 현재 날씨만 처리 (기존 방식)
    void ProcessWeatherData()
    {
        if (currentWeather != null && currentWeather.weather.Length > 0)
        {
            float tempCelsius = currentWeather.main.temp - 273.15f;

            Debug.Log("=== 현재 날씨 정보 ===");
            Debug.Log($"위치: {currentWeather.name}");
            Debug.Log($"현재 온도: {tempCelsius:F1}°C");
            Debug.Log($"날씨: {currentWeather.weather[0].description}");
            Debug.Log($"습도: {currentWeather.main.humidity}%");
            Debug.Log("실제 최저/최고 온도를 보려면 'Get Daily Min Max'를 체크하세요!");

            // UI 업데이트
            string weatherInfo = $"위치: {currentWeather.name}\n" +
                               $"현재 온도: {tempCelsius:F1}°C\n" +
                               $"날씨: {currentWeather.weather[0].description}\n" +
                               $"습도: {currentWeather.main.humidity}%\n" +
                               $"(실제 최저/최고는 Daily Min Max 체크)";

            UpdateWeatherUI(weatherInfo);

            HandleWeatherEffects(currentWeather.weather[0].main, tempCelsius);
        }
    }
    public void GetWeatherByCurrentLocation()
    {
        StartCoroutine(GetLocationAndWeather());
    }

    IEnumerator GetLocationAndWeather()
    {
        // 위치 서비스 권한 확인 및 요청
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("위치 서비스가 비활성화되어 있습니다.");

#if UNITY_ANDROID && !UNITY_EDITOR
            // Android에서 위치 권한 요청
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
            {
                Debug.Log("Android 위치 권한을 요청합니다...");
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
                
                // 권한 요청 후 잠시 대기
                yield return new WaitForSeconds(2f);
                
                if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
                {
                    Debug.LogError("사용자가 위치 권한을 거부했습니다.");
                    ShowLocationPermissionDialog();
                    GetWeatherData(); // 기본 위치로 대체
                    yield break;
                }
                else
                {
                    Debug.Log("Android 위치 권한이 승인되었습니다!");
                }
            }
#endif

#if UNITY_IOS && !UNITY_EDITOR
            Debug.Log("iOS에서는 설정 > 개인정보 보호 > 위치 서비스에서 앱 권한을 허용해주세요.");
#endif

            // 권한이 여전히 없으면 기본 위치 사용
            if (!Input.location.isEnabledByUser)
            {
                Debug.LogError("위치 권한이 여전히 비활성화되어 있습니다.");
                //ShowLocationPermissionDialog();
                GetWeatherData(); // 기본 위치로 대체
                yield break;
            }
        }

        //Debug.Log("GPS 위치 정보를 가져오는 중...");

        // 위치 서비스 시작
        Input.location.Start(1f, 1f); // 정확도 1m, 최소 이동거리 1m

        // 위치 정보 대기 (최대 20초)
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
            Debug.Log($"위치 감지 중... ({21 - maxWait}/20초)");
        }

        // 시간 초과
        if (maxWait < 1)
        {
            Debug.LogError("위치 정보 가져오기 시간 초과 (20초)");
            Debug.LogWarning("기본 위치로 날씨를 가져옵니다.");
            Input.location.Stop();
            GetWeatherData();
            yield break;
        }

        // 위치 서비스 실패
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("위치 정보 가져오기 실패");
            Debug.LogWarning("기본 위치로 날씨를 가져옵니다.");
            Input.location.Stop();
            GetWeatherData();
            yield break;
        }

        // 위치 정보 성공적으로 획득
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        float accuracy = Input.location.lastData.horizontalAccuracy;

        //Debug.Log($"현재 위치 감지 완료!");
        //Debug.Log($"위도: {latitude}");
        //Debug.Log($"경도: {longitude}");
        //Debug.Log($"정확도: {accuracy}m");

        locationFound = true;
        useCoordinates = true; // 좌표 모드로 전환

        // 현재 위치의 날씨 정보 요청
        string url = $"{baseURL}?lat={latitude}&lon={longitude}&appid={apiKey}&lang=kr";

        //Debug.Log($"현재 위치 기반 날씨 요청: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonData = request.downloadHandler.text;
                //Debug.Log($"응답 데이터: {jsonData}");

                currentWeather = JsonUtility.FromJson<WeatherData>(jsonData);
                ProcessWeatherData();

                //Debug.Log("현재 위치 기반 날씨 데이터를 성공적으로 가져왔습니다!");
            }
            else
            {
                Debug.LogError($"현재 위치 날씨 가져오기 실패: {request.error}");
                Debug.LogError($"HTTP 응답 코드: {request.responseCode}");
                Debug.LogError($"응답 내용: {request.downloadHandler.text}");

                UpdateWeatherUI("현재 위치 날씨 정보를 가져올 수 없습니다.");

                // API 실패시 기본 위치로 재시도
                Debug.LogWarning("기본 위치로 재시도합니다.");
                useCoordinates = false;
                GetWeatherData();
            }
        }

        // 위치 서비스 중지
        Input.location.Stop();
    }

    // UI 텍스트 업데이트
    void UpdateWeatherUI(string weatherInfo)
    {
        if (weatherInfoText != null)
        {
            weatherInfoText.text = weatherInfo;
        }
        else
        {
            // Debug.LogWarning("Weather Info Text UI가 설정되지 않았습니다. Inspector에서 Text 컴포넌트를 할당해주세요.");
        }
    }

    void ShowLocationPermissionDialog()
    {
        Debug.LogWarning("=== 위치 권한 설정 안내 ===");
        Debug.LogWarning("현재 위치의 날씨를 보려면 위치 권한이 필요합니다.");
        Debug.LogWarning("");
        Debug.LogWarning("Android: 설정 > 앱 > [앱이름] > 권한 > 위치");
        Debug.LogWarning("iOS: 설정 > 개인정보 보호 > 위치 서비스 > [앱이름]");
        Debug.LogWarning("");
        Debug.LogWarning("권한 설정 후 앱을 다시 시작해주세요.");
        Debug.LogWarning($"현재는 기본 위치({cityName})의 날씨를 보여드립니다.");
    }
}