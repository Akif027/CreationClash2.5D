using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    private WeaponData weaponData;
    private Rigidbody weaponRigidbody; // Rigidbody for 3D physics
    private Transform firePoint; // Launch point for the weapon

    [Header("Trajectory Settings")]
    [SerializeField] private string trajectoryDotPoolName = "DotMaker"; // Name of the trajectory dot pool
    public int dotCount = 30; // Number of trajectory dots
    private List<GameObject> activeDots = new List<GameObject>();
    private ObjectPool objectPool; // Reference to the ObjectPool

    [Header("State")]
    public bool isLaunched = false;
    public bool isEnemyWeapon = false;

    [Header("Collider")]
    [SerializeField] private Collider col;

    public float dynamicLaunchAngle; // Angle at which the weapon will launch

    public AnimationController animationController;

    private void Start()
    {

        animationController.PlayAnimation(AnimationType.Catch);


        col.enabled = false;

        // Assign the fire point
        firePoint = transform.Find("firePoint");
        if (firePoint == null)
        {
            Debug.LogError("FirePoint not found as a child of the weapon!");
            return;
        }

        // Find ObjectPool
        objectPool = FindFirstObjectByType<ObjectPool>();
        weaponRigidbody = GetComponent<Rigidbody>();
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool not found in the scene!");
        }

        // Set Rigidbody to kinematic at the start for controlled launching
        if (weaponRigidbody != null && !isEnemyWeapon)
        {
            weaponRigidbody.isKinematic = true;
        }

        // Initialize the launch angle
        dynamicLaunchAngle = weaponData != null ? weaponData.LaunchAngle : 45f;

        // Rotate weapon based on ownership (player/enemy)
        transform.rotation = isEnemyWeapon ? Quaternion.Euler(0, 180, 16) : Quaternion.Euler(0, 0, 16);

        // Show trajectory for player weapons
        if (!isEnemyWeapon)
            ShowTrajectory();
    }

    public void AssignWeaponData(WeaponData data)
    {
        weaponData = data;
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

        // Calculate launch angle
        dynamicLaunchAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Clamp the angle
        dynamicLaunchAngle = Mathf.Clamp(dynamicLaunchAngle, 0f, 90f);

        if (!isLaunched)
        {
            animationController.PlayAnimation(AnimationType.Throw);
            Launch();
        }
    }

    public void Launch(Vector3? customDirection = null)
    {
        if (!weaponRigidbody)
            weaponRigidbody = GetComponent<Rigidbody>();

        if (!weaponData)
        {
            Debug.LogError("WeaponData is not assigned.");
            return;
        }

        Vector3 launchDirection = customDirection ?? CalculateLaunchDirection();

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
                Debug.LogError("Unsupported Weapon Type.");
                break;
        }

        isLaunched = true;

        // Enable collider and disable trajectory
        StartCoroutine(EnableColliderAfterDelay(0.1f));
        StartCoroutine(DisableTrajectoryAfterDelay(1f));
    }

    private Vector3 CalculateLaunchDirection()
    {
        // Convert the launch angle into a 3D vector
        float angleRad = dynamicLaunchAngle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
        return isEnemyWeapon ? -direction : direction;
    }

    private void LaunchSpear(Vector3 launchDirection)
    {
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.linearVelocity = Vector3.zero; // Reset velocity
        weaponRigidbody.AddForce(launchDirection * weaponData.LaunchForce, ForceMode.Impulse);
    }

    private void LaunchRock(Vector3 launchDirection)
    {
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.linearVelocity = Vector3.zero;
        weaponRigidbody.AddForce(launchDirection * weaponData.LaunchForce, ForceMode.Impulse);
    }

    private void LaunchShuriken(Vector3 launchDirection)
    {
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.linearVelocity = Vector3.zero;
        weaponRigidbody.AddForce(launchDirection * weaponData.LaunchForce, ForceMode.Impulse);
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
        Vector3 velocity = new Vector3(
            weaponData.LaunchForce * Mathf.Cos(dynamicLaunchAngle * Mathf.Deg2Rad),
            weaponData.LaunchForce * Mathf.Sin(dynamicLaunchAngle * Mathf.Deg2Rad),
            0
        );

        float gravity = Mathf.Abs(Physics.gravity.y);
        float timeStep = weaponData.Range / dotCount;

        ClearActiveDots();

        for (int i = 0; i < dotCount; i++)
        {
            float t = i * timeStep / weaponData.LaunchForce;
            Vector3 position = startPosition + new Vector3(
                velocity.x * t,
                velocity.y * t - 0.5f * gravity * t * t,
                0
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

    private void OnCollisionEnter(Collision collision)
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
