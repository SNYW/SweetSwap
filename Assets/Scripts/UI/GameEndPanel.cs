using TMPro;
using UnityEngine;

namespace UI
{
    public class GameEndPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text highScoreText;
        [SerializeField]
        private TMP_Text currentScoreText;

        public void Init(bool isHighScore, string score)
        {
            highScoreText.text = isHighScore ? "New High Score!" : "Score:";
            currentScoreText.text = score;
        }
    }
}