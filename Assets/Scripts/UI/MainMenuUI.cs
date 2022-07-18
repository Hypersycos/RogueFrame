using Hypersycos.RogueFrame.Networking;
using TMPro;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_InputField displayNameInputField;
        [SerializeField] private TMP_InputField ipAddressInputField;
        [SerializeField] private Unity.Netcode.Transports.UTP.UnityTransport transport;

        private void Start()
        {
            displayNameInputField.text = PlayerPrefs.GetString("PlayerName");
            ipAddressInputField.text = PlayerPrefs.GetString("IP", "127.0.0.1");
        }

        public void OnHostClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            PlayerPrefs.SetString("IP", ipAddressInputField.text);
            transport.ConnectionData.Address = ipAddressInputField.text;

            GameNetPortal.Instance.StartHost();
        }

        public void OnClientClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            PlayerPrefs.SetString("IP", ipAddressInputField.text);
            transport.ConnectionData.Address = ipAddressInputField.text;

            ClientGameNetPortal.Instance.StartClient();
        }
    }
}

