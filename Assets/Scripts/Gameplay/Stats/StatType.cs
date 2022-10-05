using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [CreateAssetMenu(fileName = "New Stat Type", menuName = "Character/Stat Type")]
    public class StatType : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public float DefaultStartValue { get; private set; }
        [field: SerializeField] public bool DefaultStartIsPercentage { get; private set; }

    }
}
