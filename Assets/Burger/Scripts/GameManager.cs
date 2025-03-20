using System.Collections;
using UnityEngine;

namespace Burger
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameManagerView _view;
        [SerializeField] private SettingsManager _settingsManager;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private BurgerGameManager[] _levels;
        private BurgerGameManager _level;
        private int _levelIndex;

        void Awake()
        {
            SetLevel(PlayerPrefs.HasKey(Keys.LEVEL_INDEX_KEY) ? PlayerPrefs.GetInt(Keys.LEVEL_INDEX_KEY) : 0);
            InitView();
        }

        void OnEnable()
        {
            _levelManager.OnSelectLevel += OnSelectLevel;
        }

        void OnDisable()
        {
            _levelManager.OnSelectLevel -= OnSelectLevel;
        }

        private void OnSelectLevel(int index)
        {
            SetLevel(index);
            _levelManager.SetActiveView(false);
        }

        private void InitView()
        {
            _view.AddOnClickSettingsButton(() => _settingsManager.SetActiveView(true));
            _view.AddOnClickLevelButton(() =>
            {
                _levelManager.RefreshLevel(_levels);
                _levelManager.SetActiveView(true);
            });
            _view.AddOnClickPlayButton(() => _view.SetActiveTitleView(false));
            _view.AddOnClickRetryButton(() => _level.Retry());
            _view.AddOnClickUndoButton(() => _level.Undo());
            _view.AddOnClickRetryLevelButton(() => SetLevel(_levelIndex));
            _view.AddOnClickNextLevelButton(() => SetLevel(_levelIndex == _levels.Length - 1 ? _levelIndex : _levelIndex + 1));
            _view.SetActiveTitleView(true);
            _settingsManager.SetActiveView(false);
            _levelManager.SetActiveView(false);
        }

        private void SetLevel(int index)
        {
            IEnumerator DoSetLevel()
            {
                if (_level) Destroy(_level.gameObject);
                yield return new WaitForSeconds(0.1f);
                _levelIndex = index;
                PlayerPrefs.SetInt(Keys.LEVEL_INDEX_KEY, _levelIndex);
                _level = Instantiate(_levels[_levelIndex], transform);
                _level.OnFinishLevel += () =>
                {
                    _view.SetActiveGameplayView(false);
                    _view.SetActiveFinishLevelView(true);
                };
                _view.SetActiveGameplayView(true);
                _view.SetActiveFinishLevelView(false);
                _view.SetLevelText($"Level {_levelIndex + 1}");
            }
            StopCoroutine(DoSetLevel());
            StartCoroutine(DoSetLevel());
        }
    }
}