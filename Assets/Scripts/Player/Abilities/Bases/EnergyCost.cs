using System;
using Unity.Netcode.Components;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class EnergyCost : IAbilityRequirement
    {
        public int Energy { get; }
        bool IAbilityRequirement.CanCast(PlayerState state)
        {
            return state.CanUseEnergy(Energy);
        }

        bool IAbilityRequirement.Charge(PlayerState state)
        {
            return state.UseEnergy(Energy);
        }

        void IAbilityRequirement.Refund(PlayerState state)
        {
            state.GiveEnergy(Energy);
        }

        public EnergyCost(int energy)
        {
            Energy = energy;
        }
    }
}