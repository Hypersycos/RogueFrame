using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hypersycos.RogueFrame
{
    [System.Serializable]
    public class BoundedStatInstance : StatInstance
    {
        public StatType StatType;
        public class BoundedStatEvent : UnityEvent<BoundedStatInstance, float> { }
        public class CappedBoundedStatEvent : UnityEvent<BoundedStatInstance, float, float> { }
        [field: SerializeField] public float Value { get; protected set; }
        [field: SerializeField] public float MinValue { get; protected set; }
        [field: SerializeField] public float MaxValue { get; protected set; }
        public float MinMaxValue { get; protected set; }

        protected readonly StatGainInstance PositiveGainModifier = new StatGainInstance();
        protected readonly StatGainInstance NegativeGainModifier = new StatGainInstance();
        protected readonly List<StatRegenerationModifier> StatRegenerationModifiers = new();

        public CappedBoundedStatEvent OnFill = new();
        public BoundedStatEvent OnIncrease = new();
        public BoundedStatEvent OnDecrease = new();
        public BoundedStatEvent OnMaxIncrease = new();
        public BoundedStatEvent OnMaxDecrease = new();
        public CappedBoundedStatEvent OnEmpty = new();

        public BoundedStatInstance(float value, float minValue, float maxValue, float minMaxValue=0)
        {
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
            MinMaxValue = minMaxValue;
            //StatType = statType;
        }

        public virtual void Tick(float deltaTime)
        {
            foreach (StatRegenerationModifier modifier in StatRegenerationModifiers)
            {
                float change = modifier.Tick(deltaTime, MaxValue);
                float FlatMultiplier = modifier.Interval == 0 ? deltaTime : 1;
                if (change > 0)
                {
                    AddValue(change, FlatMultiplier);
                }
                else if (change < 0)
                {
                    RemoveValue(change, FlatMultiplier);
                }
            }
        }

        protected virtual void ApplyChangeBehaviour(BoundedStatModifier.ChangeBehaviour changeBehaviour, float NewMax)
        {
            switch (changeBehaviour)
            {
                case BoundedStatModifier.ChangeBehaviour.Proportional:
                    float percentage = Value / MaxValue;
                    MaxValue = NewMax;
                    Value = NewMax * percentage;
                    break;
                case BoundedStatModifier.ChangeBehaviour.Fill:
                    float difference = NewMax - MaxValue;
                    MaxValue = NewMax;
                    Value += difference;
                    break;
                default:
                    MaxValue = NewMax;
                    if (Value > MaxValue) Value = MaxValue;
                    break;
            }
        }

        protected virtual void Recalculate(BoundedStatModifier.ChangeBehaviour changeBehaviour)
        {
            float temp = 0;
            float multTemp = 1;
            foreach (StatModifier modifier in StatModifiers)
            {
                if (multTemp != 1 && modifier.StackBehaviour != StatModifier.StackType.MultiplicativeAdditive)
                {
                    temp *= multTemp;
                    multTemp = 1;
                }
                switch (modifier.StackBehaviour)
                {
                    case StatModifier.StackType.Flat:
                        temp += modifier.Value;
                        break;
                    case StatModifier.StackType.MultiplicativeAdditive:
                        multTemp += modifier.Value;
                        break;
                    case StatModifier.StackType.Multiplicative:
                        temp *= modifier.Value;
                        break;
                }
            }
            if (temp <= MinMaxValue) temp = MinMaxValue;
            if (temp == MaxValue) return;
            float maxChange = temp - MaxValue;
            float valueChange = -Value;

            ApplyChangeBehaviour(changeBehaviour, temp);

            valueChange += Value;
            if (maxChange > 0)
            {
                OnMaxIncrease.Invoke(this, maxChange);
                if (valueChange > 0)
                {
                    OnIncrease.Invoke(this, valueChange);
                }
            }
            else
            {
                OnMaxDecrease.Invoke(this, maxChange);
                if (valueChange < 0)
                {
                    OnDecrease.Invoke(this, valueChange);
                }
            }
        }

        public virtual float ApplyChange(float Amount)
        {
            if (Amount > 0)
            {
                float CappedAmount = Mathf.Min(Amount, MaxValue - Value);
                Value += CappedAmount;
                if (Value == MaxValue)
                {
                    OnFill.Invoke(this, CappedAmount, Amount);
                }
                else
                {
                    OnIncrease.Invoke(this, Amount);
                }
            }
            else if (Amount < 0)
            {
                float CappedAmount = Mathf.Max(Amount, MinValue - Value);
                Value += CappedAmount;
                if (Value == MinValue)
                {
                    OnEmpty.Invoke(this, CappedAmount, Amount);
                }
                else
                {
                    OnDecrease.Invoke(this, Amount);
                }
            }
            return Amount;
        }

        public float AddValue(float Amount, float FlatMultiplier=1, bool AllowInversions = false)
        {
            float ModifiedAmount = Amount;
            ModifiedAmount = PositiveGainModifier.Apply(ModifiedAmount, FlatMultiplier);
            if (ModifiedAmount == 0 || (!AllowInversions && ModifiedAmount < 0))
            {
                return 0;
            }
            return ApplyChange(ModifiedAmount);
        }

        public float RemoveValue(float Amount, float FlatMultiplier = 1, bool AllowInversions = false)
        {
            float ModifiedAmount = Amount;
            ModifiedAmount = NegativeGainModifier.Apply(ModifiedAmount, FlatMultiplier);
            if (ModifiedAmount == 0 || (!AllowInversions && ModifiedAmount < 0))
            {
                return 0;
            }
            return ApplyChange(-ModifiedAmount);
        }

        public bool CanRemoveValue(float Amount, float FlatMultiplier = 1, bool AllowInversions = false)
        {
            float ModifiedAmount = Amount;
            ModifiedAmount = NegativeGainModifier.Apply(ModifiedAmount, FlatMultiplier);
            if (!AllowInversions && ModifiedAmount < 0)
            {
                ModifiedAmount = 0;
            }
            return ModifiedAmount <= Value;
        }

        public bool TryRemoveValue(float Amount, float FlatMultiplier = 1, bool AllowInversions = false)
        {
            float ModifiedAmount = Amount;
            ModifiedAmount = NegativeGainModifier.Apply(ModifiedAmount, FlatMultiplier);
            if (!AllowInversions && ModifiedAmount < 0)
            {
                ModifiedAmount = 0;
            }
            if (ModifiedAmount <= Value)
            {
                ApplyChange(-ModifiedAmount);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void AddModifier(BoundedStatModifier modifier)
        {
            base.AddModifier(modifier);
            Recalculate(modifier.AddBehaviour);
        }

        public override void AddModifier(StatModifier modifier)
        {
            switch(modifier)
            {
                case BoundedStatModifier bModifier:
                    AddModifier(bModifier);
                    break;
                case StatRegenerationModifier rModifier:
                    StatRegenerationModifiers.Add(rModifier);
                    break;
                case StatGainModifier sModifier:
                    switch (sModifier.GainDirection)
                    {
                        case StatGainModifier.Direction.Negative:
                            NegativeGainModifier.AddModifier(sModifier);
                            break;
                        case StatGainModifier.Direction.Positive:
                            PositiveGainModifier.AddModifier(sModifier);
                            break;
                        case StatGainModifier.Direction.Both:
                            PositiveGainModifier.AddModifier(sModifier);
                            NegativeGainModifier.AddModifier(sModifier);
                            break;
                    }
                    break;
                default:
                    throw new System.Exception("Attempt to add invalid modifier to bounded stat");
            }
        }

        public virtual void RemoveModifier(BoundedStatModifier modifier)
        {
            base.RemoveModifier(modifier);
            Recalculate(modifier.RemoveBehaviour);
        }

        public override void RemoveModifier(StatModifier modifier)
        {
            switch (modifier)
            {
                case BoundedStatModifier bModifier:
                    RemoveModifier(bModifier);
                    break;
                case StatRegenerationModifier rModifier:
                    StatRegenerationModifiers.Remove(rModifier);
                    break;
                case StatGainModifier sModifier:
                    switch (sModifier.GainDirection)
                    {
                        case StatGainModifier.Direction.Negative:
                            NegativeGainModifier.AddModifier(sModifier);
                            break;
                        case StatGainModifier.Direction.Positive:
                            PositiveGainModifier.AddModifier(sModifier);
                            break;
                        case StatGainModifier.Direction.Both:
                            PositiveGainModifier.AddModifier(sModifier);
                            NegativeGainModifier.AddModifier(sModifier);
                            break;
                    }
                    break;
                default:
                    throw new System.Exception("Attempt to remove invalid modifier from bounded stat");
            }
        }
    }
}
