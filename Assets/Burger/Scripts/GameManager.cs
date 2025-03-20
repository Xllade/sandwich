using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private GameData _gameData;
        public GameData GameData => _gameData;

        void Awake()
        {
            void OnSuccessLoadGameData()
            {
                Debug.Log("Success load game data");
                SetLevel(_gameData.levelIndex);
                InitView();
            }
            void OnFailedLoadGameData()
            {
                Debug.Log("Failed load game data");
                _gameData = new GameData();
                _gameData.levelIndex = _levelIndex;
                for (int i = 0; i < _levels.Length; i++)
                {
                    int index = i;
                    var levelData = new LevelData();
                    levelData.starCount = 0;
                    _gameData.levelData.Add(levelData);
                }
                SetLevel(_gameData.levelIndex);
                InitView();
            }
            LoadGameData(OnSuccessLoadGameData, OnFailedLoadGameData);
        }

        private void LoadGameData(UnityAction onSuccessLoad, UnityAction onFailedLoad)
        {
            if (SaveSystem.IsExists(Keys.GAME_DATA_KEY))
            {
                try
                {
                    _gameData = SaveSystem.Load<GameData>(Keys.GAME_DATA_KEY);
                    if (_gameData != null)
                    {
                        onSuccessLoad?.Invoke();
                    }
                    else
                    {
                        onFailedLoad?.Invoke();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                    onFailedLoad?.Invoke();
                }
            }
            else
            {
                onFailedLoad?.Invoke();
            }
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
                _gameData.levelIndex = _levelIndex = index;
                _level = Instantiate(_levels[_levelIndex], transform);
                _level.OnFinishLevel += (firstStar, secondStar, thirdStar) =>
                {
                    var stars = new bool[3]{firstStar, secondStar, thirdStar};
                    _view.SetLevelFinishStars(stars);
                    _view.SetActiveGameplayView(false);
                    _view.SetActiveFinishLevelView(true);
                    int starCount = 0;
                    foreach (var star in stars)
                    {
                        if (star) starCount++;
                    }
                    if (starCount > _gameData.levelData[index].starCount)
                    {
                        _gameData.levelData[index].starCount = starCount;
                        SaveSystem.Save(Keys.GAME_DATA_KEY, _gameData);
                    }
                };
                _view.SetActiveGameplayView(true);
                _view.SetActiveFinishLevelView(false);
                _view.SetLevelText($"Level {_levelIndex + 1}");
                SaveSystem.Save(Keys.GAME_DATA_KEY, _gameData);
            }
            StopCoroutine(DoSetLevel());
            StartCoroutine(DoSetLevel());
        }
    }
}