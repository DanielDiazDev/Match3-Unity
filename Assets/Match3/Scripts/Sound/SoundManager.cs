using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Match3.Scripts.Sound
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Clips")]
        [SerializeField] private SerializedDictionary<SoundId, AudioClip> _clips;

        [Header("Volumes")]
        [Range(0f, 1f)] [SerializeField] private float _masterVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _musicVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 1f;
        public bool IsMuted => _isMuted;

        public System.Action<bool> OnMuteChanged;
        private bool _isMuted;

        private const string MASTER_KEY = "MasterVolume";
        private const string MUSIC_KEY = "MusicVolume";
        private const string SFX_KEY = "SFXVolume";
        private const string MUTE_KEY = "Muted";

        private void Awake()
        {
            ServiceLocator.ServiceLocator.Global.Register(this);
            LoadPreferences();
            ApplyVolumes();
            
        }

        #region Public API

        public void PlayMusic(SoundId soundId, bool loop = true)
        {
            if (!_clips.TryGetValue(soundId, out var clip)) return;
            if (_musicSource.clip == clip) return;

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void StopMusic() => _musicSource.Stop();

        public void PlaySFX(SoundId soundId)
        {
            if (!_clips.TryGetValue(soundId, out var clip)) return;

            _sfxSource.PlayOneShot(clip, _sfxVolume * _masterVolume);
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            Save(MASTER_KEY, _masterVolume);
            ApplyVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            Save(MUSIC_KEY, _musicVolume);
            ApplyVolumes();
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            Save(SFX_KEY, _sfxVolume);
            ApplyVolumes();
        }

        public void ToggleMute()
        {
            SetMute(!_isMuted);
        }

        public void SetMute(bool mute)
        {
            _isMuted = mute;
            PlayerPrefs.SetInt(MUTE_KEY, mute ? 1 : 0);
            ApplyVolumes();
            OnMuteChanged?.Invoke(_isMuted);
        }
        #endregion

        private void ApplyVolumes()
        {
            float master = _isMuted ? 0f : _masterVolume;

            _musicSource.volume = _musicVolume * master;
            _sfxSource.volume = _sfxVolume * master;
        }

        private void LoadPreferences()
        {
            _masterVolume = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
            _musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
            _sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
            _isMuted = PlayerPrefs.GetInt(MUTE_KEY, 0) == 1;
        }

        private void Save(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }
    }

    public enum SoundId
    {
        MainMenuMusic,
        LevelSelectorMusic,
        GameMusic,
        Swap,
        Match,
        GameOver,
        Victory,
        Select
    }
}
