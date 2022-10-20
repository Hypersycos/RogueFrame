using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class PlayerState : CharacterState
    {
        [SerializeField] TMP_Text DamageTickPrefab;
        ClientRpcParams clientRpcParams;
        void Start()
        {
            Team = 0;
            if (IsServer)
            {
                Energy.AddModifier(new StatRegenerationModifier(StatModifier.StackType.Multiplicative, null, .25f, null, delay: 0.2f));

                OverHealth.AddModifier(new StatRegenerationModifier(StatModifier.StackType.MultiplicativeAdditive, null, -0.2f, null, delay: 2));
                OverHealth.AddModifier(new StatRegenerationModifier(StatModifier.StackType.Flat, null, -5, null, delay: 2));
                Shields.AddModifier(new StatRegenerationModifier(StatModifier.StackType.Multiplicative, null, .25f, null, delay: 3));
                Health.AddModifier(new StatRegenerationModifier(StatModifier.StackType.Flat, null, 2, null, delay: 4, delayRate: 1f/4f));
                OnDamage.AddListener(CreateDamageNumber);

                OnHeal.AddListener(CreateHealNumber);

                clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { OwnerClientId }
                    }
                };
            }
            HitPoints = new DefensePool(new List<DefenseStatInstance>() { Health, Shields, OverHealth }, this);

            if (IsOwner)
            {
                GameObject.FindWithTag("HealthBar").GetComponent<StatBarScript>().AddStats(new List<BoundedStatInstance>() { Health, Shields, OverHealth });
                GameObject.FindWithTag("EnergyBar").GetComponent<StatBarScript>().AddStats(new List<BoundedStatInstance>() { Energy });
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Energy.Tick(Time.fixedDeltaTime);
        }

        [SerializeField] protected BoundedStatInstance Energy = new BoundedStatInstance(100, 0, 100);
        [SerializeField] DefenseStatInstance Health = new DefenseStatInstance(100, new SemiBoundedStatInstance(150, 0));
        [SerializeField] DefenseStatInstance Shields = new DefenseStatInstance(50);
        [SerializeField] DefenseStatInstance OverHealth = new DefenseStatInstance(400);

        public bool UseEnergy(float amount)
        {
            return Energy.TryRemoveValue(amount);
        }

        public bool CanUseEnergy(float amount)
        {
            return Energy.CanRemoveValue(amount);
        }

        public void GiveEnergy(float amount)
        {
            Energy.AddValue(amount);
        }

        private void CreateDamageNumber(CharacterState victim, DamageInstance damage)
        {
            Color c = victim.GetDamageColor();
            float damageNumber = damage.ActualAmount;
            float coefficient = (damageNumber / victim.HitPoints.MaxValue * 4 + 1);
            CreateDamageNumberClientRpc(victim.NetworkObject, (int)damageNumber, coefficient, c, clientRpcParams);
        }

        private void CreateHealNumber(CharacterState victim, DamageInstance heal)
        {
            Color c = victim.GetDamageColor();
            c = new Color(0, 1f, 0) * 0.7f + c * 0.3f;
            float healNumber = heal.ActualAmount;
            float coefficient = (healNumber / victim.HitPoints.MaxValue * 4 + 1);
            CreateDamageNumberClientRpc(victim.NetworkObject, (int)healNumber, coefficient, c, clientRpcParams);
        }

        [ClientRpc]
        private void CreateDamageNumberClientRpc(NetworkObjectReference victimRef, int number, float coefficient, Color c, ClientRpcParams clientRpcParams = default)
        {
            StatBarRotator statBarRotator = ((GameObject)victimRef).GetComponentInChildren<StatBarRotator>();
            TMP_Text instance = Instantiate(DamageTickPrefab, statBarRotator.transform);
            instance.color = c;
            instance.text = number.ToString();
            instance.fontSize = coefficient * 50 + 50;
            DamageInstanceScript script = instance.GetComponent<DamageInstanceScript>();
            float oldLife = script.lifetime;
            script.lifetime = script.lifetime * (coefficient / 4 + 0.5f);
            script.speed = script.speed * oldLife / script.lifetime;
            float angle = 100 - coefficient * 20;
            angle = Random.Range(-angle, angle) / 360 * 2*Mathf.PI;
            script.velocity = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        }
    }
}