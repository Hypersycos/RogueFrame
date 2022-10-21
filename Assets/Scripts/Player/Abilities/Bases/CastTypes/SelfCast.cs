using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class SelfCast : ICastType
    {
        public override TypeOfHit Cast(Vector3 cameraPosition, Quaternion lookDirection, CharacterState caster)
        {
            return OnHit(caster, caster, caster.transform.position);
        }
    }
}
