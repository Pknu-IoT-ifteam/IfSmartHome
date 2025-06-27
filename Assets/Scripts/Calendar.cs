using System;
using TMPro;
using UnityEngine;
using System.Globalization;

namespace SimpleCalendar
{
    public class Calendar : MonoBehaviour
    {
        // Prefab, Panel, and Title references.
        [Tooltip("Prefab for the calendar weekday headers")]
        [SerializeField] private GameObject weekdayNamePrefab;

        [Tooltip("Prefab for the days that fill the calendar")]
        [SerializeField] private GameObject dayPrefab;

        [Tooltip("Panel to put the weekday headers")]
        [SerializeField] private Transform weekdayPanel;

        [Tooltip("Calendar panel to put the days")]
        [SerializeField] private Transform dayPanel;

        [Tooltip("The text that displays the currently selected month and year")]
        [SerializeField] private TextMeshProUGUI calenderTitle;

        // What day starts the week? Usually sunday or monday but I've made it support any starting day.
        [Tooltip("What day starts each calendar week? Usually Sunday or Monday")]
        [SerializeField] private DayOfWeek startingWeekday = DayOfWeek.Sunday;

        // Currently selected month and year.
        // By default, they are set to the current month and year at the start.
        // This could be changed to, for example, save and load the month/year the user was looking at before.

        private ChartDateController chartDateController;
        private int selectedMonth = DateTime.Now.Month;
        private int selectedYear = DateTime.Now.Year;

        private int startMonth = DateTime.Now.Month;
        private int startYear = DateTime.Now.Year;

        private int endMonth = DateTime.Now.AddDays(7).Month;
        private int endYear = DateTime.Now.AddDays(7).Month;

        private bool isInitialized = false;
        /*
            Populate the initial calendar
        */
        void Start()
        {
      
        }

        private void OnEnable()
        {
            chartDateController = GetComponent<ChartDateController>();

            if (!isInitialized)
            {
                AddTableHeaders();
                isInitialized = true;
            }
            SetCalendarDate();
            PopulateCalendar(selectedYear, selectedMonth);
        }

        void AddTableHeaders()
        {
            for (int i = 0; i < 7; i++)
            {
                GameObject newHeader = Instantiate(weekdayNamePrefab, weekdayPanel);
                DayOfWeek currDOW = startingWeekday + i;
                if ((int)currDOW > 6)
                    currDOW -= 7;
                newHeader.GetComponent<TextMeshProUGUI>().text = currDOW.ToString();
            }
        }

        public void SetCalendarDate()
        {
            selectedYear = chartDateController.currDate.Year;
            selectedMonth = chartDateController.currDate.Month;
            Debug.Log(selectedYear + " - " + selectedMonth);
        }

        /// <summary>
        /// Populate the calendar given the year and month.
        /// </summary>
        void PopulateCalendar(int yr, int mon)
        {
            ClearCalendar();
            SetCalenderTitle();
            AddDaysBeforeSelMonth(yr, mon);
            AddDaysDuringSelMonth(yr, mon);
            AddDaysAfterSelMonth(yr, mon);
        }

        /// <summary>
        /// Clear all of the day panel's children
        /// </summary>
        void ClearCalendar()
        {
            for (int i = 0; i < dayPanel.childCount; i++)
            {
                Destroy(dayPanel.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Set Calendar title (example: January 2025)
        /// </summary>
        void SetCalenderTitle() 
        {
            CultureInfo englishCulture = new CultureInfo("en-US");
            calenderTitle.text = englishCulture.DateTimeFormat.GetMonthName(selectedMonth) + " " + selectedYear;
        }

        /// <summary>
        /// Add the days from the last month. 
        /// For example, if the selected month is April, the starting day of the week is set to Sunday, and April starts on a Tuesday, 
        /// we add the last 2 days of March to the calendar before adding April's days.
        /// </summary>
        void AddDaysBeforeSelMonth(int selYear, int selMonth)
        {
            DateTime firstDay = new(selYear, selMonth, 1);
            if (firstDay.DayOfWeek != startingWeekday)
            {
                int lastMonthInt;
                int lastMonthsYearInt;
                if (selMonth == 1)
                {
                    lastMonthInt = 12;
                    lastMonthsYearInt = selYear-1;
                }
                else
                {
                    lastMonthInt = selMonth-1;
                    lastMonthsYearInt = selYear;
                }
                int lastMonDayNum = DateTime.DaysInMonth(lastMonthsYearInt, lastMonthInt);
                for (int i = 0; i < (int)firstDay.DayOfWeek-(int)startingWeekday; i++)
                {
                    GameObject newday = Instantiate(dayPrefab, dayPanel);
                    newday.GetComponent<DayButton>().SetDay(new DateTime(lastMonthsYearInt, lastMonthInt, lastMonDayNum - ((int)firstDay.DayOfWeek - i)+(int)startingWeekday+1), false, 0);
                }
            }
        }

        /// <summary>
        /// Adds the currently selected month's days.
        /// If the selected month is April, it adds all 30 days of April. 
        /// The starting day of the month's day of the week is handled by the days added previously in AddDaysBeforeSelMonth.
        /// </summary>
        void AddDaysDuringSelMonth(int selYear, int selMonth)
        {
            for (int i = 0; i < DateTime.DaysInMonth(selYear, selMonth); i++)
            {
                GameObject newday = Instantiate(dayPrefab, dayPanel);
                newday.GetComponent<DayButton>().SetDay(new DateTime(selYear, selMonth, i+1), true, i%2);
            }
        }

        /// <summary>
        /// Add the next month's days.
        /// For example, if the selected month is April, the starting day of the week is set to Sunday, and the last day of April is a Wednesday,
        /// we add the first 3 days of May after adding April's days.
        /// </summary>
        void AddDaysAfterSelMonth(int selYear, int selMonth)
        {
            DateTime lastDay = new(selYear, selMonth, DateTime.DaysInMonth(selYear, selMonth));
            DayOfWeek LastDayOfWeek = startingWeekday-1;
            if (LastDayOfWeek < 0)
            {
                LastDayOfWeek = DayOfWeek.Saturday;
            }
            if (lastDay.DayOfWeek != LastDayOfWeek)
            {
                int nextMonth;
                int nextMonthsYear;
                if (selMonth == 12)
                {
                    nextMonth = 1;
                    nextMonthsYear = selYear+1;
                }
                else
                {
                    nextMonth = selMonth+1;
                    nextMonthsYear = selYear;
                }
                int numNextMonthDaysToDisplay = (int)LastDayOfWeek - (int)lastDay.DayOfWeek;
                if (numNextMonthDaysToDisplay < 0)
                {
                    numNextMonthDaysToDisplay +=7;
                }
                for (int i = 0; i < numNextMonthDaysToDisplay; i++)
                {
                    GameObject newday = Instantiate(dayPrefab, dayPanel);
                    newday.GetComponent<DayButton>().SetDay(new DateTime(nextMonthsYear, nextMonth, i+1), false, 0);
                }
            }
        }

        // Button Functions:
        
        /// <summary>
        /// Called by the right arrow button at the top of the calendar.
        /// Increments the Selected Month and repopulates the calendar
        /// </summary>
        public void SelectNextMonth()
        {
            selectedMonth +=1;
            if (selectedMonth >12)
            {
                selectedMonth = 1;
                selectedYear+=1;
            }
            PopulateCalendar(selectedYear, selectedMonth);
        }
        /// <summary>
        /// Called by the left arrow button at the top of the calendar.
        /// Decrements the Selected Month and repopulates the calendar
        /// </summary>
        public void SelectLastMonth()
        {
            selectedMonth -=1;
            if (selectedMonth < 1)
            {
                selectedMonth = 12;
                selectedYear-=1;
            }
            PopulateCalendar(selectedYear, selectedMonth);
        }

        public void OnDateSelected(DateTime date)
        {
            if (!chartDateController) return;

            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();

            string selectedDate = year + "-" + month + "-" + day;

            chartDateController.SetDate(date);
        }

    }
}