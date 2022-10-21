using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public interface IAbilityRequirement
    {
        bool CanCast(PlayerState state);
        bool Charge(PlayerState state);
        void Refund(PlayerState state);
        //TODO: Conditional not implemented
        void Conditional(PlayerState state, AbilityResult result);
    }

    public enum TypeOfHit
    {
        CharacterHit,
        ObjectHit,
        NoHit,
        NotApplicable
    }

    public class AbilityResult
    {
        public TypeOfHit typeOfHit;
        public AbilityResult(TypeOfHit typeOfHit)
        {
            this.typeOfHit = typeOfHit;
        }
    }
}
