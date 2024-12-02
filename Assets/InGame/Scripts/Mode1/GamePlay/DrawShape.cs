using System.Collections.Generic;
using ShapeRecognitionPlugin;
using UnityEngine;
using UnityEngine.UI;

public class DrawShape : MonoBehaviour
{
    [SerializeField] private ShapeRecognizer recognizer; // Reference to the ShapeRecognizer
    [SerializeField] private LineRenderer drawingLineRenderer; // Used for drawing the shape
    [SerializeField] private int minPointsToMatch = 10; // Minimum points to consider a valid drawing
    [SerializeField] private Button EnterDrawB;
    [SerializeField] private Button ChangeDrawB;
    public Transform SpawnPoint; // Spawn point for the weapon
    private List<Vector2> drawnPoints = new List<Vector2>(); // List to store drawn points
    private bool isFreeDrawingMode = true; // Toggle for drawing modes
    public bool InDrawingState = true; // Initial drawing state

    private Rect drawingBounds; // Rectangle bounds for drawing

    private void Start()
    {
        // Define the rectangular bounds in world space
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.25f, Screen.height * 0.25f, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.75f, Screen.height * 0.75f, 0));

        drawingBounds = new Rect(
            bottomLeft.x,
            bottomLeft.y,
            topRight.x - bottomLeft.x,
            topRight.y - bottomLeft.y
        );

        Debug.Log($"Drawing Bounds: {drawingBounds}");

        // Assign button actions
        EnterDrawB.onClick.AddListener(AnalyzeDraw);
        ChangeDrawB.onClick.AddListener(ChangeMode);

        // Subscribe to events to disable/enable drawing when required
        EventManager.Subscribe("OnWeaponSpawned", DisableDrawing);
        EventManager.Subscribe("OnWeaponReleased", EnableDrawing);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        EventManager.Unsubscribe("OnWeaponSpawned", DisableDrawing);
        EventManager.Unsubscribe("OnWeaponReleased", EnableDrawing);
    }

    private void Update()
    {
        if (InDrawingState)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ClearDrawing(); // Clear drawing on right-click
        }

        if (isFreeDrawingMode && Input.GetMouseButton(0))
        {
            AddPointToDrawing(); // Free drawing
        }
        else if (!isFreeDrawingMode && Input.GetMouseButtonDown(0))
        {
            AddPointToDrawing(); // Point-to-point drawing
        }
    }

    private void ChangeMode()
    {
        isFreeDrawingMode = !isFreeDrawingMode;
        ClearDrawing();
        Debug.Log(isFreeDrawingMode ? "Free Drawing Mode" : "Point-to-Point Mode");
    }

    private void AnalyzeDraw()
    {
        if (drawnPoints.Count >= minPointsToMatch)
        {
            ProcessDrawing(); // Process the shape if enough points are drawn
        }
        else
        {
            Debug.LogWarning("Drawing too short to match a shape.");
        }
    }

    private void AddPointToDrawing()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (drawingBounds.Contains(mousePos))
        {
            if (drawnPoints.Count == 0 || Vector2.Distance(drawnPoints[^1], mousePos) > 0.1f)
            {
                drawnPoints.Add(mousePos);
                drawingLineRenderer.positionCount = drawnPoints.Count;
                drawingLineRenderer.SetPosition(drawnPoints.Count - 1, mousePos);
            }
        }
        else
        {
            //  Debug.LogWarning($"Point {mousePos} is outside of bounds {drawingBounds}");
        }
    }

    private void ProcessDrawing()
    {
        Vector2[] normalizedPoints = ShapeUtils.CenterAndNormalize(drawnPoints.ToArray());
        Vector2[] resampledPoints = ResamplingUtility.ResamplePoints(normalizedPoints, recognizer.descriptorPointsCount);

        WeaponData matchedWeapon = recognizer.GetWeaponDataFromShape(new List<Vector2>(resampledPoints));

        if (matchedWeapon != null)
        {

            Debug.Log($"Matched Weapon: {matchedWeapon.WeaponPrefabName}");
            ClearDrawing();

            // Trigger weapon-matched event
            EventManager.TriggerEvent("OnWeaponMatched", matchedWeapon);
        }
        else
        {
            Debug.Log("No weapon matched.");
            ClearDrawing();
        }
    }

    private void ClearDrawing()
    {
        drawnPoints.Clear();
        if (drawingLineRenderer != null)
        {
            drawingLineRenderer.positionCount = 0;
        }
        else
        {
            Debug.LogWarning("DrawingLineRenderer is null while attempting to clear drawing.");
        }
    }

    private void DisableDrawing()
    {
        InDrawingState = false;
    }

    private void EnableDrawing()
    {
        InDrawingState = true;
    }
}
