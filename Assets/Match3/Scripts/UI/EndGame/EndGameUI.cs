using Match3.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Scripts.UI.EndGame
{
    public class EndGameUI : MonoBehaviour
    {
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _losePanel;
        [SerializeField] private WinUI _winUI;
        [SerializeField] private LoseUI _loseUI;

        private const string LEVEL_SELECTOR_SCENE = "LevelSelector";

        public void ShowWin(int score, int stars)
        {
            _winPanel.SetActive(true);
            _losePanel.SetActive(false);
            _winUI.Setup(score, stars, ReturnToMenu);
        }

        public void ShowLose()
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
