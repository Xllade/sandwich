using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Burger
{
    public class SettingsManagerView : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Slider _bgmSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Toggle _vibrateToggle;

        public void AddOnClickBackButton(UnityAction onClick)
        {
            _backButton.onClick.AddListener(onClick);
        }

        public void AddOnClickQuitButton(UnityAction onClick)
        {
            _quitButton.onClick.AddListener(onClick);
        }

        public void AddOnChangeBGMSlider(UnityAction<float> onValueChanged)
        {
            _bgmSlider.onValueChanged.AddListener(onValueChanged);
        }

        public void SetBGMSliderValue(float value)
        {
            _bgmSlider.value = value;
        }

        public void AddOnChangeSFXSlider(UnityAction<float> onValueChanged)
        {
            _sfxSlider.onValueChanged.AddListener(onValueChanged);
        }

        public void SetSFXSliderValue(float value)
        {
            _sfxSlider.value = value;
        }

        public void AddOnValueChangeVibrateToggle(UnityAction<bool> onValueChanged)
        {
            _vibrateToggle.onValueChanged.AddListener(onValueChanged);
        }

        public void SetVibrateToggleEnabled(bool value)
        {
            _vibrateToggle.enabled = value;
        }
    }
}