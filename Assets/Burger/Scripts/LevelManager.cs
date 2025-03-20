using UnityEngine;
using UnityEngine.Events;

namespace Burger
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelManagerView _view;
        [SerializeField] private Transform _selectLevelButtonParent;
        [SerializeField] private SelectLevelButton _selectLevelButtonPrefab;

        public UnityAction<int> OnSelectLevel;

        void Start()
        {
            _view.AddOnClickBackButton(() => _view.gameObject.SetActive(false));
        }

        public void RefreshLevel(BurgerGameManager[] levels)
        {
            var selectLevelButtons = _selectLevelButtonParent.GetComponentsInChildren<SelectLevelButton>();
            for (int i = 0; i < selectLevelButtons.Length; i++)
            {
                Destroy(selectLevelButtons[i].gameObject);
            }
            for (int i = 0; i < levels.Length; i++)
            {
                int index = i;
                var selectLevelButton = Instantiate(_selectLevelButtonPrefab, _selectLevelButtonParent);
                selectLevelButton.Setup($"{index + 1}", 0, () => 
                {
                    Debug.Log("select level "+index);
                    OnSelectLevel?.Invoke(index);
                });
                selectLevelButton.gameObject.SetActive(true);
            }
        }

        public void SetActiveView(bool active)
        {
            _view.gameObject.SetActive(active);
        }
    }
}