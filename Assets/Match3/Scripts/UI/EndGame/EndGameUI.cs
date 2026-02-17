using System;
using Match3.Scripts.Core;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Scripts.UI.EndGame
{
    public class EndGameUI : MonoBehaviour, IEventObserver
    {
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        

        private const string LEVEL_SELECTOR_SCENE = "LevelSelector";

        private void Start()
        {
            EventQueue.Instance.Subscribe(EventIds.ShowWin, this);
            EventQueue.Instance.Subscribe(EventIds.ShowLose, this);
        }

        private void OnDestroy()
        {
            EventQueue.Instance.Unsubscribe(EventIds.ShowWin, this);
            EventQueue.Instance.Unsubscribe(EventIds.ShowLose, this);
        }

        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.ShowLose)
            {
                var showLoseEventData = (ShowLoseEventData)eventData;
                ShowLose(showLoseEventData.Score);
            }

            if (eventData.EventId == EventIds.ShowWin)
            {
                var showWinEventData = (ShowWinEventData)eventData;
                ShowWin(showWinEventData.Score);
            }
        }

        private void ShowWin(int score)
        {
            _resultText.text = "You win!!";
            _scoreText.text = score.ToString();
        }

        private void ShowLose(int score)
        {
            _resultText.text = "You lose!";
            _scoreText.text = score.ToString();
        }

        private void ReturnToMenu()
        {
            SceneManager.LoadScene(LEVEL_SELECTOR_SCENE);
        }

        
    }
}
