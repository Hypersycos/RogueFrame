using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class PlayerState : CharacterState
    {
        void Start()
        {
            Energy = new BoundedStatInstance(100, 0, 100);
            Energy.AddModifier(new StatRegenerationModifier(StatModifier.StackType.Multiplicative, null, .25f));

            OverHealth.RemoveValue(400);
            Shields.AddModifier(new StatRegenerationModifier(StatModifier.StackType.Multiplicative, null, .25f, delay: 3));
            Health.AddModifier(new StatRegenerationModifier(StatModifier.StackType.Flat, null, 5));
            //Health.OnFill.AddListener((_, amountHealed, amountGiven) => OverHealth.AddValue(amountGiven - amountHealed));
            HitPoints = new DefensePool(new List<DefenseStatInstance>() { Health, Shields, OverHealth });
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Energy.Tick(Time.fixedDeltaTime);
        }

        [SerializeField] protected BoundedStatInstance Energy;
        [SerializeField] DefenseStatInstance Health = new DefenseStatInstance(100, new SemiBoundedStatInstance(150, 0));
        [SerializeField] DefenseStatInstance Shields = new DefenseStatInstance(50);
        [SerializeField] DefenseStatInstance OverHealth = new DefenseStatInstance(400);

        public bool UseEnergy(float amount)
        {
            return Energy.TryRemoveValue(amount);
        }

        public bool CanUseEnergy(float amount)
        {
            return Energy.CanRemoveValue(amount);
        }

        public void GiveEnergy(float amount)
        {
            Energy.AddValue(amount);
        }
        public override void AddStatus(StatusEffect effect, IStatusInstance instance)
        {
            throw new System.NotImplementedException();
        }
    }
}