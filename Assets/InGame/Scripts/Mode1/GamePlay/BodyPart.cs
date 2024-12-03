using UnityEngine;

public class BodyPart : MonoBehaviour
{
    [Header("Damage Multiplier")]
    [SerializeField] private float damageMultiplier = 1f; // Multiplier for this body part
    [SerializeField] bool Body, Head;
    private HealthManager healthManager;
    private AnimationController animationController;

    private void Start()
    {
        animationController = GetComponentInParent<AnimationController>();
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
            if (healthManager.currentHealth <= 0)
            {

                animationController.PlayAnimation(AnimationType.Dead);
            }

            else if (Body)
            {
                animationController.PlayAnimation(AnimationType.BodyHit);

            }
            else
            {
                animationController.PlayAnimation(AnimationType.HeadHit);

            }
        }
    }
}
