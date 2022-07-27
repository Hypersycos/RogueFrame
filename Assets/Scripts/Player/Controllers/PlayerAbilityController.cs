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
        private bool castOnSelect = true;
        private ushort currentAbility = 0;
        [SerializeField] private List<IAbility> abilities;
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
            }
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
            CastServerRpc(currentAbility);
        }

        [ServerRpc]
        private void CastServerRpc(int ability)
        {
            
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
            CastServerRpc(4);
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