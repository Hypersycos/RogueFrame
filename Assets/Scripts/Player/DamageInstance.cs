﻿using System;
using System.Collections.Generic;
using UnityEngine;
using static Hypersycos.RogueFrame.DefensePool;

namespace Hypersycos.RogueFrame
{
    [Serializable]
    public class DamageInstance
    {
        public bool IsDamage;
        public float Amount;
        public StatTypeTarget ValidStatTypes;
        public CharacterState owner { get; private set; } = null;
        [SerializeField, ReadOnly] private float? _ActualAmount = null;
        public HashSet<string> OneTimeEffects = new();
        public float ActualAmount
        {
            get
            {
                if (_ActualAmount == null)
                {
                    _ActualAmount = Amount;
                }
                return (float)_ActualAmount;
            }
            set
            {
                _ActualAmount = value;
            }
        }
        public readonly CharacterState.CharacterStateHealthEvent BeforeApply = new();
        public readonly CharacterState.CharacterStateHealthEvent OnApply = new();
        public readonly CharacterState.CharacterStateHealthEvent OnFullApply = new();

        public DamageInstance(bool isDamage, float amount, CharacterState owner, StatTypeTarget validStatTypes) : this(isDamage, amount, validStatTypes)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public DamageInstance(bool isDamage, float amount, StatTypeTarget validStatTypes)
        {
            this.IsDamage = isDamage;
            this.Amount = amount;
            ActualAmount = amount;
            ValidStatTypes = validStatTypes;
        }

        public DamageInstance(DamageInstance inst)
        {
            IsDamage = inst.IsDamage;
            Amount = inst.Amount;
            ActualAmount = Amount;
            ValidStatTypes = inst.ValidStatTypes;
            owner = null;
            OneTimeEffects = new(inst.OneTimeEffects);
        }

        public DamageInstance()
        {
            IsDamage = true;
            Amount = 0;
            owner = null;
            ValidStatTypes = StatTypeTarget.AllValid;
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