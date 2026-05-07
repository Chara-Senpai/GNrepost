using Unity.Netcode;
using UnityEngine;
using TMPro;

public class MultiplayerMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TextMeshProUGUI playerCountText;

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0);

    private void Start()
    {
        playerCountText.gameObject.SetActive(false);
    }
    public void StartHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            OnConnectionSuccess();
        }
    }

    public void StartClient()
    {
        if(NetworkManager.Singleton.StartClient())
        {
            OnConnectionSuccess();
        }
    }

    public void StartServer()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            OnConnectionSuccess();
        }
    }

    private void OnConnectionSuccess()
    {
        menuPanel.SetActive(false);
        playerCountText.gameObject.SetActive(true);

        playerCount.OnValueChanged += (oldVal, newVal) =>
        {
            playerCountText.text = $"Players Connected: {newVal}";
        };

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += UpdatedPlayerCount;
            NetworkManager.Singleton.OnClientDisconnectCallback += UpdatedPlayerCount;
            UpdatedPlayerCount(0);
        }
    }

    private void UpdatedPlayerCount(ulong clientId)
    {
        playerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}