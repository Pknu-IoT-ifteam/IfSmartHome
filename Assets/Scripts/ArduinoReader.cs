using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoReader : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM5", 9600); // 아두이노 포트와 보드레이트 맞게 설정

    void Start()
    {
        serialPort.Open();
        serialPort.ReadTimeout = 100;
    }
    void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine(); // 한 줄 읽기
                Debug.Log("받은 데이터: " + data);

                // 데이터 파싱 (예: "25,60")
                string[] values = data.Split(',');
                if (values.Length == 2)
                {
                    int temp = int.Parse(values[0]);
                    int hum = int.Parse(values[1]);
                    // temp, hum 값을 유니티에서 원하는 대로 사용
                }
            }
            catch (System.Exception) { }
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}

