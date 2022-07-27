using System.Collections;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    internal interface IAbility
    {
        [SerializeField] public string Name { get; protected set; }

        public bool CanCast(PlayerState state);
        public void Cast(Transform playerPosition, Quaternion lookDirection);
    }
}