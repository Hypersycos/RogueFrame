using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class BoundedStatModifier : StatModifier
    {
        public enum ChangeBehaviour
        {
            Fill,
            Proportional,
            Empty,
            Overflow
        }

        public ChangeBehaviour AddBehaviour;
        public ChangeBehaviour RemoveBehaviour;

        public BoundedStatModifier(StackType stackType, int? stackLevel, float value,
            ChangeBehaviour addBehaviour, ChangeBehaviour removeBehaviour) : base(stackType, stackLevel, value)
        {
            AddBehaviour = addBehaviour;
            RemoveBehaviour = removeBehaviour;
        }

        public BoundedStatModifier(StackType stackType, int? stackLevel, float value, string sourceName,
            ChangeBehaviour addBehaviour, ChangeBehaviour removeBehaviour) : base(stackType, stackLevel, value, sourceName)
        {
            AddBehaviour = addBehaviour;
            RemoveBehaviour = removeBehaviour;
        }
    }
}
