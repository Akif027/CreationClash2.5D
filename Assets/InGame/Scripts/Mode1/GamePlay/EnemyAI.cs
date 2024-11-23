using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Weapon Spawning")]
    [SerializeField] private Transform weaponSpawnPoint; // Position where the weapon will spawn
    private List<WeaponData> weaponDatas; // Weapon data list initialized from GameData
    private GameData gameData; // Reference to GameData

    [SerializeField] private float minSpawnInterval = 2f; // Minimum time interval between spawns
    [SerializeField] private float maxSpawnInterval = 5f; // Maximum time interval between spawns
    [SerializeField] private Vector2 randomLaunchAngleRange = new Vector2(20f, 45f); // Random launch angle range
    [SerializeField] private float launchDelay = 1f; // Delay before launching the weapon

    public AnimationController EnemyAnimationController;
    private bool canSpawn = true; // Controls whether the AI can spawn weapons

    private void Start()
    {
        GameView gameView = (GameView)GameManager.Instance.GetManager<GameView>();
        // Initialize GameData and weapon data list
        gameData = gameView.GetGameData();

        if (gameData == null)
        {
            Debug.LogError("GameData is not set in GameManager!");
            return;
        }
        weaponDatas = gameData.WeaponDatas;

        if (weaponSpawnPoint == null)
        {
            Debug.LogError("WeaponSpawnPoint is not assigned!");
            return;
        }

        if (weaponDatas == null || weaponDatas.Count == 0)
        {
            Debug.LogError("No weapon data found in GameData!");
            return;
        }

        // Start spawning weapons
        StartCoroutine(SpawnWeaponRoutine());
    }

    /// <summary>
    /// Coroutine for spawning weapons at random intervals.
    /// </summary>
    private IEnumerator SpawnWeaponRoutine()
    {
        while (canSpawn)
        {
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);
            SpawnWeapon();
        }
    }

    /// <summary>
    /// Spawns a random weapon and applies random logic to launch it towards the player.
    /// </summary>
    private void SpawnWeapon()
    {
        if (weaponDatas == null || weaponDatas.Count == 0 || weaponSpawnPoint == null)
        {
            Debug.LogWarning("Cannot spawn weapons: Missing required references or data.");
            return;
        }

        // Select a random weapon data
        WeaponData randomWeaponData = weaponDatas[Random.Range(0, weaponDatas.Count)];
        if (randomWeaponData.WeaponPrefab == null)
        {
            Debug.LogWarning($"Weapon prefab for {randomWeaponData.WeaponPrefabName} is missing!");
            return;
        }

        // Instantiate the weapon
        GameObject weaponInstance = Instantiate(randomWeaponData.WeaponPrefab, weaponSpawnPoint.position, Quaternion.identity);
        ProjectileWeapon weaponLogic = weaponInstance.GetComponent<ProjectileWeapon>();
        //weaponLogic.animationController = EnemyAnimationController;
        if (weaponLogic == null)
        {
            Debug.LogWarning("ProjectileWeapon script is missing on the spawned weapon prefab!");
            Destroy(weaponInstance);
            return;
        }

        // Set the Rigidbody to kinematic initially
        Rigidbody weaponRigidbody = weaponInstance.GetComponent<Rigidbody>();
        if (weaponRigidbody != null)
        {
            weaponRigidbody.isKinematic = true;
        }

        // Assign weapon data and set it as an enemy weapon
        weaponLogic.AssignWeaponData(randomWeaponData);
        //    weaponLogic.isEnemyWeapon = true;

        // Randomize launch angle and launch direction
        float randomAngle = Random.Range(randomLaunchAngleRange.x, randomLaunchAngleRange.y);
        Vector2 launchDirection = CalculateLaunchDirection(randomAngle);

        // Launch the weapon with a delay
        StartCoroutine(DelayedLaunch(weaponLogic, weaponRigidbody, launchDirection, randomAngle));
    }

    /// <summary>
    /// Coroutine to delay the launch of the weapon.
    /// </summary>
    private IEnumerator DelayedLaunch(ProjectileWeapon weaponLogic, Rigidbody weaponRigidbody, Vector2 launchDirection, float angle)
    {
        yield return new WaitForSeconds(launchDelay);

        // Set the Rigidbody to dynamic just before launch
        if (weaponRigidbody != null)
        {
            weaponRigidbody.isKinematic = false;
        }
        EnemyAnimationController.PlayAnimation(AnimationType.Throw);
        // Launch the weapon
        weaponLogic.Launch(launchDirection);
        Debug.Log($"EnemyAI launched {weaponLogic.name} at angle {angle}° after a delay of {launchDelay}s.");
    }

    /// <summary>
    /// Calculates the launch direction based on a random angle.
    /// </summary>
    /// <param name="angle">The angle in degrees.</param>
    /// <returns>The normalized launch direction vector.</returns>
    private Vector2 CalculateLaunchDirection(float angle)
    {
        // Convert angle to radians
        float angleRad = angle * Mathf.Deg2Rad;

        // Return the normalized direction vector
        return new Vector2(-Mathf.Cos(angleRad), Mathf.Sin(angleRad)); // Negative X direction for enemy shooting
    }

    private void OnDisable()
    {
        // Stop spawning when the AI is disabled
        canSpawn = false;
    }
}
