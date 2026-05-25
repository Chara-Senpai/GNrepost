using UnityEngine;
using Unity.Netcode;
using System.Runtime.Serialization;
using System.Numerics;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private GameObject floatingTextPrefab;

    [SerializeField] private GameObject healthBarPrefab;
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

        SpawnLocalHealthBar();
    }

    private void SpawnLocalHealthBar()
    {
        if (healthBarPrefab == null) return;

        UnityEngine.Vector3 barPosition = transform.position + UnityEngine.Vector3.up;

        GameObject barInstance = Instantiate(healthBarPrefab, barPosition, UnityEngine.Quaternion.identity, transform);

        if (barInstance.TryGetComponent<LocalHealthBar>(out var healthBar))
        {
            healthBar.Setup(this, maxHealth);
        }
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
        ShowDamageTextClientRpc(damageAmount, transform.position);

        if (CurrentHealth.Value <= 0)
        {
            Respawn();
        }
    }

    [ClientRpc]
    private void ShowDamageTextClientRpc(int damageAmount, UnityEngine.Vector3 playerPosition)
    {
        if (floatingTextPrefab != null) return;

        UnityEngine.Vector3 spawnPosition = playerPosition + UnityEngine.Vector3.up * 3.5f;

        spawnPosition += new UnityEngine.Vector3(Random.Range(-0.4f, 0.4f), Random.Range(0f, 0.3f), Random.Range(-0.4f, 0.4f));
        GameObject textInstance = Instantiate(floatingTextPrefab, spawnPosition, UnityEngine.Quaternion.identity);

        if (textInstance.TryGetComponent<FloatingText>(out var floatingText))
        {
            floatingText.SetText(damageAmount.ToString());
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
