namespace Managers
{
    public class ScoreManager : IManager
    {
        private int _score;
        
        public void Init()
        {
            _score = 0;
        }

        public void AddScore(int amount)
        {
            _score += amount;
        }

        public int GetScore()
        {
            return _score;
        }

        public void Dispose()
        {
           
        }
    }
}