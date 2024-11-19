using UnityEngine;
public class HealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public float currentHealth;

    private void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

    }

    /// <summary>
    /// Applies damage to the player.
    /// </summary>
    /// <param name="baseDamage">The base damage dealt.</param>
    /// <param name="damageMultiplier">The multiplier for the body part hit.</param>
    public void ApplyDamage(float baseDamage, float damageMultiplier)
    {
        float damage = baseDamage * damageMultiplier;
        currentHealth -= damage;
        Debug.Log($"Damage Applied: {damage}. Current Health: {currentHealth}");

        GameManager.Instance.ShowFloatingText(damage.ToString(), transform);
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        Debug.Log("Player is dead!");
        UIManager.Instance.ToggleGameOverPanel(true);
        Destroy(gameObject);
        // Handle player death logic here
    }

    /// <summary>
    /// Gets the current health percentage for UI or logic.
    /// </summary>
    /// <returns>Health percentage (0-1).</returns>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}
