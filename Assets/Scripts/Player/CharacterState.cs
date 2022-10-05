using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;

namespace Hypersycos.RogueFrame
{
    public abstract class CharacterState : MonoBehaviour
    {
        void Start()
        {
            DefenseStatInstance Health = new DefenseStatInstance(100);
            HitPoints = new DefensePool(new List<DefenseStatInstance>() { Health});
        }

        protected virtual void FixedUpdate()
        {
            HitPoints.Tick(Time.fixedDeltaTime);
        }
        public class CharacterStateEvent : UnityEvent<CharacterState, DamageInstance> { }

        readonly Dictionary<StatusEffect, List<IStatusInstance>> statusInstances = new();

        [SerializeField] protected DefensePool HitPoints;

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

        public void ApplyDamageInstance(DamageInstance damageInstance)
        {
            if (!damageInstance.isDamage) return;
            damageInstance.BeforeApply.Invoke(this, damageInstance);
            BeforeDamaged.Invoke(this, damageInstance);

            float damage = HitPoints.RemoveValue(damageInstance.actualAmount);
            damageInstance.actualAmount = damage;

            if (damage > 0)
            {
                if (!HitPoints.IsActive)
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
        }

        public void ApplyHealInstance(DamageInstance healInstance)
        {
            if (healInstance.isDamage) return;
            healInstance.BeforeApply.Invoke(this, healInstance);
            BeforeHealed.Invoke(this, healInstance);

            float heal = HitPoints.AddValue(healInstance.actualAmount);
            healInstance.actualAmount = heal;

            if (heal > 0)
            {
                if (HitPoints.Value == HitPoints.MaxValue)
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
}