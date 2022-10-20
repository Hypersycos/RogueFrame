using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hypersycos.RogueFrame.DefensePool;

namespace Hypersycos.RogueFrame
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Heal Effect", menuName = "Abilities/Heal Effect")]
    public class HealEffect : ICastEffect
    {
        DamageInstance healInstance;
        [SerializeField] float Amount;
        [SerializeField] StatTypeTarget ValidTargets;
        public override void AffectCharacter(CharacterState characterState)
        {
            characterState.ApplyHealInstance(healInstance);
        }

        public override void AffectObject(GameObject obj)
        {
            return;
        }

        public override void Initialise(CharacterState owner)
        {
            healInstance = new DamageInstance(false, Amount, owner, ValidTargets);
        }
    }
}
