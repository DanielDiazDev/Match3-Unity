using System;
using Match3.Scripts.Core;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Scripts.UI.EndGame
{
    public class EndGameUI : MonoBehaviour, IEventObserver
    {
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _losePanel;
        [SerializeField] private WinUI _winUI;
        [SerializeField] private LoseUI _loseUI;

        private const string LEVEL_SELECTOR_SCENE = "LevelSelector";

        private void OnEnable()
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
                ShowLose();
            }

            if (eventData.EventId == EventIds.ShowWin)
            {
                var showWinEventData = (ShowWinEventData)eventData;
                ShowWin(showWinEventData.Score, showWinEventData.Star);
            }
        }

        private void ShowWin(int score, int stars)
        {
            _winPanel.SetActive(true);
            _losePanel.SetActive(false);
            _winUI.Setup(score, stars, ReturnToMenu);
        }

        private void ShowLose()
        {
            _winPanel.SetActive(false);
            _losePanel.SetActive(true);
            _loseUI.Setup(ReturnToMenu);
        }

        private void ReturnToMenu()
        {
            SceneManager.LoadScene(LEVEL_SELECTOR_SCENE);
        }

        
    }
}
