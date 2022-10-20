using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;

namespace Hypersycos.RogueFrame
{
    public abstract class CharacterState : NetworkBehaviour
    {
        void Start()
        {
            DefenseStatInstance Health = new DefenseStatInstance(100);
            HitPoints = new DefensePool(new List<DefenseStatInstance>() { Health}, this);
        }

        protected virtual void FixedUpdate()
        {
            HitPoints.Tick(Time.fixedDeltaTime);
            List<StatusInstance> ToRemove = new();
            foreach(List<StatusInstance> instances in statusInstances.Values)
            {
                foreach(StatusInstance inst in instances)
                {
                    if (inst is DurationStatusInstance)
                    {
                        DurationStatusInstance dInst = (DurationStatusInstance)inst;
                        dInst.duration -= Time.fixedDeltaTime;
                        if (dInst.duration < 0)
                        {
                            ToRemove.Add(inst);
                        }
                    }
                }
            }
            foreach(StatusInstance inst in ToRemove)
            {
                RemoveStatus(inst);
            }
        }
        public class CharacterStateHealthEvent : UnityEvent<CharacterState, DamageInstance> { }
        public class CharacterStateStatusEvent : UnityEvent<CharacterState, StatusInstance> { }

        readonly Dictionary<StatusEffect, List<StatusInstance>> statusInstances = new();
        public int Team;

        public DefensePool HitPoints { get; protected set; }

        //OnDamaged should not be invoked when the character is killed
        public CharacterStateHealthEvent OnDamaged = new();
        public CharacterStateHealthEvent OnExternallyDamaged = new();
        public CharacterStateHealthEvent OnHealed = new();
        public CharacterStateHealthEvent OnExternallyHealed = new();
        public CharacterStateHealthEvent OnKilled = new();
        public CharacterStateHealthEvent OnFullyHealed = new();
        public CharacterStateHealthEvent BeforeDamaged = new();
        public CharacterStateHealthEvent BeforeHealed = new();

        public CharacterStateHealthEvent OnDamage = new();
        public CharacterStateHealthEvent OnHeal = new();
        public CharacterStateHealthEvent OnFullHeal = new();
        public CharacterStateHealthEvent OnKill = new();
        public CharacterStateHealthEvent BeforeDamage = new();
        public CharacterStateHealthEvent BeforeHeal = new();

        public int GetStatusCount(StatusEffect statusEffect)
        {
            return statusInstances.ContainsKey(statusEffect) ? statusInstances[statusEffect].Count : 0;
        }
        public void AddStatus(StatusInstance instance)
        {
            if (!statusInstances.ContainsKey(instance.StatusEffect))
            {
                statusInstances.Add(instance.StatusEffect, new List<StatusInstance>());
                statusInstances[instance.StatusEffect].Add(instance);
                ApplyStatus(instance);
            }
            else
            {
                List<StatusInstance> insts = statusInstances[instance.StatusEffect];
                switch (instance.StatusEffect.StackType)
                {
                    case StatusEffect.StackMethod.Additive:
                        insts[0].Combine(instance);
                        break;
                    case StatusEffect.StackMethod.StackingRefresh:
                        foreach(StatusInstance inst in insts)
                        {
                            inst.Refresh(instance);
                        }
                        insts.Add(instance);
                        break;
                    case StatusEffect.StackMethod.SingleRefresh:
                        insts[0].Refresh(instance);
                        break;
                    case StatusEffect.StackMethod.Instance:
                        insts.Add(instance);
                        ApplyStatus(instance);
                        break;
                    case StatusEffect.StackMethod.SingleInstance:
                        insts[0].Refresh(instance);
                        break;
                    case StatusEffect.StackMethod.SingleInstancePerSource:
                        bool Found = false;
                        foreach (StatusInstance inst in insts)
                        {
                            if (inst.owner == instance.owner)
                            {
                                inst.Refresh(instance);
                                Found = true;
                                break;
                            }
                        }
                        if (!Found)
                        {
                            insts.Add(instance);
                            ApplyStatus(instance);
                        }
                        break;
                }
            }
        }

        public void RemoveStatus(StatusInstance instance)
        {
            List<StatusInstance> insts = statusInstances[instance.StatusEffect];
            insts.Remove(instance);
            UnapplyStatus(instance);
        }

        public virtual void ApplyStatus(StatusInstance instance)
        {
            instance.Apply(this, StartCoroutine);
        }

        public virtual void UnapplyStatus(StatusInstance instance)
        {
            instance.Remove(this);
        }

        public void ApplyDamageInstance(DamageInstance damageInstance, bool external=true)
        {
            if (!damageInstance.IsDamage) return;
            damageInstance.BeforeApply.Invoke(this, damageInstance);
            BeforeDamaged.Invoke(this, damageInstance);

            HitPoints.Damage(damageInstance);
            float damage = damageInstance.ActualAmount;

            if (damage > 0)
            {
                OnDamaged.Invoke(this, damageInstance);
                if (external)
                    OnExternallyDamaged.Invoke(this, damageInstance);
                damageInstance.OnApply.Invoke(this, damageInstance);
                damageInstance.owner.OnDamage.Invoke(this, damageInstance);

                if (!HitPoints.IsActive)
                {
                    OnKilled.Invoke(this, damageInstance);
                    damageInstance.OnFullApply.Invoke(this, damageInstance);
                    damageInstance.owner.OnKill.Invoke(this, damageInstance);
                }
            }
        }

        public void ApplyHealInstance(DamageInstance healInstance, bool external=true)
        {
            if (healInstance.IsDamage) return;
            healInstance.BeforeApply.Invoke(this, healInstance);
            BeforeHealed.Invoke(this, healInstance);

            HitPoints.Heal(healInstance);
            float heal = healInstance.ActualAmount;

            if (heal > 0)
            {
                OnHealed.Invoke(this, healInstance);
                if (external)
                    OnExternallyHealed.Invoke(this, healInstance);
                healInstance.OnApply.Invoke(this, healInstance);
                healInstance.owner.OnHeal.Invoke(this, healInstance);

                if (HitPoints.Value == HitPoints.MaxValue)
                {
                    OnFullyHealed.Invoke(this, healInstance);
                    healInstance.OnFullApply.Invoke(this, healInstance);
                    healInstance.owner.OnFullHeal.Invoke(this, healInstance);
                }
            }
        }

        public Color GetDamageColor()
        {
            return HitPoints.GetDamageColor();
        }
    }
}