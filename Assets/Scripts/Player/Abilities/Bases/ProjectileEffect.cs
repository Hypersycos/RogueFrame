using Unity.Netcode;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class ProjectileEffect : ICastEffect
    {
        [SerializeField] private ProjectileScript Projectile;
        void ICastEffect.Cast(Vector3 cameraPosition, Quaternion lookDirection, PlayerState caster)
        {
            ProjectileScript spawned = Object.Instantiate(Projectile, cameraPosition, lookDirection);
            spawned.Initialise(caster);
            spawned.GetComponent<NetworkObject>().Spawn();
        }
    }
}
