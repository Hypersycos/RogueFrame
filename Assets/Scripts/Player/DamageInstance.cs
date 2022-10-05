using System;

namespace Hypersycos.RogueFrame
{
    public class DamageInstance
    {
        public bool isDamage;
        public float amount;
        public readonly CharacterState owner;
        public float actualAmount;
        public readonly CharacterState.CharacterStateEvent BeforeApply = new();
        public readonly CharacterState.CharacterStateEvent OnApply = new();
        public readonly CharacterState.CharacterStateEvent OnFullApply = new();

        public DamageInstance(bool isDamage, float amount, CharacterState owner)
        {
            this.isDamage = isDamage;
            this.amount = amount;
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            actualAmount = amount;
        }
        //TODO: List of modifiers which can only apply once
        //e.g. if a character is splitting its damage with someone else, they can't also
        //apply that since that would cause an infinite loop
    }
}