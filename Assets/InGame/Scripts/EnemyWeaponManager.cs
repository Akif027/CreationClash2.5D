using UnityEngine;

public class EnemyWeaponManager : WeaponManager
{
    [SerializeField] private Transform weaponSpawnPoint; // Where the weapon appears initially

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
        // Ensure weapon spawns at the spawn point
        return weaponSpawnPoint != null ? weaponSpawnPoint.position : base.GetWeaponSpawnPosition();
    }

    public void LaunchTowardTarget(Vector3 targetPosition)
    {
        if (currentWeaponInstance == null || currentWeaponRigidbody == null)
        {
            Debug.LogWarning("No weapon to launch!");
            return;
        }

        // Calculate the launch vector
        Vector3 launchVector = CalculateLaunchVector(GetWeaponSpawnPosition(), targetPosition);

        // Launch the weapon
        LaunchWithForce(launchVector);
    }

    public Vector3 CalculateLaunchVector(Vector3 startPosition, Vector3 targetPosition)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);
        float distanceX = targetPosition.x - startPosition.x;
        float distanceY = targetPosition.y - startPosition.y;

        float velocitySquared = gravity * Mathf.Pow(distanceX, 2) / (2 * Mathf.Abs(distanceY));
        if (velocitySquared <= 0)
        {
            Debug.LogWarning("Invalid launch calculation: Target out of range!");
            return Vector3.zero;
        }

        float velocity = Mathf.Sqrt(velocitySquared);
        return new Vector3(distanceX, velocity, 0).normalized * velocity;
    }
}
