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

        currentWeaponInstance = Instantiate(weaponData.WeaponPrefab, GetWeaponSpawnPosition(), Quaternion.identity);
        currentWeaponRigidbody = currentWeaponInstance.GetComponent<Rigidbody>();
        currentWeaponInstance.transform.SetParent(handpos);
        currentWeaponInstance.transform.position = handpos.position;

        if (currentWeaponRigidbody != null)
        {
            currentWeaponRigidbody.isKinematic = true; // Disable physics until launched
        }

        currentWeaponData = weaponData;
    }

    public virtual Vector3 GetWeaponSpawnPosition()
    {
        return transform.position;
    }

    public void LaunchWithForce(Vector3 force)
    {
        if (currentWeaponInstance != null && currentWeaponRigidbody != null)
        {
            ProjectileWeapon projectileWeapon = currentWeaponInstance.GetComponent<ProjectileWeapon>();
            projectileWeapon.Damage = currentWeaponData.Damage;
            currentWeaponInstance.transform.SetParent(null); // Detach from any parent (like a hand)
            currentWeaponRigidbody.isKinematic = false; // Enable physics
            currentWeaponRigidbody.AddForce(force, ForceMode.VelocityChange); // Apply the given force
            currentWeaponInstance = null; // Reset after launch
            projectileWeapon.EnableCollider();

        }
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
