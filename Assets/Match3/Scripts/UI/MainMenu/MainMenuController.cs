using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Match3.Scripts.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button _startButton;

        private void Start()
        {
            _startButton.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            SceneManager.LoadScene("LevelSelector");
        }
    }
}