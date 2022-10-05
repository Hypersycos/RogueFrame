using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hypersycos.RogueFrame
{
    public class PlayerAbilityController : NetworkBehaviour
    {
        // Start is called before the first frame update
        private Controls ControlAsset;
        [SerializeField] private new Transform camera;
        [SerializeField] private Transform cameraRoot;
        private bool castOnSelect = true;
        private ushort currentAbility = 0;
        private ushort lastCastAbility = 0;
        private Quaternion? lastCastRotation = null;
        private double clientCastLockout = 0;
        private double serverCastLockout = 0;
        [SerializeField] private List<Ability> abilities = new List<Ability>() { new TestProjectileAbility()};
        [SerializeField] private PlayerState playerState;
        private float castSpeed = 1; //TODO: replace with generic stat
        [SerializeField] private Vector3 cameraOffset = new Vector3(0.7f, 0, -1);
        public override void OnNetworkSpawn()
        {
            if (IsLocalPlayer)
            {
                ControlAsset = GetComponent<PlayerMovementController>().ControlAsset;
                ControlAsset.Player.Ability1.started += SetAbility;
                ControlAsset.Player.Ability2.started += SetAbility;
                ControlAsset.Player.Ability3.started += SetAbility;
                ControlAsset.Player.Ability4.started += SetAbility;
                ControlAsset.Player.Ultimate.started += CastUltimate;
                ControlAsset.Player.ChangeAbility.started += NextAbility;
                ControlAsset.Player.UseAbility.started += CastAbility;
                camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
            }
            cameraRoot = transform.GetChild(0);
        }

        public override void OnNetworkDespawn()
        {
            if (IsLocalPlayer)
            {
                ControlAsset.Player.Ability1.started -= SetAbility;
                ControlAsset.Player.Ability2.started -= SetAbility;
                ControlAsset.Player.Ability3.started -= SetAbility;
                ControlAsset.Player.Ability4.started -= SetAbility;
                ControlAsset.Player.Ultimate.started -= CastUltimate;
                ControlAsset.Player.ChangeAbility.started -= NextAbility;
                ControlAsset.Player.UseAbility.started -= CastAbility;
            }
        }

        private void CastAbility(InputAction.CallbackContext obj)
        {
            if (clientCastLockout >= 0 && clientCastLockout < NetworkManager.ServerTime.Time)
            {
                clientCastLockout = -1;
                CastServerRpc(currentAbility, camera.rotation);
                Ability selected = abilities[currentAbility];
                lastCastAbility = currentAbility;
                if (selected.CastTime > 0)
                {
                    StartCoroutine(ClientDelayedCast(selected.CastTime));
                }
            }
        }

        IEnumerator ClientDelayedCast(double waitTime)
        {
            while (waitTime > 0f)
            {
                waitTime -= Time.fixedDeltaTime * castSpeed;
                yield return new WaitForFixedUpdate();
            }
            DelayedCastServerRpc(camera.rotation);
        }

        [ClientRpc] 
        private void CastResultClientRpc(double lockout, double castDelay)
        {
            clientCastLockout = lockout;
        }

        [ServerRpc]
        private void CastServerRpc(ushort abilityIndex, Quaternion lookDirection)
        {
            if (serverCastLockout >= 0 && serverCastLockout < NetworkManager.ServerTime.Time)
            {
                serverCastLockout = -1;
            }
            else if (serverCastLockout == -1)
            {
                return;
            }
            else
            {
                CastResultClientRpc(serverCastLockout, -1);
                return;
            }
            Ability ability = abilities[abilityIndex];
            if (ability.CastCost(playerState))
            {
                Vector3 cameraPosition = cameraRoot.position + lookDirection * cameraOffset;
                ability.CastEffect(cameraPosition, lookDirection, playerState);
                serverCastLockout = NetworkManager.ServerTime.Time + ability.AnimationTime;
                CastResultClientRpc(serverCastLockout, ability.CastTime);
                if (ability.CastTime > 0)
                {
                    StartCoroutine(ServerDelayedCast(NetworkManager.ServerTime.Time + ability.CastTime, lookDirection));
                }
                lastCastAbility = abilityIndex;
            }
            else
            {
                CastResultClientRpc(0, -1);
                serverCastLockout = 0;
            }
        }

        [ServerRpc]
        private void DelayedCastServerRpc(Quaternion lookDirection)
        {
            lastCastRotation = lookDirection;
        }

        IEnumerator ServerDelayedCast(double expectedCastTime, Quaternion oldLookDirection)
        {
            while (NetworkManager.ServerTime.Time < expectedCastTime)
            {
                if (castSpeed != 1)
                {
                    expectedCastTime += (1 - castSpeed) * Time.fixedDeltaTime;
                }
                yield return new WaitForFixedUpdate();
            }
            float timeout = 0.25f;
            while (lastCastRotation == null && timeout > 0 )
            {
                timeout -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            Ability delayedAbility = abilities[lastCastAbility];
            Quaternion lookDirection = lastCastRotation ?? oldLookDirection;
            Vector3 cameraPosition = cameraRoot.position + lookDirection * cameraOffset;
            delayedAbility.DelayedCastEffect(cameraPosition, lookDirection);
        }

        private void NextAbility(InputAction.CallbackContext obj)
        {
            currentAbility += 1;
            if (currentAbility > 3)
            {
                currentAbility = 0;
            }
        }

        private void CastUltimate(InputAction.CallbackContext obj)
        {
            CastServerRpc(4, camera.rotation);
        }

        private void SetAbility(InputAction.CallbackContext obj)
        {
            switch(obj.action.name)
            {
                case "Ability 1":
                    currentAbility = 0;
                    break;
                case "Ability 2":
                    currentAbility = 1;
                    break;
                case "Ability 3":
                    currentAbility = 2;
                    break;
                case "Ability 4":
                    currentAbility = 3;
                    break;
            }
            if (castOnSelect)
            {
                CastAbility(obj);
            }
        }
    }
}