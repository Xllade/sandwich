using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Burger
{
    public class GameManagerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private GameObject _titleView;
        [SerializeField] private GameObject _gameplayView;
        [SerializeField] private GameObject _finishLevelView;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _levelButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _undoButton;
        [SerializeField] private Button _retryLevelButton;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Image[] _levelFinishStars;

        public void SetLevelText(string text)
        {
            _levelText.text = text;
        }

        public void SetActiveTitleView(bool active)
        {
            _titleView.SetActive(active);
        }

        public void SetActiveGameplayView(bool active)
        {
            _gameplayView.SetActive(active);
        }

        public void SetActiveFinishLevelView(bool active)
        {
            _finishLevelView.SetActive(active);
        }

        public void AddOnClickPlayButton(UnityAction onClick)
        {
            _playButton.onClick.AddListener(onClick);
        }

        public void AddOnClickSettingsButton(UnityAction onClick)
        {
            _settingsButton.onClick.AddListener(onClick);
        }

        public void AddOnClickLevelButton(UnityAction onClick)
        {
            _levelButton.onClick.AddListener(onClick);
        }

        public void AddOnClickRetryButton(UnityAction onClick)
        {
            _retryButton.onClick.AddListener(onClick);
        }

        public void AddOnClickUndoButton(UnityAction onClick)
        {
            _undoButton.onClick.AddListener(onClick);
        }

        public void AddOnClickNextLevelButton(UnityAction onClick)
        {
            _nextLevelButton.onClick.AddListener(onClick);
        }

        public void AddOnClickRetryLevelButton(UnityAction onClick)
        {
            _retryLevelButton.onClick.AddListener(onClick);
        }

        public void SetLevelFinishStars(bool[] stars)
        {
            for (int i = 0; i < _levelFinishStars.Length; i++)
            {
                int index = i;
                _levelFinishStars[index].color = stars[index] ? Color.yellow : Color.white;
            }
        }
    }
}