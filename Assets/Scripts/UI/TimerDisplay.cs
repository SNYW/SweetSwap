using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TimerDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text timerText;

        private void Start()
        {
            Injection.GetManager<TimerManager>().OnTimerTick += OnTimerTick;
        }

        private void OnTimerTick(int timeRemaining)
        {
            timerText.text = FormatTime(timeRemaining);
        }
        
        private string FormatTime(int seconds)
        {
            int minutes = seconds / 60;
            int secs = seconds % 60;
            return $"{minutes:00}:{secs:00}";
        }
    }
}