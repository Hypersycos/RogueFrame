using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hypersycos.RogueFrame
{
    public class SemiBoundedStatInstance : StatInstance
    {
        public float Value { get; protected set; }
        public float BaseValue { get; protected set; }
        public float Bound { get; protected set; }
        private bool BoundIsMax => BaseValue < Bound;

        public SemiBoundedStatInstance(float baseValue, float bound)
        {
            Value = baseValue;
            Bound = bound;
            BaseValue = baseValue;
        }

        protected void Recalculate()
        {
            float temp = BaseValue;
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
            if ((BoundIsMax && Value > Bound) || (!BoundIsMax && Value < Bound))
            {
                temp = Bound;
            }
            Value = temp;
        }

        public override void AddModifier(StatModifier modifier)
        {
            base.AddModifier(modifier);
            Recalculate();
        }

        public override void RemoveModifier(StatModifier modifier)
        {
            base.RemoveModifier(modifier);
            Recalculate();
        }
    }
}
