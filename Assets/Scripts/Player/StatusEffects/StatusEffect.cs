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

        public static string Name { get; protected set; }
        public static string Description { get; protected set; }
        public static StackMethod StackType { get; protected set; }
    }
}