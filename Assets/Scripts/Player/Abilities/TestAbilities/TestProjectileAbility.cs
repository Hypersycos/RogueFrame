using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Hypersycos.RogueFrame
{
    public class TestProjectileAbility : ProjectileAbility
    {
        public override NetworkObject Projectile => Resources.Load("Projectile").GetComponent<NetworkObject>();//Addressables.LoadAssetAsync<GameObject>("Projectile");

        public override string Name => "ProjectileAbility";
        public override string Description => "Projectiles";
        public override double CastTime => 0;

        public override IAbilityRequirement[] Requirements => new[] { new EnergyCost(25) };

        public override double AnimationTime => 0;

        public override void DrawIcon(Canvas container)
        {
            throw new System.NotImplementedException();
        }

        public override void DelayedCastEffect(Vector3 cameraPosition, Quaternion lookDirection)
        {
            throw new System.NotImplementedException();
        }

        protected override void SetProjectileProperties(NetworkObject spawned, PlayerState caster)
        {
            Rigidbody rb = spawned.GetComponent<Rigidbody>();
            rb.velocity = spawned.transform.rotation * new Vector3(0, 0, 10);
            ProjectileScript script = spawned.GetComponent<ProjectileScript>();
            script.damageInstance = new DamageInstance(true, 30, caster);
        }
    }
}