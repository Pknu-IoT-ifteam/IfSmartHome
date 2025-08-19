//using MySql.Data.MySqlClient;
using System;
using UnityEngine;
using MySqlConnector;


public class RaspiReader : MonoBehaviour
{
    private string connectionString = "server=192.168.0.6;port=3306;database=SmartHome;uid=myuser;pwd=1234;CharSet=utf8;";

    void Start()
    {
        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                Debug.Log("DB Connection Successful");

                string query = "SELECT * FROM sensor_test;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteScalar();
                if (result != null && !(result is DBNull))
                    //Debug.Log(result.ToString());
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())  // 한 행씩 반복
                        {
                            int fieldCount = reader.FieldCount;  // 컬럼 개수
                            string row = "";
                            for (int i = 0; i < fieldCount; i++)
                            {
                                object value = reader.GetValue(i);
                                row += value.ToString() + "\t";  // 탭으로 구분하여 문자열 구성
                            }
                            Debug.Log(row);  // 한 행 출력
                        }
                    }
                else
                    Debug.LogWarning("DB에 값이 없거나 쿼리 결과가 null입니다.");

                conn.Close();

                //conn.Open();
                //Debug.Log("DB Connection Successful");
                //conn.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("DB Connection Error: " + ex.Message);
            Debug.LogException(ex);
        }
    }

    void Update()
    {

    }
}
