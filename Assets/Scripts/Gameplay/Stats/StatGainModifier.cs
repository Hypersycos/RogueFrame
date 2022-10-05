using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class StatGainModifier : StatModifier
    {
        public StatGainModifier(StackType stackBehaviour, int? stackLevel, float value, string sourceName, Direction gainDirection) : base(stackBehaviour, stackLevel, value, sourceName)
        {
            GainDirection = gainDirection;
        }

        public StatGainModifier(StackType stackBehaviour, int? stackLevel, float value, Direction gainDirection) : base(stackBehaviour, stackLevel, value)
        {
            GainDirection = gainDirection;
        }

        public enum Direction
        {
            Positive,
            Negative,
            Both
        }
        public Direction GainDirection { get; protected set; }
    }
}
