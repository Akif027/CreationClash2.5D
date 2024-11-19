using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game Data/WeaponData")]
public class WeaponData : ScriptableObject, IWeaponStats
{
    [SerializeField] private string weaponPrefabName;
    [SerializeField] private GameObject weaponPrefab; // Weapon prefab reference
    [SerializeField] private int damage;
    [SerializeField] private float range;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float launchForce;
    [SerializeField] private float launchAngle;
    [SerializeField] private WeaponType weaponType;

    public string WeaponPrefabName => weaponPrefabName;
    public GameObject WeaponPrefab => weaponPrefab;

    // Properties from IWeaponStats
    public int Damage => damage;
    public float Range => range;
    public float AttackSpeed => attackSpeed;
    public float LaunchForce => launchForce;
    public float LaunchAngle => launchAngle;
    public WeaponType WeaponType => weaponType; // WeaponType property



}
