using System;
using Match3.Scripts.Core;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Match3.Scripts.UI.EndGame
{
    public class EndGameUI : MonoBehaviour, IEventObserver
    {
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _nextButton; 

        private const string LEVEL_SELECTOR_SCENE = "LevelSelector";

        private void OnDisable()
        {
            EventQueue.Instance.Unsubscribe(EventIds.ShowWin, this);
            EventQueue.Instance.Unsubscribe(EventIds.ShowLose, this);
        }

        private void Start()
        {
            EventQueue.Instance.Subscribe(EventIds.ShowWin, this);
            EventQueue.Instance.Subscribe(EventIds.ShowLose, this);
            _nextButton.onClick.AddListener(ReturnToMenu);
        }

        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.ShowLose)
            {
                ServiceLocator.ServiceLocator.Global.Get<SoundManager>().PlayMusic(SoundId.GameOver);
                var showLoseEventData = (ShowLoseEventData)eventData;
                ShowLose(showLoseEventData.Score);
            }

            if (eventData.EventId == EventIds.ShowWin)
            {
                ServiceLocator.ServiceLocator.Global.Get<SoundManager>().PlayMusic(SoundId.Victory);
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
