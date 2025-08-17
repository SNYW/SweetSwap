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
            _score = 0;
        }

        public void PostInit()
        {
          
        }

        public void AddScore(int amount)
        {
            _score += amount;
            OnScoreAdded?.Invoke(_score);
        }

        public void Dispose()
        {
           
        }
    }
}