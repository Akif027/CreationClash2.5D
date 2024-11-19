using UnityEngine;

public class BodyPart : MonoBehaviour
{
    [Header("Damage Multiplier")]
    [SerializeField] private float damageMultiplier = 1f; // Multiplier for this body part

    private HealthManager healthManager;

    private void Start()
    {
        // Assume the HealthManager is on the parent object
        healthManager = GetComponentInParent<HealthManager>();
        if (healthManager == null)
        {
            Debug.LogError("HealthManager not found on parent object.");
        }
    }

    /// <summary>
    /// Called when this body part takes damage.
    /// </summary>
    /// <param name="baseDamage">The base damage dealt.</param>
    public void TakeDamage(float baseDamage)
    {
        if (healthManager != null)
        {
            healthManager.ApplyDamage(baseDamage, damageMultiplier);
        }
    }
}
