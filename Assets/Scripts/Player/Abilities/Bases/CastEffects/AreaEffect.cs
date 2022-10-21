using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [CreateAssetMenu(fileName = "New Area Effect", menuName = "Abilities/Area Effect")]
    public class AreaEffect : ICastEffect
    {
        [SerializeField] protected float Range;
        [SerializeField] protected List<TypeOfHit> ValidHits;
        [SerializeField] protected LayerMask LayerMask;
        [SerializeField] protected List<ICastEffect> Effects;
        protected CharacterState Owner;
        public override void AffectCharacter(CharacterState characterState, Vector3 location)
        {
            if (ValidHits.Contains(TypeOfHit.CharacterHit))
                GetTargets(location);
        }

        private void GetTargets(Vector3 location)
        {
            foreach(Collider coll in Physics.OverlapSphere(location, Range, LayerMask))
            {
                CharacterState state = coll.gameObject.GetComponent<CharacterState>();
                if (state == null)
                {
                    ObjectEffect(coll.gameObject);
                }
                else
                {
                    CharacterEffect(state);
                }
            }
        }

        public virtual void CharacterEffect(CharacterState state)
        {
            foreach(ICastEffect effect in Effects)
            {
                ICastEffect clone = effect.Clone();
                clone.Initialise(Owner);
                clone.AffectCharacter(state, state.transform.position);
            }
        }
        public virtual void ObjectEffect(GameObject obj)
        {
            foreach (ICastEffect effect in Effects)
            {
                ICastEffect clone = effect.Clone();
                clone.Initialise(Owner);
                clone.AffectObject(obj, obj.transform.position);
            }
        }

        public override void AffectObject(GameObject obj, Vector3 location)
        {
            if (obj != null)
            {
                if (ValidHits.Contains(TypeOfHit.CharacterHit))
                    GetTargets(location);
            }
            else
            {
                if (ValidHits.Contains(TypeOfHit.NoHit))
                    GetTargets(location);
            }
        }

        public override void Initialise(CharacterState owner)
        {
            Owner = owner;
        }
    }
}
