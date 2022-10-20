using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hypersycos.RogueFrame.DefensePool;

namespace Hypersycos.RogueFrame
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Damage Effect", menuName = "Abilities/Damage Effect")]
    public class DamageEffect : ICastEffect
    {
        DamageInstance damageInstance;
        [SerializeField] float Amount;
        [SerializeField] StatTypeTarget ValidTargets;
        public override void AffectCharacter(CharacterState characterState)
        {
            characterState.ApplyDamageInstance(damageInstance);
        }

        public override void AffectObject(GameObject obj)
        {
            return;
        }

        public override void Initialise(CharacterState owner)
        {
            damageInstance = new DamageInstance(true, Amount, owner, ValidTargets);
        }
    }
}
