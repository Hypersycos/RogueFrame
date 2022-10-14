using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{

    [CreateAssetMenu(fileName = "New Ability", menuName = "Character/Ability")]
    public class AbilitySO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        public bool NeedsRedraw;
        [SerializeField] [field: SerializeReference] private List<IAbilityRequirement> Requirements = new();
        [SerializeField] [field: SerializeReference] private List<ICastEffect> CastEffects;
        [SerializeField] [field: SerializeReference] private List<ICastEffect> DelayedCastEffects;
        [SerializeField] [field: SerializeReference] private List<IDrawIcon> IconPainters;
        [field: SerializeField] public double CastTime { get; private set; }
        [field: SerializeField] public double AnimationTime { get; private set; }

        public bool CanCast(PlayerState state)
        {
            foreach (IAbilityRequirement requirement in Requirements)
            {
                if (!requirement.CanCast(state))
                {
                    return false;
                }
            }
            return true;
        }
        public bool CastCost(PlayerState state)
        {
            if (!CanCast(state))
            {
                return false;
            }
            foreach (IAbilityRequirement requirement in Requirements)
            {
                requirement.Charge(state);
            }
            return true;
        }
        public void CastEffect(Vector3 cameraPosition, Quaternion lookDirection, PlayerState caster)
        {
            foreach (ICastEffect castEffect in CastEffects)
            {
                castEffect.Cast(cameraPosition, lookDirection, caster);
            }
        }
        public void DelayedCastEffect(Vector3 cameraPosition, Quaternion lookDirection, PlayerState caster)
        {
            foreach (ICastEffect castEffect in DelayedCastEffects)
            {
                castEffect.Cast(cameraPosition, lookDirection, caster);
            }
        }
        public void QuickDrawIcon(Canvas container)
        {
            foreach (IDrawIcon drawIcon in IconPainters)
            {
                drawIcon.QuickDrawIcon(container);
            }
        }

        public void FullDrawIcon(Canvas container)
        {
            foreach (Transform child in container.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (IDrawIcon drawIcon in IconPainters)
            {
                drawIcon.FullDrawIcon(container);
            }
        }
    }
}