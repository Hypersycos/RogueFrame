using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public abstract class ProjectileAbility : Ability
    {
        public virtual NetworkObject Projectile { get; }
        public override void CastEffect(Vector3 cameraPosition, Quaternion lookDirection, PlayerState caster)
        {
            NetworkObject spawned = UnityEngine.Object.Instantiate(Projectile, cameraPosition, lookDirection);
            SetProjectileProperties(spawned, caster);
            spawned.Spawn();
        }

        protected abstract void SetProjectileProperties(NetworkObject spawned, PlayerState caster);
    }
}