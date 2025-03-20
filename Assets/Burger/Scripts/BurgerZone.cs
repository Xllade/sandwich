using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Burger
{
    public class BurgerZone : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        private BurgerGameManager _burgerGameManager;
        private BurgerZone[] _burgerZones = new BurgerZone[4]{null, null, null, null};
        private List<BurgerPart> _burgerPartList = new List<BurgerPart>();
        private bool _isDragging;
        public bool IsMoving { get; set; }
        private float _moveSpeed;

        public UnityAction OnStartMove;
        public UnityAction<MoveData> OnFinishMoveDefault;
        public UnityAction<MoveData> OnFinishMoveUndo;
        public UnityAction<MoveData> OnFinishMoveCancel;

        void Awake()
        {
            _burgerGameManager = FindAnyObjectByType<BurgerGameManager>();
        }

        void Start()
        {
            GetAdjacentBurgerZones();
            GetBurgerPartList();
            _moveSpeed = _burgerGameManager.MoveSpeed;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging) return;
            _isDragging = true;
            if (eventData.delta.x < 0)
            {
                CheckMoveCondition(0);
            }
            if (eventData.delta.x > 0)
            {
                CheckMoveCondition(1);
            }
            if (eventData.delta.y < 0)
            {
                CheckMoveCondition(2);
            }
            if (eventData.delta.y > 0)
            {
                CheckMoveCondition(3);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
        }

        #region Get
        private void GetAdjacentBurgerZones()
        {
            GetBurgerZoneByRaycast(ref _burgerZones[0], Vector3.left);
            GetBurgerZoneByRaycast(ref _burgerZones[1], Vector3.right);
            GetBurgerZoneByRaycast(ref _burgerZones[2], Vector3.back);
            GetBurgerZoneByRaycast(ref _burgerZones[3], Vector3.forward);
        }

        private void GetBurgerZoneByRaycast(ref BurgerZone burgerZone, Vector3 rayDirection)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(rayDirection), out hit, Mathf.Infinity))
            {
                burgerZone = hit.collider.GetComponent<BurgerZone>();
            }
        }

        private void GetBurgerPartList()
        {
            _burgerPartList = GetComponentsInChildren<BurgerPart>().ToList();
            _burgerPartList.Sort((x,y) => x.transform.position.y.CompareTo(y.transform.position.y));
        }
        #endregion


        public float GetBurgerPartTotalHeight()
        {
            float height = 0;
            foreach (var burgetPart in _burgerPartList)
            {
                height += burgetPart.GetComponent<Collider>().bounds.size.y;
            }
            return height;
        }

        private Vector3 GetMoveTargetRotation(int directionType)
        {
            Vector3 rot = Vector3.zero;
            switch (directionType)
            {
                case 0:
                    rot = new Vector3(0, 0, 180);
                    break;
                case 1:
                    rot = new Vector3(0, 0, -180);
                    break;
                case 2:
                    rot = new Vector3(-180, 0, 0);
                    break;
                case 3:
                    rot = new Vector3(180, 0, 0);
                    break;
            }
            return rot;
        }

        #region Move
        private void CheckMoveCondition(int burgerZoneIndex)
        {
            if (!_burgerZones[burgerZoneIndex])
            {
                Debug.Log($"{name} no target burgerzone");
                return;
            }

            if (_burgerPartList.Exists((x) => x.BurgerType == EBurgerPartType.Bottom))
            {
                PrepareMove(_burgerZones[burgerZoneIndex], burgerZoneIndex, true);
            }
            else if (_burgerPartList.Exists((x) => x.BurgerType == EBurgerPartType.Top) && _burgerGameManager.MoveRemaining > 1)
            {
                PrepareMove(_burgerZones[burgerZoneIndex], burgerZoneIndex, true);
            }
            else
            {
                PrepareMove(_burgerZones[burgerZoneIndex], burgerZoneIndex, false);
            }
        }

        public void PrepareMove(BurgerZone targetZone, int directionType, bool isCancel)
        {
            if (IsMoving || targetZone.IsMoving)
            {
                Debug.Log($"{name} or {targetZone} is currently moving burgerpart(s)");
                return;
            }

            if (_burgerPartList.Count == 0 || targetZone._burgerPartList.Count == 0)
            {
                Debug.Log($"{name} or {targetZone} doesn't have burgerpart");
                return;
            }

            if (_burgerPartList.Exists((x) => x.Cooldown > 0) || targetZone._burgerPartList.Exists((x) => x.Cooldown > 0))
            {
                Debug.Log($"{name} or {targetZone} have a burger part that currently in cooldown");
                return;
            }
            
            var moveData = new MoveData();
            moveData.burgerParts = _burgerPartList.ToArray();
            moveData.fromBurgerZone = this;
            moveData.toBurgerZone = targetZone;
            moveData.fromPos = _burgerPartList[0].transform.position;
            Vector3 posOffset = new Vector3(0, GetBurgerPartTotalHeight() + targetZone.GetBurgerPartTotalHeight() + 0.1f);
            moveData.toPos = targetZone.transform.position + posOffset;
            moveData.fromRot = _burgerPartList[0].transform.eulerAngles;
            moveData.toRot = GetMoveTargetRotation(directionType);

            Move(moveData, isCancel ? EMoveType.Cancel : EMoveType.Default);
        }

        public void Move(MoveData moveData, EMoveType moveType)
        {
            var moverGO = new GameObject();
            moverGO.name = "Mover";
            moverGO.transform.position = moveData.burgerParts[0].transform.position;
            moverGO.transform.eulerAngles = moveData.burgerParts[0].transform.eulerAngles;
            foreach (var burgerPart in moveData.burgerParts)
            {
                burgerPart.EnableGravity(false);
                burgerPart.transform.SetParent(moverGO.transform);
            }

            var targetBurgerZone = moveType == EMoveType.Undo ? moveData.fromBurgerZone : moveData.toBurgerZone;
            var startPos = moveType == EMoveType.Undo ? moveData.toPos : moveData.fromPos;
            var endPos = moveType == EMoveType.Undo ? moveData.fromPos : moveData.toPos;
            var startRot = moveType == EMoveType.Undo ? moveData.toRot : moveData.fromRot;
            var endRot = moveType == EMoveType.Undo ? moveData.fromRot : moveData.toRot;
            IsMoving = targetBurgerZone.IsMoving = true;
            OnStartMove?.Invoke();

            if (moveType == EMoveType.Cancel)
            {
                _burgerGameManager.noMoveCancel = false;
                Sequence.Create()
                    .Chain(Tween.Position(moverGO.transform, startPos, endPos, _moveSpeed, Ease.Linear))
                    .Group(Tween.EulerAngles(moverGO.transform, startRot, endRot, _moveSpeed, Ease.Linear))
                    .ChainDelay(_moveSpeed)
                    .Chain(Tween.Position(moverGO.transform, endPos, startPos, _moveSpeed, Ease.Linear))
                    .Group(Tween.EulerAngles(moverGO.transform, endRot, startRot, _moveSpeed, Ease.Linear))
                    .ChainCallback(OnFinishMove);
            }
            else
            {
                Sequence.Create()
                    .Chain(Tween.Position(moverGO.transform, startPos, endPos, _moveSpeed, Ease.Linear))
                    .Group(Tween.EulerAngles(moverGO.transform, startRot, endRot, _moveSpeed, Ease.Linear))
                    .ChainCallback(OnFinishMove);
            }

            void OnFinishMove()
            {
                foreach (var burgerPart in moveData.burgerParts)
                {
                    burgerPart.transform.SetParent(moveType == EMoveType.Cancel ? transform : targetBurgerZone.transform);
                    burgerPart.EnableGravity(true);
                }
                Destroy(moverGO);
                IsMoving = targetBurgerZone.IsMoving = false;
                GetBurgerPartList();
                targetBurgerZone.GetBurgerPartList();
                switch (moveType)
                {
                    case EMoveType.Default:
                        OnFinishMoveDefault?.Invoke(moveData);
                        break;
                    case EMoveType.Undo:
                        OnFinishMoveUndo?.Invoke(moveData);
                        break;
                    case EMoveType.Cancel:
                        OnFinishMoveCancel?.Invoke(moveData);
                        break;
                }
            }
        }
        #endregion
    }
}