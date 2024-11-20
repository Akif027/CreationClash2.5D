using UnityEngine;

namespace ShapeRecognitionPlugin
{
    [CreateAssetMenu(fileName = "New Shape", menuName = "Shape Recognition Plugin/Shape Template", order = 1)]
    public class ShapeTemplate : ScriptableObject
    {
        public string shapeName;
        public ShapeType shapeType; // Add this field to group shapes
        public WeaponType weaponType;
        public Vector2[] points;


    }


    public enum ShapeType
    {

        Orignal,
        Variant

    }
}
