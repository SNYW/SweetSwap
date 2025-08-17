using System;
using System.Threading;
using System.Threading.Tasks;
using Settings;

namespace Managers
{
    public class TimerManager : IManager
    {
        public event Action<int> OnTimerTick;
        public event Action OnTimerFinished;
        private int _timeRemaining;
        private CancellationTokenSource _cts;
        private GameSettings _settings;

        public void Init() { }

        public void PostInit()
        {
            _settings = Injection.GetManager<SettingsManager>().ActiveSettings;
        }

        public void ResetTimer()
        {
            StopTimer();
            _cts = new CancellationTokenSource();
            _ = RunTimer(_cts.Token);
        }

        public void StopTimer()
        {
            if (_cts == null) return;
            
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        private async Task RunTimer(CancellationToken token)
        {
            _timeRemaining = _settings.roundDuration;
            while (_timeRemaining > 0 && !token.IsCancellationRequested)
            {
                OnTimerTick?.Invoke(_timeRemaining);

                await Task.Delay(1000, token);

                _timeRemaining--;
            }

            if (_timeRemaining <= 0 && !token.IsCancellationRequested)
            {
                OnTimerTick?.Invoke(0);
                OnTimerFinished?.Invoke();
            }
        }

        public void Dispose()
        {
            StopTimer();
        }
    }
}