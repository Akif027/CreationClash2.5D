using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ShapeRecognitionPlugin.Editor
{
    public class ShapeTemplateEditor : EditorWindow
    {
        private List<Vector2> shapePoints = new List<Vector2>();
        private string shapeName = "";

        private bool showSmoothingOptions = false; // Toggle for showing the slider
        private int smoothingWindow = 3; // Default smoothing intensity

        [MenuItem("Tools/Shape Recognition Plugin/Shape Editor")]
        public static void OpenWindow()
        {
            GetWindow<ShapeTemplateEditor>("Shape Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Shape Drawing Tool", EditorStyles.boldLabel);
            shapeName = EditorGUILayout.TextField("Shape Name", shapeName);

            if (GUILayout.Button("Start Drawing"))
            {
                shapePoints.Clear();
                SceneView.duringSceneGui += OnSceneGUI;
                showSmoothingOptions = false; // Reset smoothing option visibility
            }

            if (GUILayout.Button("Stop Drawing"))
            {
                SceneView.duringSceneGui -= OnSceneGUI;
                SaveShape();
            }

            if (GUILayout.Button("Clear Shape"))
            {
                shapePoints.Clear();
                SceneView.RepaintAll();
                showSmoothingOptions = false; // Reset smoothing option visibility
            }

            // Smooth Shape Button
            if (GUILayout.Button("Smooth Shape"))
            {
                ApplySmoothing();
                showSmoothingOptions = true; // Show the slider after smoothing
            }

            // Show the smoothing slider if the option is enabled
            if (showSmoothingOptions)
            {
                smoothingWindow = EditorGUILayout.IntSlider("Smoothing Intensity", smoothingWindow, 1, 10);
                if (GUILayout.Button("Apply Smoothing with Intensity"))
                {
                    ApplySmoothing(); // Apply smoothing again with the adjusted smoothing window
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Click and drag in the Scene view to draw.");
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            {
                Vector2 mousePos = e.mousePosition;
                mousePos.y = sceneView.camera.pixelHeight - mousePos.y;
                Vector2 worldPos = sceneView.camera.ScreenToWorldPoint(mousePos);

                if (shapePoints.Count == 0 || Vector2.Distance(shapePoints[^1], worldPos) > 0.1f)
                {
                    shapePoints.Add(worldPos);
                    SceneView.RepaintAll();
                }
                e.Use();
            }

            for (int i = 0; i < shapePoints.Count - 1; i++)
            {
                Handles.DrawLine(shapePoints[i], shapePoints[i + 1]);
            }
        }

        private void SaveShape()
        {
            if (shapePoints.Count < 3 || string.IsNullOrEmpty(shapeName))
            {
                Debug.LogWarning("Shape is invalid or has no name.");
                return;
            }

            string directoryPath = "Assets/ShapeRecognition/Resources/Shapes";
            if (!AssetDatabase.IsValidFolder(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
                AssetDatabase.Refresh();
            }

            ShapeTemplate template = ScriptableObject.CreateInstance<ShapeTemplate>();
            template.shapeName = shapeName;
            template.points = shapePoints.ToArray();

            AssetDatabase.CreateAsset(template, $"{directoryPath}/{shapeName}.asset");
            AssetDatabase.SaveAssets();

            Debug.Log($"Shape '{shapeName}' saved with {shapePoints.Count} points.");
        }

        private void ApplySmoothing()
        {
            if (shapePoints == null || shapePoints.Count < 3)
            {
                Debug.LogWarning("Not enough points to smooth.");
                return;
            }

            // Apply uniform smoothing to all points
            shapePoints = new List<Vector2>(ShapeUtils.SmoothPoints(shapePoints.ToArray(), smoothingWindow));

            Debug.Log("Shape smoothed with intensity " + smoothingWindow);
            SceneView.RepaintAll();
        }

    }
}
