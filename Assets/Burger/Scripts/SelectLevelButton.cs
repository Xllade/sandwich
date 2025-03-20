using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Burger
{
    [RequireComponent(typeof(Button))]
    public class SelectLevelButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private GameObject[] _stars;
        
        public void Setup(string text, int stars, UnityAction onClick)
        {
            _text.text = text;
            foreach (var star in _stars)
            {
                star.GetComponent<Image>().color = Color.white;
            }
            for (int i = 0; i < stars; i++)
            {
                _stars[i].GetComponent<Image>().color = Color.yellow;
            }
            GetComponent<Button>().onClick.AddListener(onClick);
        }
    }
}