using System.Collections;
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
    public Transform SpawnPoint;
    private List<Vector2> drawnPoints = new List<Vector2>(); // List to store drawn points
    private bool isFreeDrawingMode = true; // Toggle for drawing modes
    public bool InDrawingState = false;

    private Rect drawingBounds; // Rectangle bounds for drawing
    public AnimationController PlayerAnimationController;
    private void Start()
    {
        // Define the rectangular bounds in world space
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.25f, Screen.height * 0.25f, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.75f, Screen.height * 0.75f, 0));

        // Create the drawing rectangle based on these points
        drawingBounds = new Rect(
            bottomLeft.x,
            bottomLeft.y,
            topRight.x - bottomLeft.x,
            topRight.y - bottomLeft.y
        );

        Debug.Log($"Drawing Bounds: {drawingBounds}");

        EnterDrawB.onClick.AddListener(AnalyzeDraw);
        ChangeDrawB.onClick.AddListener(ChangeMode);
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Clear the drawing when right-clicking
        if (Input.GetMouseButtonDown(1))
        {
            ClearDrawing();
        }

        // Handle drawing in free mode
        if (isFreeDrawingMode && InDrawingState && Input.GetMouseButton(0))
        {
            AddPointToDrawing();
        }
        // Handle point-to-point drawing
        else if (!isFreeDrawingMode && InDrawingState && Input.GetMouseButtonDown(0))
        {
            AddPointToDrawing();
        }
    }

    private void ChangeMode()
    {
        isFreeDrawingMode = !isFreeDrawingMode;
        Debug.Log(isFreeDrawingMode ? "Free Drawing Mode" : "Point-to-Point Mode");
        ClearDrawing();
    }

    private void AnalyzeDraw()
    {
        if (drawnPoints.Count >= minPointsToMatch)
        {
            Debug.Log("Processing drawing...");
            ProcessDrawing();
        }
        else
        {
            Debug.LogWarning("Drawing too short to match a shape.");
        }
    }

    private void AddPointToDrawing()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check if the mouse position is within the rectangular bounds
        if (drawingBounds.Contains(mousePos))
        {
            // Add point only if far enough from the last point
            if (drawnPoints.Count == 0 || Vector2.Distance(drawnPoints[^1], mousePos) > 0.1f)
            {
                drawnPoints.Add(mousePos);
                drawingLineRenderer.positionCount = drawnPoints.Count;
                drawingLineRenderer.SetPosition(drawnPoints.Count - 1, mousePos);
            }
        }
        else
        {
            Debug.LogWarning($"Point {mousePos} is outside of bounds {drawingBounds}");
        }
    }

    private void ProcessDrawing()
    {
        // Center and normalize the drawn points before resampling
        Vector2[] normalizedPoints = ShapeUtils.CenterAndNormalize(drawnPoints.ToArray());

        // Resample points to a fixed count for matching
        Vector2[] resampledPoints = ResamplingUtility.ResamplePoints(normalizedPoints, recognizer.descriptorPointsCount);

        // Match the shape and get the weapon data
        WeaponData matchedWeapon = recognizer.GetWeaponDataFromShape(new List<Vector2>(resampledPoints));

        if (matchedWeapon != null)
        {
            // Shape matched, proceed with weapon instantiation
            ClearDrawing();
            Debug.Log($"Matched Weapon: {matchedWeapon.WeaponPrefabName}");

            // Instantiate the weapon prefab
            if (matchedWeapon.WeaponPrefab != null)
            {
                GameObject weaponInstance = Instantiate(matchedWeapon.WeaponPrefab, SpawnPoint.position, Quaternion.identity, SpawnPoint);
                ProjectileWeapon weapon = weaponInstance.GetComponent<ProjectileWeapon>();
                weapon.animationController = PlayerAnimationController;
                weapon.AssignWeaponData(matchedWeapon);

                Debug.Log($"Instantiated Weapon: {matchedWeapon.WeaponPrefabName}");
                GameManager.Instance.ShowPopUpText($"{matchedWeapon.WeaponPrefabName}");
                InDrawingState = false; // Disable drawing state
                StartCoroutine(EnableDrawingStateAfterDelay(1f));
            }
            else
            {
                Debug.LogWarning($"Weapon prefab for {matchedWeapon.WeaponPrefabName} is missing!");
            }
        }
        else
        {
            // No shape matched, clear the drawing
            ClearDrawing();
            Debug.Log("No weapon matched. Drawing cleared.");
            GameManager.Instance.ShowPopUpText("No weapon matched");
        }
    }

    private void ClearDrawing()
    {
        drawnPoints.Clear();
        drawingLineRenderer.positionCount = 0;
        Debug.Log("Cleared drawn points.");
    }

    private IEnumerator EnableDrawingStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        InDrawingState = true;
        Debug.Log("Drawing state enabled after delay.");
    }
}
