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
    }
}
