using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class HitscanCastType : ICastType
    {
        [SerializeField] float MaxLength;
        [SerializeField] LayerMask Layers;
        [SerializeField] QueryTriggerInteraction HitTriggers;
        public override void Cast(Vector3 cameraPosition, Quaternion lookDirection, CharacterState caster)
        {
            RaycastHit hit;
            bool success;
            if (MaxLength <= 0) MaxLength = Mathf.Infinity;
            success = Physics.Raycast(cameraPosition, lookDirection * new Vector3(0, 0, 1), out hit, MaxLength, Layers, HitTriggers);
            if (success)
            {
                CharacterState target = hit.collider.GetComponent<CharacterState>();
                OnHit(target, hit.collider.gameObject, caster, hit.point);
            }
        }
    }
}
