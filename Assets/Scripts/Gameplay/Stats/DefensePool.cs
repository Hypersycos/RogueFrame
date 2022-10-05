using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class DefensePool : DefenseStatInstance
    {
        List<DefenseStatInstance> DefenseInstances;

        public override bool IsActive
        {
            get
            {
                foreach (DefenseStatInstance inst in DefenseInstances)
                {
                    if (inst.IsActive) return true;
                }
                return false;
            }
        }
        public DefensePool(List<DefenseStatInstance> defenseInstances) : base(1, null)
        {
            OnIncrease.AddListener((_, change) => Value += change);
            OnDecrease.AddListener((_, change) => Value += change);
            OnMaxIncrease.AddListener((_, change) => MaxValue += change);
            OnMaxDecrease.AddListener((_, change) => MaxValue += change);
            OnEmpty.AddListener((_, _, _) => Value = 0);
            OnFill.AddListener((_, _, _) => Value = MaxValue);

            DefenseInstances = defenseInstances;
            for(int i = 1; i < DefenseInstances.Count; i++)
            {
                defenseInstances[i].Below = defenseInstances[i - 1];
                defenseInstances[i - 1].Above = defenseInstances[i];
            }
            defenseInstances[0].OnEmpty.AddListener((_, arg1, arg2) => OnEmpty?.Invoke(this, arg1, arg2));
            defenseInstances[DefenseInstances.Count-1].OnFill.AddListener((_, arg1, arg2) => OnEmpty?.Invoke(this, arg1, arg2));

            float Max = 0;

            foreach(DefenseStatInstance defenseStatInstance in DefenseInstances)
            {
                defenseStatInstance.OnMaxDecrease.AddListener((_, change) => OnMaxDecrease?.Invoke(this, change));
                defenseStatInstance.OnMaxIncrease.AddListener((_, change) => OnMaxIncrease?.Invoke(this, change));
                Max += defenseStatInstance.MaxValue;
            }
            MaxValue = Max;
            Value = Max;
        }

        public override void Tick(float deltaTime)
        {
            if (!IsActive) return;
            foreach(DefenseStatInstance defenseInstance in DefenseInstances)
            {
                defenseInstance.Tick(deltaTime);
            }
            base.Tick(deltaTime);
        }

        public override float ApplyChange(float Amount)
        {
            if (Amount == 0) return 0;

            int index = 0;
            int end = DefenseInstances.Count - 1;
            int direction = 1;
            if (Amount < 0)
            {
                index = end;
                end = 0;
                direction = -1;
            }

            while (index - direction != end)
            {
                DefenseStatInstance inst = DefenseInstances[index];
                if (inst.IsActive)
                {
                    if (Amount > 0)
                    {
                        float change = inst.AddValue(Amount);
                        if (Value < MaxValue) OnIncrease?.Invoke(this, change);
                        return change;
                    }
                    else
                    {
                        float change = inst.RemoveValue(-Amount);
                        if (Value < MaxValue) OnDecrease?.Invoke(this, change);
                        return change;
                    }
                }
                index += direction;
            }
            //has no active health
            return 0;
        }

        public override void AddModifier(BoundedStatModifier modifier)
        {
            foreach(DefenseStatInstance inst in DefenseInstances)
            {
                inst.AddModifier(modifier);
            }
            base.AddModifier(modifier);
        }

        public override void RemoveModifier(BoundedStatModifier modifier)
        {
            foreach(DefenseStatInstance inst in DefenseInstances)
            {
                inst.RemoveModifier(modifier);
            }
            base.RemoveModifier(modifier);
        }

        protected override void Recalculate(BoundedStatModifier.ChangeBehaviour changeBehaviour)
        {
            return;
        }

        protected override void ApplyChangeBehaviour(BoundedStatModifier.ChangeBehaviour changeBehaviour, float NewMax)
        {
            throw new System.Exception("Method should never be called");
        }
    }
}
