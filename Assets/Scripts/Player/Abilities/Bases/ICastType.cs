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
        public abstract void Cast(Vector3 cameraPosition, Quaternion lookDirection, CharacterState caster);

        public void OnHit(CharacterState target, CharacterState owner, Vector3 location)
        {
            foreach (ICastEffect effect in Effects)
            {
                ICastEffect clone = effect.Clone();
                clone.Initialise(owner);
                clone.AffectCharacter(target, location);
            }
        }

        public void OnHit(GameObject hit, CharacterState owner, Vector3 location)
        {
            foreach (ICastEffect effect in Effects)
            {
                ICastEffect clone = effect.Clone();
                clone.Initialise(owner);
                clone.AffectObject(hit, location);
            }
        }

        public void OnHit(CharacterState character, GameObject hit, CharacterState owner, Vector3 location)
        {
            if (character == null)
            {
                OnHit(hit, owner, location);
            }
            else
            {
                OnHit(character, owner, location);
            }
        }
    }
}