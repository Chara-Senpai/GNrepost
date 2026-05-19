using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    //private static int nextSpawnIndex;
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>
        (
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            CurrentHealth.Value = maxHealth;
        }
        CurrentHealth.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        CurrentHealth.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int previousValue, int newValue)
    {
        Debug.Log($"{gameObject.name} chealth Hhange: {previousValue} -> {newValue}");
    }

    public void TakeDamage(int damageAmount)
    {
        if(!IsServer)
        {
            return;
        }

        CurrentHealth.Value -= damageAmount;
        CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value,0,maxHealth);
        if (CurrentHealth.Value <= 0)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        CurrentHealth.Value = maxHealth;
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        int randomIndex = Random.Range(0, spawnPointObjects.Length);
        Transform selectedSPawn = spawnPointObjects[randomIndex].transform;

        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = selectedSPawn.position;
        transform.rotation = selectedSPawn.rotation;

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }
}
