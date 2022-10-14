using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class ProjectileScript : MonoBehaviour
    {
        public DamageInstance damageInstance;
        [SerializeField] private Vector3 LaunchVelocity = new();
        private void OnCollisionEnter(Collision collision)
        {
            EnemyState enemy = collision.collider.GetComponent<EnemyState>();
            if (enemy == null)
            {
                Destroy(gameObject);
            }
            else
            {
                enemy.ApplyDamageInstance(damageInstance);
                Destroy(gameObject);
            }
        }

        public void Initialise(PlayerState caster)
        {
            damageInstance.SetOwner(caster);
            GetComponent<Rigidbody>().velocity = transform.rotation * LaunchVelocity;
        }
    }
}
