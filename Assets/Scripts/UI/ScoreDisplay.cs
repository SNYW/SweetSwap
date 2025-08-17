using System;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreText;
        
        private int _displayedScore;
        private int _targetDisplayScore;
        private void Start()
        {
            Injection.GetManager<ScoreManager>().OnScoreAdded += OnScoreAdded;
        }

        private void OnScoreAdded(int score)
        {
            _targetDisplayScore = score;
        }

        private void Update()
        {
            if (_displayedScore != _targetDisplayScore)
            {
                _displayedScore = Mathf.RoundToInt(Mathf.Lerp(_displayedScore, _targetDisplayScore, 1));
                _displayedScore = Mathf.Clamp(_displayedScore,0, _targetDisplayScore);
            }
            scoreText.text = _displayedScore.ToString();
        }
    }
}