using System.Collections.Generic;
using System.Linq;
using ShapeRecognitionPlugin;
using UnityEngine;

public class ShapeMatcher : MonoBehaviour
{
    [Tooltip("Templates for different shapes to match against")]
    public List<ShapeTemplate> shapeTemplates;

    [Tooltip("Threshold for Fourier descriptor comparison")]
    public float fourierThreshold = 0.5f;

    [Tooltip("Threshold for Dynamic Time Warping (DTW) comparison")]
    public float dtwThreshold = 10f;

    [Tooltip("Minimum confidence threshold to accept a match")]
    public float minConfidenceThreshold = 0.3f;

    private readonly Dictionary<string, System.Numerics.Complex[]> templateDescriptors = new();
    private readonly Dictionary<string, Vector2[]> templatePoints = new();

    private void Start()
    {
        LoadTemplatesFromGameData();
        InitializeTemplates();
    }

    private void LoadTemplatesFromGameData()
    {
        // Get GameView from GameManager and load shapeTemplates from GameData
        GameView gameView = (GameView)GameManager.Instance.GetManager<GameView>();
        if (gameView == null)
        {
            Debug.LogError("GameView manager not found in GameManager.");
            return;
        }

        GameData gameData = gameView.GetGameData();

        if (gameData == null)
        {
            Debug.LogError("GameData not found in GameView.");
            return;
        }

        shapeTemplates = gameData.ShapeData.GetAllShapeTemplates();

        // Log to confirm if shapeTemplates were loaded
        if (shapeTemplates == null || shapeTemplates.Count == 0)
        {
            Debug.LogWarning("No shape templates loaded from GameData.");
        }
        else
        {
            Debug.Log($"Loaded {shapeTemplates.Count} shape templates from GameData.");
        }
    }

    public void InitializeTemplates()
    {
        if (shapeTemplates == null || shapeTemplates.Count == 0)
        {
            Debug.LogWarning("No shape templates provided for matching.");
            return;
        }

        foreach (var template in shapeTemplates)
        {
            if (template == null || template.points == null || template.points.Length == 0)
            {
                Debug.LogWarning($"Template '{template?.shapeName ?? "Unnamed"}' has invalid or empty points.");
                continue;
            }

            // Center and normalize the template points before storing them
            Vector2[] normalizedPoints = ShapeUtils.CenterAndNormalize(template.points);
            var descriptor = FourierTransform.GenerateDescriptor(normalizedPoints);

            templateDescriptors[template.shapeName] = descriptor;
            templatePoints[template.shapeName] = normalizedPoints; // Store normalized points
        }
    }


    public string MatchShape(Vector2[] resampledPoints)
    {
        if (resampledPoints == null || resampledPoints.Length == 0)
        {
            Debug.LogWarning("Resampled points are empty or null.");
            return null;
        }

        string bestMatch = "No Match Found";
        float bestScore = float.MaxValue;

        foreach (var group in shapeTemplates.GroupBy(template => template.shapeType))
        {
            foreach (var template in group)
            {
                float fourierScore = FourierTransform.CompareDescriptors(
                    FourierTransform.GenerateDescriptor(resampledPoints),
                    templateDescriptors[template.shapeName]);

                float dtwScore = DynamicTimeWarpingUtility.Calculate(
                    resampledPoints,
                    ResamplingUtility.ResamplePoints(template.points, resampledPoints.Length));

                float combinedScore = (fourierScore + dtwScore) / 2;

                float adjustedFourierThreshold = fourierThreshold;
                float adjustedDTWThreshold = dtwThreshold;

                if (fourierScore < adjustedFourierThreshold && dtwScore < adjustedDTWThreshold && combinedScore < bestScore)
                {
                    bestScore = combinedScore;
                    bestMatch = template.shapeName;
                }
            }
        }

        return bestScore < minConfidenceThreshold ? bestMatch : "No Match Found";
    }
}
