using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class ProjectileScript : MonoBehaviour
    {
        [SerializeField] private Vector3 LaunchVelocity = new();
        [SerializeField] private float Lifetime = 0;
        [SerializeField] private float Range = 0;
        [SerializeField] private int MaxEnemyHits = 0;
        [SerializeField] private int MaxTerrainHits = 0;
        [SerializeField] private int MaxHits = 0;
        [SerializeField] private bool CanPunchthroughEnemies = false;
        [SerializeField] private bool CanPunchthroughTerrain = false;
        [SerializeField] private float MaxPunchthroughLength = 0;

        float distanceTravelled = 0;
        float PunchThrough = 0;
        Vector3? LastCollision = null;
        float Timer = 0;
        int Hits = 0;
        int EnemyHits = 0;
        int TerrainHits = 0;
        Vector3 StartPosition;
        List<CharacterState> alreadyHit = new();
        Action<CharacterState> OnHitChar;
        Action<GameObject> OnHitObj;

        private void OnTriggerEnter(Collider other)
        {
            Collided(other);
            if (LastCollision == null)
                LastCollision = transform.position;
        }
        private void OnCollisionEnter(Collision collision)
        {
            Collided(collision.collider);
            if (LastCollision == null)
                LastCollision = collision.GetContact(0).point;
        }

        private void Collided(Collider other)
        {
            CharacterState enemy = other.GetComponent<CharacterState>();
            if (enemy == null)
            {
                OnHitObj(other.gameObject);
                if (!CanPunchthroughTerrain) Expire();
                TerrainHits++;
            }
            else if (!alreadyHit.Contains(enemy))
            {
                OnHitChar(enemy);
                if (CanPunchthroughEnemies)
                    alreadyHit.Add(enemy);
                else
                    Expire();
                EnemyHits++;
            }
            Hits++;
            if (MaxHits > 0 && Hits >= MaxHits) Expire();
            if (MaxTerrainHits > 0 && TerrainHits >= MaxTerrainHits) Expire();
            if (MaxEnemyHits > 0 && EnemyHits >= MaxEnemyHits) Expire();
        }

        private void OnCollisionExit(Collision collision)
        {
            if (MaxPunchthroughLength > 0 && LastCollision != null)
            {
                Vector3 newPosition = collision.GetContact(0).point;
                PunchThrough += (newPosition - (Vector3)LastCollision).magnitude;
                if (PunchThrough > MaxPunchthroughLength)
                    Expire();
            }
            distanceTravelled += (StartPosition - transform.position).magnitude;
            StartPosition = transform.position;
        }

        private void OnTriggerExit(Collider other)
        {
            if (MaxPunchthroughLength > 0 && LastCollision != null)
            {
                PunchThrough += (transform.position - (Vector3)LastCollision).magnitude;
                if (PunchThrough > MaxPunchthroughLength)
                    Expire();
            }
        }

        public void Initialise(Action<CharacterState> onHit, Action<GameObject> onHitObj)
        {
            OnHitChar = onHit;
            OnHitObj = onHitObj;
            GetComponent<Rigidbody>().velocity = transform.rotation * LaunchVelocity;
            StartPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (Range > 0 && (StartPosition - transform.position).magnitude + distanceTravelled >= Range)
            {
                Expire();
            }
            if (Lifetime > 0)
            {
                Timer += Time.fixedDeltaTime;
                if (Timer > Lifetime)
                    Expire();
            }
        }

        public void Delete()
        {
            Destroy(gameObject);
        }

        public void Expire()
        {
            Destroy(gameObject);
        }
    }
}
