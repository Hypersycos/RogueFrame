using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hypersycos.RogueFrame.DefensePool;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.UI.GridLayoutGroup;

namespace Hypersycos.RogueFrame
{
    [CreateAssetMenu(fileName = "Detonate Effect", menuName = "Abilities/Fire/Detonate")]
    public class DetonateEffect : ICastEffect
    {
        [SerializeField] StatusEffect Heat;
        [SerializeField] float ExplosionRange;
        [SerializeField] LayerMask LayerMask;
        CharacterState Owner;

        public override void AffectCharacter(CharacterState state, Vector3 location)
        {
            List<StatusInstance> HeatInstances = state.GetStatusInstances(Heat);
            if (HeatInstances == null) return;

            float total = 0;
            foreach (StatusInstance h in HeatInstances)
            {
                HeatStatusInstance inst = (HeatStatusInstance)h;
                int ticks = (int)inst.duration + 1;
                total += ticks * inst.Amount;
                state.RemoveStatus(h);
            }
            foreach (Collider coll in Physics.OverlapSphere(state.transform.position, ExplosionRange, LayerMask))
            {
                CharacterState victimState = coll.gameObject.GetComponent<CharacterState>();
                if (victimState != null && victimState != state)
                {
                    victimState.ApplyDamageInstance(new DamageInstance(true, total, Owner, StatTypeTarget.AllValid));
                }
            }
        }

        public override void AffectObject(GameObject obj, Vector3 location)
        {
            throw new System.NotImplementedException();
        }

        public override void Initialise(CharacterState owner)
        {
            Owner = owner;
        }
    }
}
