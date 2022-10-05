using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [System.Serializable]
    public class DefenseStatInstance : BoundedStatInstance
    {
        SemiBoundedStatInstance ReductionStat;
        float ReductionValue => ReductionStat is null ? 50 : ReductionStat.Value;
        [SerializeField] float DamageReduction => (ReductionValue - 50) / (ReductionValue + 50);
        [SerializeField] float EHP => Value * (ReductionValue + 50) / 100;
        protected List<DefenseGate> defenseGates = new();
        public virtual bool IsActive => MaxValue > 0 && (Value > 0 || defenseGates.Exists(gate => gate.IsActive));
        public DefenseStatInstance Above;
        public DefenseStatInstance Below;
        public DefenseStatInstance(float maxValue, SemiBoundedStatInstance reductionStat = null) : base(maxValue, 0, maxValue, 0)
        {
            ReductionStat = reductionStat;
        }

        public override float ApplyChange(float Amount)
        {
            if (Amount < 0)
            {
                foreach (DefenseGate gate in defenseGates)
                {
                    if (gate.IsAvailable)
                    {
                        Amount = gate.TestDamage(Amount, Value, Mathf.Max(Value - Amount, 0));
                        if (Amount == 0) return 0;
                    }
                }
            }
            float ModifiedAmount = base.ApplyChange(Amount);
            if (ModifiedAmount != Amount)
            {
                float Overflow = Amount - ModifiedAmount;
                if (Amount > 0)
                {
                    if (Above != null)
                    {
                        Overflow = PositiveGainModifier.Reverse(Overflow);
                        return Above.AddValue(Overflow);
                    }
                }
                else
                {
                    if (Above != null)
                    {
                        Overflow = NegativeGainModifier.Reverse(Overflow);
                        return Below.RemoveValue(Overflow);
                    }
                }
            }
            return ModifiedAmount;
        }

        public void AddGate(DefenseGate gate)
        {
            int index = defenseGates.BinarySearch(gate, new DefenseGate.ByPriority());
            index = index < 0 ? ~index : index;
            defenseGates.Insert(index, gate);
        }

        public void RemoveGate(DefenseGate gate)
        {
            defenseGates.Remove(gate);
        }
    }
}
