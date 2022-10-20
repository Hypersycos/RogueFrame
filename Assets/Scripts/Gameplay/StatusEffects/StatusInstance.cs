using static Hypersycos.RogueFrame.DefensePool;
using System;
using UnityEngine;
using System.Collections;

namespace Hypersycos.RogueFrame
{
    [Serializable]
    public abstract class StatusInstance : ICloneable
    {
        [field: SerializeField] public StatusEffect StatusEffect { get; protected set; }
        public float Amount;
        public CharacterState owner { get; private set; } = null;

        public StatusInstance(float amount, CharacterState owner, StatusEffect statusEffect) : this(amount, statusEffect)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public StatusInstance(float amount, StatusEffect statusEffect)
        {
            Amount = amount;
            StatusEffect = statusEffect;
        }

        public StatusInstance() : this(0, null) { }

        public StatusInstance CloneInstance()
        {
            return (StatusInstance)Clone();
        }

        public void SetOwner(CharacterState Owner)
        {
            if (owner != null)
                Debug.Log("Attempted to set status instance owner twice");
            owner = Owner ?? throw new ArgumentNullException(nameof(Owner));
        }

        public abstract void Combine(StatusInstance other);
        public abstract void Refresh(StatusInstance other);

        public object Clone()
        {
            return MemberwiseClone();
        }

        public abstract void Apply(CharacterState victim, Func<IEnumerator, Coroutine> Start);
        public abstract void Remove(CharacterState victim);
    }
}