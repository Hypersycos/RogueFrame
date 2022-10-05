using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class EnemyState : CharacterState
    {
        void Start()
        {
            Health.OnEmpty.AddListener((_,_,_) => StartCoroutine(FullHealAfter(3)));
            HitPoints = new DefensePool(new List<DefenseStatInstance>() { Health });
        }

        IEnumerator FullHealAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Health.AddValue(10000);
        }

        [SerializeField] DefenseStatInstance Health = new DefenseStatInstance(100);

        public override void AddStatus(StatusEffect effect, IStatusInstance instance)
        {
            throw new System.NotImplementedException();
        }
    }
}