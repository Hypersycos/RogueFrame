using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [System.Serializable]
    public class ProjectileCastType : ICastType
    {
        [SerializeField] private ProjectileScript Projectile;

        public override void Cast(Vector3 cameraPosition, Quaternion lookDirection, CharacterState caster, List<ICastEffect> castEffects)
        {
            ProjectileScript spawned = Object.Instantiate(Projectile, cameraPosition, lookDirection);
            spawned.Initialise((target) => OnHit(target, castEffects), (target) => OnHit(target, castEffects));
            spawned.GetComponent<NetworkObject>().Spawn();
        }
    }
}
