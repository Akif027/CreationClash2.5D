using UnityEngine;
using System.Collections.Generic;

public class PlayerWeaponManager : WeaponManager
{
    [Header("Player-Specific Settings")]
    [SerializeField] private Transform weaponSpawnPoint;
    [SerializeField] private string trajectoryDotPoolName = "DotMaker";
    [SerializeField] private int dotCount = 30;
    [SerializeField] private float maxDragDistance = 5f;
    [SerializeField] private Transform TrajectoryPos;
    [SerializeField] private AnimationController animationController;

    private Vector3 dragStartPosition;
    private Vector3 dragEndPosition;
    private bool isDragging;
    private ObjectPool objectPool;
    private List<GameObject> activeDots = new List<GameObject>();
    private Vector3 calculatedLaunchForce; // Store calculated launch force

    private void Start()
    {
        objectPool = FindFirstObjectByType<ObjectPool>();
        InitializeWeapons();
    }

    public override void InitializeWeapons()
    {
        // Load and prepare weapons available to the player
    }

    public override void SpawnRandomWeapon()
    {
        Debug.LogWarning("Player does not use random weapon spawns.");
    }

    public override Vector3 GetWeaponSpawnPosition()
    {
        return weaponSpawnPoint != null ? weaponSpawnPoint.position : base.GetWeaponSpawnPosition();
    }

    private void Update()
    {
        if (currentWeaponInstance == null) return;

        HandleDragAndRelease();
    }

    private void HandleDragAndRelease()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = GetMouseWorldPosition();
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentMousePosition = GetMouseWorldPosition();
            Vector3 dragVector = dragStartPosition - currentMousePosition;

            // Clamp drag vector to maximum drag distance
            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
            }

            ShowTrajectory(dragVector, TrajectoryPos.position);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            dragEndPosition = GetMouseWorldPosition();
            isDragging = false;

            calculatedLaunchForce = CalculateLaunchForce(dragStartPosition, dragEndPosition);
            animationController.PlayAnimation(AnimationType.Throw); // Play throw animation
            ClearActiveDots(); // Clear trajectory dots after release
        }
    }

    private Vector3 CalculateLaunchForce(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3 releaseVector = startPosition - endPosition;

        // Clamp release vector to maximum drag distance
        if (releaseVector.magnitude > maxDragDistance)
        {
            releaseVector = releaseVector.normalized * maxDragDistance;
        }

        return releaseVector * currentWeaponData.LaunchForce; // Adjust force based on drag and weapon's LaunchForce
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(Camera.main.transform.position.z); // Adjust depth
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }

    private void ShowTrajectory(Vector3 dragVector, Vector3 startTrajectoryPosition)
    {
        if (objectPool == null || string.IsNullOrEmpty(trajectoryDotPoolName))
        {
            Debug.LogError("ObjectPool or trajectory dot pool name is not set.");
            return;
        }

        ClearActiveDots();

        Vector3 startPosition = startTrajectoryPosition; // Use the custom starting position
        Vector3 velocity = dragVector * currentWeaponData.LaunchForce; // Calculate initial velocity

        float gravity = Mathf.Abs(Physics.gravity.y); // Match Unity's gravity
        float timeStep = 0.05f; // Time interval for trajectory calculations

        for (int i = 0; i < dotCount; i++) // Loop to calculate trajectory points
        {
            float t = i * timeStep; // Time step for each dot
            Vector3 position = startPosition + velocity * t + 0.5f * Physics.gravity * Mathf.Pow(t, 2); // Projectile equation

            GameObject dot = objectPool.GetPooledObject(trajectoryDotPoolName); // Fetch dot from pool
            if (dot == null) break;

            dot.transform.position = position; // Set dot's position
            activeDots.Add(dot); // Track active dots
        }
    }

    private void ClearActiveDots()
    {
        foreach (var dot in activeDots)
        {
            objectPool.ReturnObjectToPool(dot, trajectoryDotPoolName);
        }
        activeDots.Clear();
    }

    /// <summary>
    /// Launches the weapon. Call this method via the animation event.
    /// </summary>
    public void LaunchTheWeapon()
    {
        if (currentWeaponInstance != null && currentWeaponRigidbody != null)
        {
            LaunchWithForce(calculatedLaunchForce); // Use the pre-calculated force
        }
        else
        {
            Debug.LogError("Weapon instance or Rigidbody is null. Cannot launch the weapon.");
        }
    }
}
