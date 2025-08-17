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
            scoreText.text = _displayedScore.ToString();
        }
    }
}