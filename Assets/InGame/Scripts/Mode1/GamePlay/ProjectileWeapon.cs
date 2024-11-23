using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    private WeaponData weaponData; // Reference to the WeaponData
    private Rigidbody weaponRigidbody; // Rigidbody for physics
    private Vector3 initialPosition; // Initial position of the weapon (spawn point)
    private Camera mainCamera;

    [Header("Drag & Release Settings")]
    [SerializeField] private float maxDragDistance = 5f; // Maximum drag distance
    private Vector3 dragStartPosition; // Start position of the drag
    private Vector3 dragEndPosition; // End position of the drag
    private bool isDragging = false;

    [Header("State")]
    public bool isLaunched = false; // Whether the weapon is launched

    [Header("Trajectory Settings")]
    [SerializeField] private string trajectoryDotPoolName = "DotMaker"; // Name of the trajectory dot pool
    public int dotCount = 30; // Number of trajectory dots
    private List<GameObject> activeDots = new List<GameObject>(); // Active dots for trajectory visualization
    private ObjectPool objectPool; // Object pool reference

    private void Start()
    {
        mainCamera = Camera.main;
        weaponRigidbody = GetComponent<Rigidbody>();
        initialPosition = transform.position; // Save the initial position
        objectPool = FindFirstObjectByType<ObjectPool>();

        if (weaponRigidbody != null)
        {
            weaponRigidbody.isKinematic = true; // Ensure it's kinematic before launch
        }

        // Trigger event to notify that a weapon is spawned
        EventManager.TriggerEvent("OnWeaponSpawned");
    }

    private void Update()
    {
        if (isLaunched) return; // Skip update if the weapon is launched

        HandleDragAndRelease(); // Handle drag and release mechanics
    }

    private void HandleDragAndRelease()
    {
        if (Input.GetMouseButtonDown(0)) // Start drag
        {
            dragStartPosition = GetMouseWorldPosition();
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging) // During drag
        {
            Vector3 currentMousePosition = GetMouseWorldPosition();
            Vector3 dragVector = dragStartPosition - currentMousePosition;

            // Clamp drag vector to maximum drag distance
            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
            }

            // Show trajectory based on drag vector
            ShowTrajectory(dragVector);
        }

        if (Input.GetMouseButtonUp(0) && isDragging) // Release drag
        {
            dragEndPosition = GetMouseWorldPosition();
            Vector3 releaseVector = dragStartPosition - dragEndPosition;

            // Clamp the release vector to the maximum drag distance
            if (releaseVector.magnitude > maxDragDistance)
            {
                releaseVector = releaseVector.normalized * maxDragDistance;
            }

            Launch(releaseVector); // Launch the weapon
            isDragging = false;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(mainCamera.transform.position.z); // Adjust depth
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    public void Launch(Vector3 releaseVector)
    {
        if (isLaunched) return; // Prevent multiple launches

        weaponRigidbody.isKinematic = false; // Enable physics
        weaponRigidbody.AddForce(releaseVector * weaponData.LaunchForce, ForceMode.Impulse);
        isLaunched = true;

        // Trigger event to notify weapon is released
        EventManager.TriggerEvent("OnWeaponReleased");

        // Hide trajectory and finalize launch
        StartCoroutine(DisableTrajectoryAfterDelay(1f));
    }

    private void ShowTrajectory(Vector3 dragVector)
    {
        if (objectPool == null || weaponData == null || string.IsNullOrEmpty(trajectoryDotPoolName))
        {
            Debug.LogError("Required components are missing for trajectory display.");
            return;
        }

        ClearActiveDots();

        Vector3 startPosition = initialPosition;
        Vector3 velocity = dragVector * weaponData.LaunchForce;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float timeStep = weaponData.Range / dotCount;

        for (int i = 0; i < dotCount; i++)
        {
            float t = i * timeStep / weaponData.LaunchForce;
            Vector3 position = startPosition + new Vector3(
                velocity.x * t,
                velocity.y * t - 0.5f * gravity * t * t,
                0
            );

            GameObject dot = objectPool.GetPooledObject(trajectoryDotPoolName);
            if (dot == null) break; // Stop if no dots are available in the pool

            dot.transform.position = position;
            activeDots.Add(dot);
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

    private IEnumerator DisableTrajectoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideTrajectory();
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
    /// Assigns the provided WeaponData to this weapon.
    /// </summary>
    /// <param name="data">The WeaponData to assign.</param>
    public void AssignWeaponData(WeaponData data)
    {
        if (data == null)
        {
            Debug.LogError("WeaponData is null and cannot be assigned!");
            return;
        }

        weaponData = data; // Reference only, no modifications
    }

    private void OnCollisionEnter(Collision collision)
    {
        HideTrajectory(); // Hide trajectory upon collision
    }
}
