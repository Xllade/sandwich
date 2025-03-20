using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Burger
{
    public class SettingsManagerView : MonoBehaviour
    {
        [SerializeField] private Button _backButton;

        public void AddOnClickBackButton(UnityAction onClick)
        {
            _backButton.onClick.AddListener(onClick);
        }
    }
}