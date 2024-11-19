using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    private WeaponData weaponData;
    private Rigidbody2D weaponRigidbody; // Rigidbody of the weapon in the scene
    private Transform firePoint; // Point from where the weapon is launched

    [Header("Trajectory Settings")]
    [SerializeField] private string trajectoryDotPoolName = "DotMaker"; // The name of the dot pool in the ObjectPool
    public int dotCount = 30; // Number of dots in the trajectory
    private List<GameObject> activeDots = new List<GameObject>(); // List to track active trajectory dots
    private ObjectPool objectPool; // Reference to the ObjectPool script

    [Header("State")]
    public bool isLaunched = false;
    public bool isEnemyWeapon = false;

    [Header("Collider")]
    [SerializeField] private Collider2D col;

    public float dynamicLaunchAngle; // Temporary runtime value for the launch angle

    private void Start()
    {
        col.enabled = false;

        // Assign the fire point specific to this weapon
        firePoint = transform.Find("firePoint");
        if (firePoint == null)
        {
            Debug.LogError("FirePoint not found as a child of the weapon!");
            return;
        }

        // Find the ObjectPool in the scene
        objectPool = FindFirstObjectByType<ObjectPool>();
        weaponRigidbody = GetComponent<Rigidbody2D>();
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool not found in the scene!");
        }

        // Set weapon to kinematic at start for better control
        if (weaponRigidbody != null && !isEnemyWeapon)
        {
            weaponRigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        // Initialize the temporary launch angle from WeaponData
        dynamicLaunchAngle = weaponData != null ? weaponData.LaunchAngle : 45f;

        // Rotate the weapon appropriately
        transform.rotation = isEnemyWeapon ? Quaternion.Euler(0, 180, 16) : Quaternion.Euler(0, 0, 16);

        // Show trajectory at the start (for player only)
        if (!isEnemyWeapon)
            ShowTrajectory();
    }

    public void AssignWeaponData(WeaponData data)
    {
        weaponData = data;

        // Initialize the dynamic launch angle
        dynamicLaunchAngle = weaponData.LaunchAngle;
    }

    private void Update()
    {
        // For player: Update launch angle and trajectory before launching
        if (!isLaunched && !isEnemyWeapon && Input.GetMouseButton(0)) // Left mouse button
        {
            UpdateLaunchAngle();
            ShowTrajectory();
        }

        // Align weapon rotation with velocity after launch
        if (isLaunched && weaponData.WeaponType == WeaponType.Spear)
        {
            AlignWeaponWithVelocity();
        }
    }

    private void UpdateLaunchAngle()
    {
        if (firePoint == null) return;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mouseWorldPosition - firePoint.position;

        // Calculate angle in degrees
        dynamicLaunchAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Clamp the angle to a valid range
        dynamicLaunchAngle = Mathf.Clamp(dynamicLaunchAngle, 0f, 90f);

        if (!isLaunched)
        {
            Launch();
        }
    }

    public void Launch(Vector2? customDirection = null)
    {
        if (weaponRigidbody == null)
            weaponRigidbody = GetComponent<Rigidbody2D>();

        if (weaponData == null)
        {
            Debug.LogError("WeaponData is not assigned.");
            return;
        }

        Vector2 launchDirection = customDirection ?? CalculateLaunchDirection();

        switch (weaponData.WeaponType)
        {
            case WeaponType.Spear:
                LaunchSpear(launchDirection);
                break;
            case WeaponType.Rock:
                LaunchRock(launchDirection);
                break;
            case WeaponType.Shuriken:
                LaunchShuriken(launchDirection);
                break;
            default:
                Debug.LogError("Weapon type not supported.");
                break;
        }

        isLaunched = true;

        // Start coroutines for collider enabling and trajectory hiding
        StartCoroutine(EnableColliderAfterDelay(0.1f));
        StartCoroutine(DisableTrajectoryAfterDelay(1f));
    }

    private Vector2 CalculateLaunchDirection()
    {
        // Convert the dynamic launch angle into a 2D vector
        float angleRad = dynamicLaunchAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * (isEnemyWeapon ? -1 : 1); // Reverse X direction for enemy
    }

    private void LaunchSpear(Vector2 launchDirection)
    {
        weaponRigidbody.bodyType = RigidbodyType2D.Dynamic;
        weaponRigidbody.linearVelocity = Vector2.zero; // Reset existing velocity
        weaponRigidbody.AddForce(launchDirection * weaponData.LaunchForce, ForceMode2D.Impulse);
    }

    private void LaunchRock(Vector2 launchDirection)
    {
        weaponRigidbody.bodyType = RigidbodyType2D.Dynamic;
        weaponRigidbody.linearVelocity = Vector2.zero;
        weaponRigidbody.AddForce(launchDirection * weaponData.LaunchForce, ForceMode2D.Impulse);
    }

    private void LaunchShuriken(Vector2 launchDirection)
    {
        weaponRigidbody.bodyType = RigidbodyType2D.Dynamic;
        weaponRigidbody.linearVelocity = Vector2.zero;
        weaponRigidbody.AddForce(launchDirection * weaponData.LaunchForce, ForceMode2D.Impulse);
    }

    private void AlignWeaponWithVelocity()
    {
        if (weaponRigidbody.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(weaponRigidbody.linearVelocity.y, weaponRigidbody.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        col.enabled = true;
    }

    private IEnumerator DisableTrajectoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideTrajectory();
    }

    private void ShowTrajectory()
    {
        if (firePoint == null || objectPool == null || weaponData == null) return;

        Vector3 startPosition = firePoint.position;
        Vector2 velocity = new Vector2(
            weaponData.LaunchForce * Mathf.Cos(dynamicLaunchAngle * Mathf.Deg2Rad),
            weaponData.LaunchForce * Mathf.Sin(dynamicLaunchAngle * Mathf.Deg2Rad)
        );

        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float timeStep = weaponData.Range / dotCount;

        ClearActiveDots();

        for (int i = 0; i < dotCount; i++)
        {
            float t = i * timeStep / weaponData.LaunchForce;
            Vector3 position = startPosition + new Vector3(
                (isEnemyWeapon ? -1 : 1) * velocity.x * t,
                velocity.y * t - 0.5f * gravity * t * t
            );

            GameObject dot = objectPool.GetPooledObject(trajectoryDotPoolName);
            if (dot != null)
            {
                dot.transform.position = position;
                activeDots.Add(dot);
            }
        }
    }

    private void HideTrajectory()
    {
        foreach (var dot in activeDots)
        {
            objectPool.ReturnObjectToPool(dot, trajectoryDotPoolName);
        }

        activeDots.Clear();
    }

    private void ClearActiveDots()
    {
        foreach (var dot in activeDots)
        {
            objectPool.ReturnObjectToPool(dot, trajectoryDotPoolName);
        }

        activeDots.Clear();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BodyPart bodyPart = collision.collider.GetComponent<BodyPart>();
        if (bodyPart != null)
        {
            bodyPart.TakeDamage(weaponData.Damage);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Hit something other than a body part.");
        }

        HideTrajectory();
    }
}
