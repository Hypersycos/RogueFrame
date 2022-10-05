using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Combat/Status Effect")]
    public class StatusEffect : ScriptableObject
    {
        public enum StackMethod
        {
            Additive,
            StackingRefresh,
            SingleRefresh,
            Instance
        }

        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public StackMethod StackType { get; private set; }
    }
}