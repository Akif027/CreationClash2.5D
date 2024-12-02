using UnityEngine;

public class EnemyWeaponManager : WeaponManager
{
    [Header("Enemy-Specific Settings")]
    [SerializeField] private float minLaunchForce = 10f;
    [SerializeField] private float maxLaunchForce = 20f;
    [SerializeField] private Vector2 targetRangeX = new Vector2(-10f, -20f);
    [SerializeField] private Vector2 targetRangeY = new Vector2(0f, 5f);
    [SerializeField] private Transform weaponSpawnPoint;
    public override void InitializeWeapons()
    {
        GameView gameView = (GameView)GameManager.Instance.GetManager<GameView>();
        if (gameView == null)
        {
            Debug.LogError("GameView is not found in GameManager.");
            return;
        }

        availableWeapons = gameView.GetGameData()?.WeaponDatas;
        if (availableWeapons == null || availableWeapons.Count == 0)
        {
            Debug.LogError("No weapons available for EnemyWeaponManager.");
        }
    }

    public override void SpawnRandomWeapon()
    {
        if (availableWeapons == null || availableWeapons.Count == 0) return;

        WeaponData randomWeapon = availableWeapons[Random.Range(0, availableWeapons.Count)];
        SpawnWeapon(randomWeapon);
    }
    public override Vector3 GetWeaponSpawnPosition()
    {
        if (weaponSpawnPoint != null)
        {
            return weaponSpawnPoint.position;
        }
        return base.GetWeaponSpawnPosition(); // Fallback to default if not set
    }
    public void LaunchTowardTarget(Vector3 targetPosition)
    {
        if (currentWeaponInstance == null || currentWeaponRigidbody == null) return;

        Vector3 startPosition = GetWeaponSpawnPosition();
        Vector3 launchVector = CalculateLaunchVector(startPosition, targetPosition);

        LaunchWithForce(launchVector);
    }

    public Vector3 CalculateLaunchVector(Vector3 startPosition, Vector3 targetPosition)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);
        float distanceX = targetPosition.x - startPosition.x;
        float distanceY = targetPosition.y - startPosition.y;

        // Calculate required velocity
        float velocitySquared = gravity * Mathf.Pow(distanceX, 2) / (2 * Mathf.Abs(distanceY));
        if (velocitySquared <= 0)
        {
            Debug.LogWarning("Invalid launch calculation: Target out of range!");
            return Vector3.zero;
        }

        float velocity = Mathf.Sqrt(velocitySquared);
        return new Vector3(-velocity, velocity, 0).normalized * velocity;
    }
}
