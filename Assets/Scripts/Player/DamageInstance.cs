using System;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [Serializable]
    public class DamageInstance
    {
        public bool isDamage;
        public float amount;
        public CharacterState owner { get; private set; } = null;
        [SerializeField] private float? _actualAmount = null;
        public float actualAmount
        {
            get
            {
                if (_actualAmount == null)
                {
                    _actualAmount = amount;
                }
                return (float)_actualAmount;
            }
            set
            {
                _actualAmount = value;
            }
        }
        public readonly CharacterState.CharacterStateEvent BeforeApply = new();
        public readonly CharacterState.CharacterStateEvent OnApply = new();
        public readonly CharacterState.CharacterStateEvent OnFullApply = new();

        public DamageInstance(bool isDamage, float amount, CharacterState owner) : this(isDamage, amount)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public DamageInstance(bool isDamage, float amount)
        {
            this.isDamage = isDamage;
            this.amount = amount;
            actualAmount = amount;
        }

        public DamageInstance()
        {
            isDamage = true;
            amount = 0;
            owner = null;
        }

        public void SetOwner(CharacterState Owner)
        {
            if (owner != null)
                Debug.Log("Attempted to set damage instance owner twice");
            owner = Owner ?? throw new ArgumentNullException(nameof(Owner));
        }
        //TODO: List of modifiers which can only apply once
        //e.g. if a character is splitting its damage with someone else, they can't also
        //apply that since that would cause an infinite loop
    }
}