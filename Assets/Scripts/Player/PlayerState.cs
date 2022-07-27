using System.Collections.Generic;

namespace Hypersycos.RogueFrame
{
    public class PlayerState : CharacterState
    {
        float Energy;
        int EnergyMax;
        float Shields;
        int MaxShields;
        float OverHealth;

        public override void AddStatus(StatusEffect effect, IStatusInstance instance)
        {
            throw new System.NotImplementedException();
        }
    }
}