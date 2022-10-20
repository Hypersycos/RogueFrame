using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [System.Serializable]
    public abstract class ICastEffect : ScriptableObject
    {
        public abstract void AffectCharacter(CharacterState characterState);
        public abstract void AffectObject(GameObject obj);
        public abstract void Initialise(CharacterState owner);
        public ICastEffect Clone()
        {
            return Instantiate(this);
        }
    }
}
