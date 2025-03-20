using UnityEngine;

namespace Burger
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private SettingsManagerView _view;

        void Start()
        {
            _view.AddOnClickBackButton(() => _view.gameObject.SetActive(false));
        }

        public void SetActiveView(bool active)
        {
            _view.gameObject.SetActive(active);
        }
    }
}