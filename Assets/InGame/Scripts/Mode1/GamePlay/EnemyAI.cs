using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Weapon Manager")]
    [SerializeField] private EnemyWeaponManager enemyWeaponManager;

    [Header("Target Landing Range")]
    [SerializeField] private Vector2 minRange = new Vector2(-10f, 0f);
    [SerializeField] private Vector2 maxRange = new Vector2(-20f, 5f);

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnInterval = 2f;
    [SerializeField] private float maxSpawnInterval = 5f;

    [Header("Animation")]
    [SerializeField] private AnimationController animationController;

    private bool canSpawn = true;
    private Vector3 targetPosition;

    private void Start()
    {
        if (enemyWeaponManager == null)
        {
            Debug.LogError("EnemyWeaponManager is not assigned to EnemyAI!");
            return;
        }

        enemyWeaponManager.InitializeWeapons();
        StartCoroutine(SpawnWeaponRoutine());
    }

    private IEnumerator SpawnWeaponRoutine()
    {
        while (canSpawn)
        {
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);

            SpawnWeapon();
        }
    }

    private void SpawnWeapon()
    {
        if (enemyWeaponManager == null) return;

        // Spawn a random weapon
        enemyWeaponManager.SpawnRandomWeapon();

        // Calculate a random target position
        targetPosition = GetRandomTargetPosition();

        // Play the throw animation (launch triggered via animation event)
        if (animationController != null)
        {
            animationController.PlayAnimation(AnimationType.Throw);
        }
        else
        {
            Debug.LogError("AnimationController is not assigned to EnemyAI!");
        }
    }

    private Vector3 GetRandomTargetPosition()
    {
        return new Vector3(
            Random.Range(minRange.x, maxRange.x),
            Random.Range(minRange.y, maxRange.y),
            0
        );
    }

    public void OnThrowAnimationEvent()
    {
        if (enemyWeaponManager == null) return;

        Vector3 launchVector = enemyWeaponManager.CalculateLaunchVector(
            enemyWeaponManager.GetWeaponSpawnPosition(),
            targetPosition
        );

        enemyWeaponManager.LaunchWithForce(launchVector);
    }

    private void OnDisable()
    {
        canSpawn = false;
    }
}
