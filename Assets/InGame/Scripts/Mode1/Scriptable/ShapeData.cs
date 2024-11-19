using System.Collections.Generic;
using System.Linq;
using ShapeRecognitionPlugin;
using UnityEngine;

[System.Serializable]
public class ShapeData
{
    public List<ShapeTemplate> ShapeDatas; // List of all shape templates

    /// <summary>
    /// Retrieves all shape templates.
    /// </summary>
    public List<ShapeTemplate> GetAllShapeTemplates()
    {
        return ShapeDatas;
    }

    /// <summary>
    /// Retrieves a ShapeTemplate that matches the given WeaponType.
    /// </summary>
    public ShapeTemplate GetShapeTemplateByWeaponType(WeaponType weaponType)
    {
        return ShapeDatas.FirstOrDefault(shape => shape.weaponType == weaponType);
    }
}
