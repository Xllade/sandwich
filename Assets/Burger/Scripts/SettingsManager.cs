using UnityEngine;

namespace Burger
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private SettingsManagerView _view;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private AudioMixerManager _audioMixerManager;

        void Start()
        {
            _view.AddOnClickBackButton(() =>
            {
                _view.gameObject.SetActive(false);
                SaveSystem.Save(Keys.GAME_DATA_KEY, _gameManager.GameData);
            });
            _view.AddOnClickQuitButton(Application.Quit);
            _view.AddOnChangeBGMSlider((value) =>
            {
                _gameManager.GameData.bgmVolume = value;
                _audioMixerManager.SetBGMVolume(value);
            });
            _view.AddOnChangeSFXSlider((value) =>
            {
                _gameManager.GameData.sfxVolume = value;
                _audioMixerManager.SetSFXVolume(value);
            });
            _view.AddOnValueChangeVibrateToggle((value) => _gameManager.GameData.vibrate = value);
        }

        public void LoadSettings()
        {
            _view.SetBGMSliderValue(_gameManager.GameData.bgmVolume);
            _view.SetSFXSliderValue(_gameManager.GameData.sfxVolume);
            _view.SetVibrateToggleEnabled(_gameManager.GameData.vibrate);
        }

        public void SetActiveView(bool active)
        {
            _view.gameObject.SetActive(active);
        }
    }
}