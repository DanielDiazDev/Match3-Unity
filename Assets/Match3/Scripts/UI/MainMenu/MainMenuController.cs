using System;
using Match3.Scripts.Sound;
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
            ServiceLocator.ServiceLocator.Global.Get<SoundManager>().PlayMusic(SoundId.MainMenuMusic);
            _startButton.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            ServiceLocator.ServiceLocator.Global.Get<SoundManager>().PlaySFX(SoundId.Select);
            SceneManager.LoadScene("LevelSelector");
        }
    }
}