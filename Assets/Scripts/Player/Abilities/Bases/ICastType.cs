using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [System.Serializable]
    public abstract class ICastType
    {
        [SerializeField] [field: SerializeReference] protected List<ICastEffect> Effects;

        public List<ICastEffect> CloneEffects()
        {
            List<ICastEffect> clone = new();
            foreach(ICastEffect effect in Effects)
            {
                clone.Add(effect.Clone());
            }
            return clone;
        }

        public void BeforeCast(CharacterState caster, List<ICastEffect> castEffects)
        {
            foreach (ICastEffect effect in castEffects)
            {
                effect.Initialise(caster);
            }
        }
        public abstract void Cast(Vector3 cameraPosition, Quaternion lookDirection, CharacterState caster, List<ICastEffect> castEffects);

        public void OnHit(CharacterState character, List<ICastEffect> castEffects)
        {
            foreach (ICastEffect effect in castEffects)
            {
                effect.AffectCharacter(character);
            }
        }

        public void OnHit(GameObject hit, List<ICastEffect> castEffects)
        {
            foreach (ICastEffect effect in castEffects)
            {
                effect.AffectObject(hit);
            }
        }

        public void OnHit(CharacterState character, GameObject hit, List<ICastEffect> castEffects)
        {
            if (character == null)
            {
                OnHit(hit, castEffects);
            }
            else
            {
                OnHit(character, castEffects);
            }
        }
    }
}