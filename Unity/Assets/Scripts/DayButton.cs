using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;


namespace SimpleCalendar
{
    /// <summary>
    /// Button that fills the day grid panel
    /// </summary>
    public class DayButton : MonoBehaviour
    {
        [Tooltip("reference to the text field that displays the day.")]
        [SerializeField] private TextMeshProUGUI dayText;

        [Tooltip("Color of the day's box when it is today's date.")]
        [SerializeField] private Color todaysColor = Color.yellow;

        [Tooltip("Base Color of the day's box.")]
        [SerializeField] private Color baseColor = Color.white;

        [Tooltip("Alternate Color of the day's box.")]
        [SerializeField] private Color altColor = new(0.90f, 0.90f, 0.90f);

        [Tooltip("Color of the day's box when it is not part of the selected month (AKA a padding day).")]
        [SerializeField] private Color paddingDayColor = new(0.75f, 0.75f, 0.75f);


        /// <summary>
        /// The date associated with this day button
        /// </summary>
        private DateTime Date;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();

            button.onClick.AddListener(OnDayClicked);
        }

        private void OnDayClicked()
        {
            FindObjectOfType<Calendar>().OnDateSelected(Date);
        }
        public void SetDay(DateTime day, bool curMonth, int col)
        {
            Date = day;
            dayText.text = Date.Day.ToString();

            // Set the color:
            if (Date.Date == DateTime.Now.Date)
            {
                GetComponent<UnityEngine.UI.Image>().color = todaysColor;
            }
            else if (!curMonth)
            {
                GetComponent<UnityEngine.UI.Image>().color = paddingDayColor;
            }
            else
            {
                GetComponent<UnityEngine.UI.Image>().color = baseColor;
            }
        }
    }
}