using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using UnityEngine;

public class ArduinoReader : MonoBehaviour
{
    public static int latestTemp { get; private set; }
    public static int latestHum { get; private set; }
    public static bool relayOnTem { get; private set; }
    public static bool relayOnMotor { get; private set; }
    public static bool relayOnLed { get; private set; }



    private string latestLine = null;
    StringBuilder buffer = new StringBuilder();

    static public string portName = "COM5";
    static public int baudRate = 9600;

    SerialPort serialPort = new SerialPort(portName, baudRate); // 아두이노 포트와 보드레이트 맞게 설정

    void Start()
    {
        serialPort.ReadTimeout = 100;

        try
        {
            serialPort.Open();
            Debug.Log("시리얼 포트 열림");
        }
        catch (Exception e)
        {
            Debug.LogError("포트 열기 실패: " + e.Message);
        }
    }
    void Update()
    {
        if (serialPort == null || !serialPort.IsOpen)
        {
            // 포트가 열려있지 않으면 더 이상 진행하지 말기
            return;
        }

        try
        {
            if (serialPort.BytesToRead > 0)
            {
                string incomingData = serialPort.ReadExisting();
                buffer.Append(incomingData);

                // 줄바꿈문자 기준으로 라인을 분리
                string[] lines = buffer.ToString().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                // 마지막 element는 아직 완전하지 않은 라인일 수 있으니 다시 버퍼에 저장
                for (int i = 0; i < lines.Length - 1; i++)
                {
                    ProcessLine(lines[i]);
                }

                buffer.Clear();
                // 아직 끝나지 않은 마지막 라인은 버퍼에 다시 넣음
                buffer.Append(lines[lines.Length - 1]);
            }
        }
        catch (TimeoutException)
        {
            return;  // 읽을 데이터 없으면 바로 반환
        }
        catch (Exception e)
        {
            Debug.LogError($"Serial read error: {e.Message}");
            return;
        }
        if (string.IsNullOrEmpty(latestLine))
            return;

        ProcessLine(latestLine);
        latestLine = null;

    }

    private void ProcessLine(string line)
    {
        // 릴레이 상태 파싱 (한국어+영어 메시지 모두 처리)
        if (line.Contains("RelayTem"))
        {
            relayOnTem = line.Contains("ON") || line.Contains("켜짐");
            Debug.Log($"온습도 릴레이: {(relayOnTem ? "ON" : "OFF")}");
        }
        else if (line.Contains("RelayMotor"))
        {
            relayOnMotor = line.Contains("ON") || line.Contains("켜짐");
            Debug.Log($"모터 릴레이: {(relayOnMotor ? "ON" : "OFF")}");
        }
        else if (line.Contains("RelayLED"))
        {
            relayOnLed = line.Contains("ON") || line.Contains("켜짐");
            Debug.Log($"LED 릴레이: {(relayOnLed ? "ON" : "OFF")}");
        }
        // 온도 파싱 (다양한 형식 대응)
        else if (line.Contains("Temperature") || line.Contains("온도"))
        {
            ParseTemperature(line);
        }
        // 습도 파싱 (다양한 형식 대응)
        else if (line.Contains("Humidity") || line.Contains("습도"))
        {
            ParseHumidity(line);
        }
        // 모터 동작 상태 파싱
        else if (line.Contains("모터 회전") || line.Contains("Motor Active"))
        {
            Debug.Log("모터 작동 중");
        }
    }

    private void ParseTemperature(string line)
    {
        // 다양한 형식 지원:
        // "Temperature : 25C"
        // "온도: 25℃"
        // "Temp=25"
        string numStr = line
            .Replace("Temperature", "")
            .Replace("온도", "")
            .Replace("Temp", "")
            .Replace(":", "")
            .Replace("=", "")
            .Replace("C", "")
            .Replace("℃", "")
            .Trim();

        if (int.TryParse(numStr, out int temp))
        {
            latestTemp = temp;
            Debug.Log($"파싱된 온도 = {temp}℃");
        }
    }

    private void ParseHumidity(string line)
    {
        // 다양한 형식 지원:
        // "Humidity : 50%"
        // "습도: 50%"
        // "Hum=50"
        string numStr = line
            .Replace("Humidity", "")
            .Replace("습도", "")
            .Replace("Hum", "")
            .Replace(":", "")
            .Replace("=", "")
            .Replace("%", "")
            .Trim();

        if (int.TryParse(numStr, out int hum))
        {
            latestHum = hum;
            Debug.Log($"파싱된 습도 = {hum}%");
        }
    }

    public void SetIsOn(ProductType type, bool isOn)
    {
        string cmd = GetCommand(type, isOn);
        if (!string.IsNullOrEmpty(cmd) && serialPort.IsOpen)
        {
            serialPort.WriteLine(cmd);
            Debug.Log($"{type} {(isOn ? "ON" : "OFF")} 명령 전송: {cmd}");
        }
    }

    private string GetCommand(ProductType type, bool isOn)
    {
        switch (type)
        {
            case ProductType.Lamp: return isOn ? "6" : "5";
            case ProductType.AirConditioner: return isOn ? "4" : "3";
            case ProductType.Television: return isOn ? "2" : "1";
            // ... 추가 제품 타입
            default: return null;
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}