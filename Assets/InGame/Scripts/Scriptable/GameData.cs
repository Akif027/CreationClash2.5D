using System.Collections.Generic;
using System.Linq;
using ShapeRecognitionPlugin;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data/GameData")]
public class GameData : ScriptableObject
{
    public GameObject TextPrefab;
    public GameObject FloatingTextPrefab;
    [SerializeField] private ShapeData shapeData; // Contains all shape templates
    [SerializeField] private List<WeaponData> weaponDatas; // Contains all weapon data

    private Dictionary<WeaponType, ShapeTemplate> weaponTypeToShapeMapping;

    public ShapeData ShapeData => shapeData; // Public access to shape data
    public List<WeaponData> WeaponDatas => weaponDatas; // Public access to weapon data

    private void OnEnable()
    {
        InitializeWeaponTypeToShapeMapping();
    }

    /// <summary>
    /// Initializes the mapping between WeaponType and ShapeTemplate.
    /// </summary>
    private void InitializeWeaponTypeToShapeMapping()
    {
        weaponTypeToShapeMapping = new Dictionary<WeaponType, ShapeTemplate>();

        Debug.Log("Initializing weapon type-to-shape mapping...");

        foreach (var shapeTemplate in shapeData.ShapeDatas)
        {
            Debug.Log($"Processing ShapeTemplate: {shapeTemplate.shapeName}");

            // Ensure a weapon with this type exists
            var matchingWeapon = weaponDatas.FirstOrDefault(w => w.WeaponType == shapeTemplate.weaponType);

            if (matchingWeapon != null)
            {
                Debug.Log($"Matched WeaponType: {matchingWeapon.WeaponType} with ShapeTemplate: {shapeTemplate.shapeName}");
                weaponTypeToShapeMapping[matchingWeapon.WeaponType] = shapeTemplate;
            }
            else
            {
                Debug.LogWarning($"No matching WeaponData found for WeaponType: {shapeTemplate.weaponType}");
            }
        }

        Debug.Log($"Weapon type-to-shape mapping initialized with {weaponTypeToShapeMapping.Count} entries.");
    }

    /// <summary>
    /// Retrieves the ShapeTemplate associated with the given WeaponType.
    /// </summary>
    /// <param name="weaponType">The WeaponType to look for.</param>
    /// <returns>The corresponding ShapeTemplate, or null if not found.</returns>
    public ShapeTemplate GetShapeTemplateByWeaponType(WeaponType weaponType)
    {
        weaponTypeToShapeMapping.TryGetValue(weaponType, out var shapeTemplate);
        return shapeTemplate;
    }

    /// <summary>
    /// Retrieves the WeaponData associated with the given ShapeTemplate.
    /// </summary>
    /// <param name="shapeTemplate">The ShapeTemplate to look for.</param>
    /// <returns>The corresponding WeaponData, or null if not found.</returns>
    public WeaponData GetWeaponDataForShapeTemplate(ShapeTemplate shapeTemplate)
    {
        return weaponDatas.FirstOrDefault(w => w.WeaponType == shapeTemplate.weaponType);
    }

    /// <summary>
    /// Retrieves the WeaponData associated with the given WeaponType.
    /// </summary>
    /// <param name="weaponType">The WeaponType to look for.</param>
    /// <returns>The corresponding WeaponData, or null if not found.</returns>
    public WeaponData GetWeaponDataForWeaponType(WeaponType weaponType)
    {
        return weaponDatas.FirstOrDefault(w => w.WeaponType == weaponType);
    }
}
