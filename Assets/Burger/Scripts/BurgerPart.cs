using TMPro;
using UnityEngine;

namespace Burger
{
    public class BurgerPart : MonoBehaviour
    {
        private Rigidbody _rigidBody;
        [SerializeField] private EBurgerPartType _burgerType;
        public EBurgerPartType BurgerType => _burgerType;
        [SerializeField] private int _initialCooldown;
        public int InitialCooldown => _initialCooldown;
        public int Cooldown { get; private set; }
        [SerializeField] private TextMeshProUGUI _cooldownText;

        void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            Cooldown = _initialCooldown;
            UpdateCooldownText();
            EnableGravity(true);
        }

        private void UpdateCooldownText()
        {
            _cooldownText.text = Cooldown == 0 ? "" : Cooldown.ToString();
        }

        public void AddCooldown()
        {
            if (Cooldown < _initialCooldown) Cooldown++;
            UpdateCooldownText();
        }

        public void SubstractCooldown()
        {
            if (Cooldown > 0) Cooldown--;
            UpdateCooldownText();
        }

        public void EnableGravity(bool enable)
        {
            _rigidBody.useGravity = enable;
        }
    }
}