using UnityEngine;
using System.Collections.Generic;

public abstract class WeaponManager : MonoBehaviour
{
    protected List<WeaponData> availableWeapons = new List<WeaponData>();
    protected GameObject currentWeaponInstance;
    protected Rigidbody currentWeaponRigidbody;
    protected WeaponData currentWeaponData;

    public Transform handpos;

    public virtual void Awake()
    {
        // Subscribe to the event to handle weapon matching
        EventManager.Subscribe("OnWeaponMatched", HandleWeaponMatched);
    }

    public virtual void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        EventManager.Unsubscribe("OnWeaponMatched", HandleWeaponMatched);
    }

    public abstract void InitializeWeapons();

    public abstract void SpawnRandomWeapon();
    public virtual void SpawnWeapon(WeaponData weaponData)
    {
        if (weaponData == null || weaponData.WeaponPrefab == null)
        {
            Debug.LogWarning("Invalid WeaponData or missing WeaponPrefab!");
            return;
        }

        if (currentWeaponInstance != null)
        {
            Debug.LogWarning("Weapon already equipped. Cannot spawn another.");
            return;
        }

        // Instantiate the weapon
        currentWeaponInstance = Instantiate(weaponData.WeaponPrefab, handpos.position, Quaternion.identity);
        currentWeaponRigidbody = currentWeaponInstance.GetComponent<Rigidbody>();
        currentWeaponInstance.transform.SetParent(handpos);

        // Assign the anchor for proper attachment
        ProjectileWeapon projectileWeapon = currentWeaponInstance.GetComponent<ProjectileWeapon>();
        if (projectileWeapon != null)
        {
            projectileWeapon.anchor = handpos;
        }

        if (currentWeaponRigidbody != null)
        {
            currentWeaponRigidbody.isKinematic = true; // Disable physics until launch
        }

        // Update current weapon data
        currentWeaponData = weaponData;

        Debug.Log($"Spawned Weapon: {weaponData.WeaponPrefabName}");
    }
    public virtual Vector3 GetWeaponSpawnPosition()
    {
        return transform.position;
    }

    public virtual void LaunchWithForce(Vector3 force)
    {
        if (currentWeaponInstance != null && currentWeaponRigidbody != null)
        {
            // Detach weapon from the hand
            ProjectileWeapon projectileWeapon = currentWeaponInstance.GetComponent<ProjectileWeapon>();
            if (projectileWeapon != null)
            {
                projectileWeapon.Launch(force); // Use the Launch method to apply force
            }

            // Handle different weapon types
            switch (currentWeaponData.WeaponType)
            {
                case WeaponType.Spear:
                    LaunchSpear(force);
                    break;
                case WeaponType.Stick:
                    LaunchStick(force);
                    break;
                case WeaponType.Rock:
                    LaunchRock(force);
                    break;
                case WeaponType.Shuriken:
                    LaunchShuriken(force);
                    break;
                default:
                    LaunchGenericWeapon(force);
                    break;
            }
        }
    }
    private void LaunchSpear(Vector3 force)
    {
        // Ensure the spear starts at the spawn position
        currentWeaponInstance.transform.position = GetWeaponSpawnPosition();
        currentWeaponInstance.transform.SetParent(null);

        // Align the spear with the launch direction
        Quaternion spearRotation = Quaternion.LookRotation(force.normalized, Vector3.up);
        currentWeaponInstance.transform.rotation = spearRotation;

        currentWeaponRigidbody.isKinematic = false;
        currentWeaponRigidbody.AddForce(force, ForceMode.VelocityChange);



        ResetWeapon();
    }

    private void LaunchStick(Vector3 force)
    {
        // Ensure the stick starts at the spawn position
        currentWeaponInstance.transform.position = GetWeaponSpawnPosition();
        currentWeaponInstance.transform.SetParent(null);

        // Align the stick with the launch direction
        Quaternion stickRotation = Quaternion.LookRotation(force.normalized, Vector3.up);
        currentWeaponInstance.transform.rotation = stickRotation;

        currentWeaponRigidbody.isKinematic = false;
        currentWeaponRigidbody.AddForce(force, ForceMode.VelocityChange);

        // Optional: Add a slower rotation for the stick
        currentWeaponRigidbody.AddTorque(new Vector3(0, 100f, 0), ForceMode.VelocityChange);



        ResetWeapon();
    }
    private void LaunchRock(Vector3 force)
    {
        // Ensure the weapon starts at the spawn position
        currentWeaponInstance.transform.position = GetWeaponSpawnPosition();
        currentWeaponInstance.transform.SetParent(null);

        currentWeaponRigidbody.isKinematic = false;
        currentWeaponRigidbody.AddForce(force, ForceMode.VelocityChange);

        // Add random spin for the rock
        Vector3 randomTorque = new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)
        );
        currentWeaponRigidbody.AddTorque(randomTorque, ForceMode.VelocityChange);

        ResetWeapon();
    }

    private void LaunchShuriken(Vector3 force)
    {
        // Ensure the weapon starts at the spawn position
        currentWeaponInstance.transform.position = GetWeaponSpawnPosition();
        currentWeaponInstance.transform.SetParent(null);

        // Align the shuriken with the launch direction
        Quaternion shurikenRotation = Quaternion.LookRotation(force.normalized, Vector3.up);
        currentWeaponInstance.transform.rotation = shurikenRotation;

        currentWeaponRigidbody.isKinematic = false;
        currentWeaponRigidbody.AddForce(force, ForceMode.VelocityChange);

        // Add spinning for shuriken
        currentWeaponRigidbody.AddTorque(new Vector3(0, 0, 1000f), ForceMode.VelocityChange);

        ResetWeapon();
    }

    private void LaunchGenericWeapon(Vector3 force)
    {
        // Default launch logic for generic weapons
        currentWeaponInstance.transform.position = GetWeaponSpawnPosition();
        currentWeaponInstance.transform.SetParent(null);

        currentWeaponRigidbody.isKinematic = false;
        currentWeaponRigidbody.AddForce(force, ForceMode.VelocityChange);

        ResetWeapon();
    }

    private void ResetWeapon()
    {
        // Enable collision and damage
        ProjectileWeapon projectileWeapon = currentWeaponInstance.GetComponent<ProjectileWeapon>();
        if (projectileWeapon != null)
        {
            projectileWeapon.Damage = currentWeaponData.Damage;
            projectileWeapon.EnableCollider();
        }

        // Reset weapon references
        currentWeaponInstance = null;
        currentWeaponRigidbody = null;
        currentWeaponData = null;
    }

    // Handle the weapon matched event
    protected virtual void HandleWeaponMatched(object weaponObj)
    {
        if (weaponObj is WeaponData weaponData)
        {
            Debug.Log($"Weapon matched: {weaponData.WeaponPrefabName}");
            SpawnWeapon(weaponData); // Spawn the weapon at the spawn position
        }
    }
}
