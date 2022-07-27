using System.Collections.Generic;
using UnityEngine.Events;

namespace Hypersycos.RogueFrame
{
    public abstract class CharacterState
    {
        public class CharacterStateEvent : UnityEvent<CharacterState, DamageInstance> { }

        public float Health;
        public int MaxHealth;
        readonly Dictionary<StatusEffect, List<IStatusInstance>> statusInstances = new();

        //OnDamaged should not be invoked when the character is killed
        public CharacterStateEvent OnDamaged = new();
        public CharacterStateEvent OnHealed = new();
        public CharacterStateEvent OnKilled = new();
        public CharacterStateEvent OnFullyHealed = new();
        public CharacterStateEvent BeforeDamaged = new();
        public CharacterStateEvent BeforeHealed = new();

        public CharacterStateEvent OnDamage = new();
        public CharacterStateEvent OnHeal = new();
        public CharacterStateEvent OnFullHeal = new();
        public CharacterStateEvent OnKill = new();
        public CharacterStateEvent BeforeDamage = new();
        public CharacterStateEvent BeforeHeal = new();

        public abstract void AddStatus(StatusEffect effect, IStatusInstance instance);

        protected virtual KeyValuePair<float, bool> ApplyDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                return new KeyValuePair<float, bool>(damage - Health, true);
            }
            else
            {
                return new KeyValuePair<float, bool>(damage, false);
            }
        }
        public void ApplyDamageInstance(DamageInstance damageInstance)
        {
            if (!damageInstance.isDamage) return;
            damageInstance.BeforeApply.Invoke(this, damageInstance);
            BeforeDamaged.Invoke(this, damageInstance);

            KeyValuePair<float, bool> result = ApplyDamage(damageInstance.actualAmount);
            damageInstance.actualAmount = result.Key;

            if (result.Value)
            {
                OnKilled.Invoke(this, damageInstance);
                damageInstance.OnFullApply.Invoke(this, damageInstance);
            }
            else
            {
                OnDamaged.Invoke(this, damageInstance);
                damageInstance.OnApply.Invoke(this, damageInstance);
            }
        }

        protected virtual KeyValuePair<float, bool> ApplyHeal(float heal)
        {
            Health += heal;
            if (Health > MaxHealth)
            {
                return new KeyValuePair<float, bool>(heal - Health + MaxHealth, true);
            }
            else
            {
                return new KeyValuePair<float, bool>(heal, false);
            }
        }

        public void ApplyHealInstance(DamageInstance healInstance)
        {
            if (healInstance.isDamage) return;
            healInstance.BeforeApply.Invoke(this, healInstance);
            BeforeHealed.Invoke(this, healInstance);

            KeyValuePair<float, bool> result = ApplyHeal(healInstance.amount);
            healInstance.actualAmount = result.Key;

            if (result.Value)
            {
                OnFullyHealed.Invoke(this, healInstance);
                healInstance.OnFullApply.Invoke(this, healInstance);
            }
            else
            {
                OnHealed.Invoke(this, healInstance);
                healInstance.OnApply.Invoke(this, healInstance);
            }
        }
    }
}