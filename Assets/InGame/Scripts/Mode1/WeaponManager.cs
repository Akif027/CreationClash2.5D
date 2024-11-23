using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Transform cardParent; // Parent transform for weapon cards
    [SerializeField] private Transform weaponSpawnPoint; // Spawn point for weapons

    private Dictionary<WeaponData, WeaponRuntimeData> weaponRuntimeData = new Dictionary<WeaponData, WeaponRuntimeData>();
    private Dictionary<WeaponData, GameObject> weaponCards = new Dictionary<WeaponData, GameObject>();

    private void Start()
    {
        InitializeWeapons();
        EventManager.Subscribe("OnWeaponMatched", HandleWeaponMatched);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe("OnWeaponMatched", HandleWeaponMatched);
        foreach (var card in weaponCards.Values)
        {
            Destroy(card);
        }
    }

    private void InitializeWeapons()
    {
        GameView gameView = (GameView)GameManager.Instance.GetManager<GameView>();
        if (gameView == null)
        {
            Debug.LogError("GameView is not found in GameManager.");
            return;
        }

        List<WeaponData> allWeapons = gameView.GetGameData()?.WeaponDatas;
        if (allWeapons == null)
        {
            Debug.LogError("Weapon data not found in GameData.");
            return;
        }

        foreach (var weapon in allWeapons)
        {
            var runtimeData = new WeaponRuntimeData
            {
                WeaponData = weapon,
                RemainingCount = 4
            };

            weaponRuntimeData[weapon] = runtimeData;

            if (weapon.weaponCard != null && cardParent != null)
            {
                GameObject card = Instantiate(weapon.weaponCard, cardParent);
                weaponCards[weapon] = card;
                UpdateWeaponCardUI(card, runtimeData);
            }
        }
    }


    private void HandleWeaponMatched(object weaponObj)
    {
        if (weaponObj is WeaponData matchedWeapon && weaponRuntimeData.ContainsKey(matchedWeapon))
        {
            var runtimeData = weaponRuntimeData[matchedWeapon];

            if (runtimeData.RemainingCount > 0)
            {
                runtimeData.RemainingCount--; // Modify runtime state
                Debug.Log($"Weapon used: {matchedWeapon.WeaponPrefabName}. Remaining: {runtimeData.RemainingCount}");

                // Check weapon prefab and spawn point
                if (matchedWeapon.WeaponPrefab == null)
                {
                    Debug.LogError($"Weapon prefab is null for {matchedWeapon.WeaponPrefabName}");
                    return;
                }

                if (weaponSpawnPoint == null)
                {
                    Debug.LogError("Weapon spawn point is not assigned in WeaponManager.");
                    return;
                }

                // Instantiate the weapon at the spawn point
                GameObject weaponInstance = Instantiate(matchedWeapon.WeaponPrefab, weaponSpawnPoint.position, Quaternion.identity);
                Debug.Log($"Instantiated Weapon: {matchedWeapon.WeaponPrefabName} at {weaponSpawnPoint.position}");

                // Call AssignWeaponData on the instantiated weapon
                ProjectileWeapon projectileWeapon = weaponInstance.GetComponent<ProjectileWeapon>();
                if (projectileWeapon != null)
                {
                    projectileWeapon.AssignWeaponData(matchedWeapon);
                }
                else
                {
                    Debug.LogError($"ProjectileWeapon component is missing on {matchedWeapon.WeaponPrefabName}");
                }

                // Update the card UI if applicable
                if (weaponCards.ContainsKey(matchedWeapon))
                {
                    UpdateWeaponCardUI(weaponCards[matchedWeapon], runtimeData);
                }
            }
            else
            {
                Debug.Log($"Out of {matchedWeapon.WeaponPrefabName}");
            }
        }
        else
        {
            Debug.LogWarning("Weapon match event received, but weapon data is invalid or not initialized.");
        }
    }

    private void UpdateWeaponCardUI(GameObject card, WeaponRuntimeData runtimeData)
    {
        if (card != null)
        {
            card.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = runtimeData.WeaponData.WeaponPrefabName;
            card.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = runtimeData.RemainingCount.ToString();
        }
    }


}

/// <summary>
/// Runtime data for a weapon, used to track state that shouldn't modify the original asset.
/// </summary>
public class WeaponRuntimeData
{
    public WeaponData WeaponData { get; set; }
    public int RemainingCount { get; set; }
}
