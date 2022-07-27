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
        //TODO: List of modifiers which can only apply once
        //e.g. if a character is splitting its damage with someone else, they can't also
        //apply that since that would cause an infinite loop
    }
}