using System;
using UnityEngine;

namespace Burger
{
    [Serializable]
    public struct MoveData
    {
        public BurgerPart[] burgerParts;
        public BurgerZone fromBurgerZone;
        public BurgerZone toBurgerZone;
        public Vector3 fromPos;
        public Vector3 toPos;
        public Vector3 fromRot;
        public Vector3 toRot;
    }
}