using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [System.Serializable]
    public class IgniteStatusInstance : StatusInstance
    {
        [SerializeField] StatusEffect Heat;
        public float Strength;
        public float Duration;

        public IgniteStatusInstance(float strength, float duration, CharacterState owner, StatusEffect Ignite, StatusEffect Heat) : base(strength*duration, owner, Ignite)
        {
            this.Heat = Heat;
            Strength = strength;
            Duration = duration;
        }

        public IgniteStatusInstance() : base(0, null) { }

        public void AddHeat(CharacterState victim, DamageInstance damage)
        {
            StatusInstance HeatInstance = new HeatStatusInstance(damage.Amount / Heat.DefaultDuration * 2 * Strength, damage.owner, Heat, Heat.DefaultDuration * Duration);
            victim.AddStatus(HeatInstance);
        }

        private IEnumerator Coroutine(CharacterState victim)
        {
            victim.OnExternallyDamaged.AddListener(AddHeat);
            yield return new WaitForSeconds(1f);
            while (victim.GetStatusCount(Heat) > 0)
            {
                while (victim.GetStatusCount(Heat) > 0)
                    yield return new WaitForFixedUpdate();
                yield return new WaitForSeconds(1f);
            }
            victim.RemoveStatus(this);
        }

        public override void Combine(StatusInstance other)
        {
            if (other.Amount > Amount)
            {
                IgniteStatusInstance castOther = (IgniteStatusInstance)other;
                Duration = castOther.Duration;
                Strength = castOther.Strength;
            }
        }

        public override void Refresh(StatusInstance other)
        {
            if (other.Amount > Amount)
            {
                IgniteStatusInstance castOther = (IgniteStatusInstance)other;
                Duration = castOther.Duration;
                Strength = castOther.Strength;
            }
        }

        public override void Apply(CharacterState victim, Func<IEnumerator, Coroutine> Start)
        {
            Start(Coroutine(victim));
        }

        public override void Remove(CharacterState victim)
        {
            victim.OnExternallyDamaged.RemoveListener(AddHeat);
        }
    }
}
