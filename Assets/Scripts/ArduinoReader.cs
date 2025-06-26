using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoReader : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM5", 9600); // �Ƶ��̳� ��Ʈ�� ���巹��Ʈ �°� ����

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
                string data = serialPort.ReadLine(); // �� �� �б�
                Debug.Log("���� ������: " + data);

                // ������ �Ľ� (��: "25,60")
                string[] values = data.Split(',');
                if (values.Length == 2)
                {
                    int temp = int.Parse(values[0]);
                    int hum = int.Parse(values[1]);
                    // temp, hum ���� ����Ƽ���� ���ϴ� ��� ���
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

