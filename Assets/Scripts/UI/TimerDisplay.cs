using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TimerDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text timerText;
        private TimerManager _timerManager;

        private void OnEnable()
        {
            _timerManager = Injection.GetManager<TimerManager>();
            _timerManager.OnTimerTick += OnTimerTick;
        }

        private void OnTimerTick(int timeRemaining)
        {
            timerText.text = FormatTime(timeRemaining);
        }
        
        private string FormatTime(int seconds)
        {
            var minutes = seconds / 60;
            var secs = seconds % 60;
            return $"{minutes:00}:{secs:00}";
        }

        private void OnDisable()
        {
            _timerManager.OnTimerTick -= OnTimerTick;
        }
    }
}