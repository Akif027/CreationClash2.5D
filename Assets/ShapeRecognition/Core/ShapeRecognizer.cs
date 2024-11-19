using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShapeRecognitionPlugin
{
    public class ShapeRecognizer : MonoBehaviour
    {
        [SerializeField] private ShapeMatcher shapeMatcher; // Handles shape matching logic
        [SerializeField, Tooltip("Number of points to describe the shape accurately")]
        public int descriptorPointsCount = 32;

        [SerializeField] private GameData gameData; // Reference to GameData for weapon lookup

        private void Start()
        {
            if (shapeMatcher == null)
            {
                Debug.LogError("ShapeMatcher is not assigned. Please assign it in the inspector.");
                return;
            }

            GameView gameView = (GameView)GameManager.Instance.GetManager<GameView>();
            gameData = gameView.GetGameData();
            if (gameData == null)
            {
                Debug.LogError("GameData is not assigned. Please assign it in the inspector.");
                return;
            }
        }

        /// <summary>
        /// Retrieves the ShapeTemplate that matches the WeaponType.
        /// </summary>
        public ShapeTemplate GetShapeTemplateForWeaponType(WeaponType weaponType)
        {
            return gameData.ShapeData.GetShapeTemplateByWeaponType(weaponType);
        }

        /// <summary>
        /// Matches a shape based on drawn points and retrieves the corresponding weapon data.
        /// </summary>
        public WeaponData GetWeaponDataFromShape(List<Vector2> drawnPoints)
        {
            string matchedShapeName = GetMatchedShape(drawnPoints);
            if (string.IsNullOrEmpty(matchedShapeName) || matchedShapeName == "No Match Found")
            {
                Debug.Log("No weapon data found for the matched shape.");
                return null;
            }

            // Retrieve ShapeTemplate from matched shape name
            var shapeTemplate = gameData.ShapeData.ShapeDatas.FirstOrDefault(s => s.shapeName == matchedShapeName);
            if (shapeTemplate == null)
            {
                Debug.LogWarning($"No ShapeTemplate found for shapeName: {matchedShapeName}");
                return null;
            }

            return gameData.WeaponDatas.FirstOrDefault(w => w.WeaponType == shapeTemplate.weaponType);
        }

        private string GetMatchedShape(List<Vector2> drawnPoints)
        {
            if (drawnPoints == null || drawnPoints.Count == 0)
            {
                Debug.LogWarning("Drawn points are empty or null.");
                return "No Match Found";
            }

            Vector2[] centeredPoints = ShapeUtils.CenterPoints(drawnPoints.ToArray());
            Vector2[] rotatedPoints = ShapeUtils.NormalizeRotation(centeredPoints);
            Vector2[] resampledPoints = ResamplingUtility.ResamplePoints(rotatedPoints, descriptorPointsCount);

            return shapeMatcher.MatchShape(resampledPoints) ?? "No Match Found";
        }
    }
}
