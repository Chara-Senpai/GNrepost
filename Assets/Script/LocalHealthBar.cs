using UnityEngine;
using UnityEngine.UI;

public class LocalHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private NetworkPlayerHealth targetHealth;
    private int maxHealth;

    public void Setup(NetworkPlayerHealth healthScript, int maxHealth)
    {
        targetHealth = healthScript;
        this.maxHealth = maxHealth;

        targetHealth.CurrentHealth.OnValueChanged += UpdateHealthBar;
        UpdateHealthVisual(targetHealth.CurrentHealth.Value, maxHealth);
    }

    private void OnDestroy()
    {
        if (targetHealth != null)
        {
            targetHealth.CurrentHealth.OnValueChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(int previousHealth, int newHealth)
    {
        if(fillImage != null)
        {
            fillImage.fillAmount = (float)newHealth / maxHealth;
        }
    }

    private void UpdateHealthVisual(int currentHealth, int maxHealth)
    {
        if (fillImage != null)
        {
            // Fill Amount expects a fraction between 0.0f and 1.0f
            fillImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, 
                                    Camera.main.transform.rotation * Vector3.up);
        }   
    }
}
