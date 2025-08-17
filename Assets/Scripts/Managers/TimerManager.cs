using System;
using System.Threading;
using System.Threading.Tasks;

namespace Managers
{
    public class TimerManager : IManager
    {
        public event Action<int> OnTimerTick;
        public event Action OnTimerFinished;
        private int _timeRemaining;
        private CancellationTokenSource _cts;

        public void Init() { 
            _timeRemaining = Injection.GetManager<SettingsManager>().ActiveSettings.roundDuration;
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
            while (_timeRemaining > 0 && !token.IsCancellationRequested)
            {
                OnTimerTick?.Invoke(_timeRemaining);

                await Task.Delay(1000, token).ContinueWith(_ => { }, token); // wait 1 second

                _timeRemaining--;
            }

            if (_timeRemaining <= 0 && !token.IsCancellationRequested)
            {
                OnTimerFinished?.Invoke();
            }
        }

        public void Dispose()
        {
            StopTimer();
        }
    }
}