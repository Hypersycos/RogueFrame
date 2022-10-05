using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hypersycos.RogueFrame
{
    public abstract class DefenseGate
    {
        public bool IsActive { get; protected set; }
        public bool IsAvailable { get; protected set; }
        public float Duration { get; protected set; }
        public int Priority { get; protected set; }

        public abstract float TestDamage(float damage, float oldHealth, float newHealth);

        public class ByPriority : IComparer<DefenseGate>
        {
            public int Compare(DefenseGate x, DefenseGate y)
            {
                return x.Priority.CompareTo(y.Priority);
            }
        }
    }
}
