using System;
using Unity.VisualScripting;

namespace Managers
{
    public class ScoreManager : IManager
    {
        public Action<int> OnScoreAdded;
        private int _score;
        
        public void Init()
        {
            ResetScore();
        }

        public void PostInit() { }
        
        public void ResetScore()
        {
            _score = 0;
            OnScoreAdded?.Invoke(0);
        }

        public void AddScore(int amount)
        {
            _score += amount;
            OnScoreAdded?.Invoke(_score);
        }
        
        public int GetScore()
        {
            return _score;
        }

        public void Dispose() { }
    }
}