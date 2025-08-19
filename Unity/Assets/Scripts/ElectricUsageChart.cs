using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCharts.Runtime;

public class ElecticUsageChart : MonoBehaviour
{
    public LineChart chart;
    public SmartHomeManager smartHomeManager;
    public void ShowUsageChart(DateTime startDate, DateTime endDate)
    {
        chart.ClearData();

        var serie = chart.GetSerie(0);
        if (serie != null)
        {
            serie.symbol.show = true;
            serie.symbol.size = 3f;
            serie.symbol.type = SymbolType.Circle;
            serie.lineStyle.show = true;
            serie.lineStyle.width = 2f;
        }

        List<float> usageValues = new List<float>();
        int dayCount = (endDate - startDate).Days + 1;

        // 먼저 모든 데이터 수집
        for (int i = 0; i < dayCount; i++)
        {
            DateTime currentDate = startDate.AddDays(i);
            chart.AddXAxisData(currentDate.ToString("MM/dd"));

            var dayData = smartHomeManager.electricUsageData
                .FirstOrDefault(data => data.date.Date == currentDate.Date);

            float usageValue = dayData?.usage ?? 0f;
            usageValues.Add(usageValue);
            chart.AddData(0, usageValue);
        }

        // Y축 범위 계산 및 설정
        float maxValue = usageValues.Max();
        float minValue = usageValues.Min();

        // 여유값 추가 (최대값의 10% 추가)
        float yMax = maxValue * 1.1f;
        float yMin = Math.Max(0, minValue * 0.9f); // 음수 방지

        // Y축 설정
        var yAxis = chart.GetChartComponent<YAxis>();
        if (yAxis != null)
        {
            yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
            yAxis.min = yMin;
            yAxis.max = yMax;
        }

        chart.RefreshChart();

        //Debug.Log($"Y축 범위: {yMin} ~ {yMax}, 데이터: [{string.Join(", ", usageValues)}]");
    }
}

[System.Serializable]
public class ElectricUsageData
{
    public DateTime date;
    public float usage;
    public ElectricUsageData(DateTime date, float usage)
    {
        this.date = date;
        this.usage = usage;
    }
}