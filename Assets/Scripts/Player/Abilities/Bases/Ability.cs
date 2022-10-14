using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public abstract class Ability
    {
        public virtual string Name { get; }
        public virtual string Description { get; }
        public virtual bool NeedsRedraw { get; set; }
        public virtual IAbilityRequirement[] Requirements { get; }
        public virtual double CastTime { get; }
        public virtual double AnimationTime { get; }

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
        public abstract void CastEffect(Vector3 cameraPosition, Quaternion lookDirection, PlayerState caster);
        public abstract void DelayedCastEffect(Vector3 cameraPosition, Quaternion lookDirection, PlayerState caster);
        public abstract void DrawIcon(Canvas container);
    }
}