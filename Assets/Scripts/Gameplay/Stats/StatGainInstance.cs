using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [Serializable]
    public class StatGainInstance : StatInstance
    {
        private readonly Dictionary<float, float> cache = new();
        public bool blocked { get; private set; } = false;
        public StatGainInstance() { }

        public float Apply(float Value, float FlatMultiplier=1)
        {
            if (blocked)
                return 0;

            if (cache.ContainsKey(Value))
            {
                return cache[Value];
            }
            float temp = ApplyModifiers(Value, FlatMultiplier);
            cache[Value] = temp;
            return temp;
        }

        public float Reverse(float Value)
        {
            float temp = Value;
            float multTemp = 1;
            foreach (StatModifier modifier in StatModifiers)
            {
                //TODO: Fix this, broken with multiplicativeadditive
                if (multTemp != 1 && modifier.StackBehaviour != StatModifier.StackType.MultiplicativeAdditive)
                {
                    temp /= multTemp;
                    multTemp = 1;
                }
                switch (modifier.StackBehaviour)
                {
                    case StatModifier.StackType.MultiplicativeAdditive:
                        multTemp += modifier.Value;
                        break;
                    case StatModifier.StackType.Multiplicative:
                        temp /= modifier.Value;
                        break;
                }
            }
            return temp;
        }

        public override void AddModifier(StatModifier modifier)
        {
            base.AddModifier(modifier);
            if (StatModifiers[StatModifiers.Count] == modifier)
            {
                blocked = modifier.StackBehaviour == StatModifier.StackType.Multiplicative
                          && modifier.Value == 0;
            }
            cache.Clear();
        }

        public override void RemoveModifier(StatModifier modifier)
        {
            base.RemoveModifier(modifier);
            StatModifier last = StatModifiers[StatModifiers.Count];
            blocked = last.StackBehaviour == StatModifier.StackType.Multiplicative
                      && last.Value == 0;
            cache.Clear();
        }
    }
}
