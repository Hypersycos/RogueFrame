using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static Hypersycos.RogueFrame.BoundedStatInstance;

namespace Hypersycos.RogueFrame
{
    [System.Serializable]
    public class SemiBoundedStatInstance : StatInstance, ISync
    {
        public class SemiBoundedStatEvent : UnityEvent<SemiBoundedStatInstance, float> { }
        [field: SerializeField] public StatType StatType { get; protected set; }
        [field: SerializeField] public float Value { get; protected set; }
        [field: SerializeField] public float BaseValue { get; protected set; }
        [field: SerializeField] public float Bound { get; protected set; }
        private bool BoundIsMax => BaseValue < Bound;

        public SemiBoundedStatEvent OnChange = new();

        public SemiBoundedStatInstance(float baseValue, float bound)
        {
            Value = baseValue;
            Bound = bound;
            BaseValue = baseValue;
        }

        public SemiBoundedStatInstance() : this(50, 0) { }

        protected void Recalculate()
        {
            float temp = ApplyModifiers(BaseValue);
            if ((BoundIsMax && Value > Bound) || (!BoundIsMax && Value < Bound))
            {
                temp = Bound;
            }
            if (Value == temp) return;
            float diff = temp - Value;
            Value = temp;
            OnChange?.Invoke(this, diff);
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

        public void StartSync(Action<int, SyncChange> syncFunc, int index)
        {
            OnChange.AddListener((_, change) => syncFunc(index, new SyncChange(true, change)));
        }

        public void ApplySync(SyncChange change)
        {
            Value += change.Change;
        }
    }
}
