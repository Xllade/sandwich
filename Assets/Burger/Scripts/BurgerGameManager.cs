using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Burger
{
    public class BurgerGameManager : MonoBehaviour
    {
        private List<MoveData> _moveDatas = new List<MoveData>();
        private BurgerZone[] _burgerZones;
        private BurgerPart[] _burgerParts;
        public int MoveRemaining => _burgerZones.Length - _moveDatas.Count - 1;
        private bool _isLevelComplete;
        public float MoveSpeed { get; private set; } = 0.2f;

        public UnityAction OnStartMove;
        public UnityAction<MoveData> OnFinishMove;
        public UnityAction OnFinishLevel;

        void Awake()
        {
            _burgerZones = GetComponentsInChildren<BurgerZone>();
            _burgerParts = GetComponentsInChildren<BurgerPart>();
        }

        void OnEnable()
        {
            foreach (var burgerZone in _burgerZones)
            {
                burgerZone.OnStartMove += OnStartMoveBurgerZone;
                burgerZone.OnFinishMoveDefault += OnFinishMoveDefaultBurgerZone;
                burgerZone.OnFinishMoveUndo += OnFinishMoveUndoBurgerZone;
                burgerZone.OnFinishMoveCancel += OnFinishMoveCancelBurgerZone;
            }
        }

        void OnDisable()
        {
            foreach (var burgerZone in _burgerZones)
            {
                burgerZone.OnStartMove -= OnStartMoveBurgerZone;
                burgerZone.OnFinishMoveDefault -= OnFinishMoveDefaultBurgerZone;
                burgerZone.OnFinishMoveUndo -= OnFinishMoveUndoBurgerZone;
                burgerZone.OnFinishMoveCancel -= OnFinishMoveCancelBurgerZone;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.U)) Undo();
            if (Input.GetKeyDown(KeyCode.R)) Retry();
        }

        #region Callback
        private void OnStartMoveBurgerZone()
        {
            OnStartMove?.Invoke();
        }

        private void OnFinishMoveDefaultBurgerZone(MoveData moveData)
        {
            _moveDatas.Add(moveData);
            foreach (var burgerPart in _burgerParts)
            {
                burgerPart.SubstractCooldown();
            }
            if (_moveDatas.Count == _burgerZones.Length - 1)
            {
                _isLevelComplete = true;
                OnFinishLevel?.Invoke();
            }
            OnFinishMove?.Invoke(moveData);
        }

        private void OnFinishMoveUndoBurgerZone(MoveData moveData)
        {
            _moveDatas.Remove(moveData);
            foreach (var burgerPart in _burgerParts)
            {
                if (burgerPart.InitialCooldown > _moveDatas.Count) burgerPart.AddCooldown();
            }
            OnFinishMove?.Invoke(moveData);
        }

        private void OnFinishMoveCancelBurgerZone(MoveData moveData)
        {
            OnFinishMove?.Invoke(moveData);
        }
        #endregion

        public void Undo()
        {
            if (_moveDatas.Count == 0 || _isLevelComplete) return;
            var lastMoveData = _moveDatas[_moveDatas.Count - 1];
            lastMoveData.toBurgerZone.Move(lastMoveData, EMoveType.Undo);
        }

        public void Retry()
        {
            if (_moveDatas.Count == 0 || _isLevelComplete) return;
            IEnumerator DoRetry()
            {
                for (int i = _moveDatas.Count; i > 0; i--)
                {
                    Undo();
                    yield return new WaitForSeconds(MoveSpeed + 0.01f);
                }
            }
            StartCoroutine(DoRetry());
        }
    }
}