using UnityEngine;
using System.Collections;

public class ScreamMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float screamThreshold = 0.02f;         // Sensitivity for scream detection
    public float upwardForceMultiplier = 4f;      // Multiplier for upward force based on scream volume
    public float maxTiltAngle = 30f;              // Maximum tilt angle for arrow effect
    public float tiltSpeed = 5f;                  // Speed at which the arrow tilts back to center
    public float fallMultiplier = 2.5f;           // Multiplier for faster falling
    public float volumeCheckInterval = 0.2f;      // Increased interval for optimized microphone volume check
    public Vector3 targetPosition;                // Target position to move to at the start
    public float moveDuration = 2f;               // Duration for the initial move to target

    [Header("House Settings")]
    public Transform houseTransform;              // Reference to the house transform
    public float houseBackwardDistance = 5f;      // Distance to move the house backward
    public float houseMoveSpeed = 1f;             // Speed to move the house backward (in units per second)

    private string microphoneName;
    private AudioClip micClip;
    private Rigidbody2D rb;
    private bool isScreaming = false;
    private bool reachedTarget = false;           // Flag to enable tilting only after reaching target
    private float targetAngle = 0f;
    private float volume = 0f;
    private Vector2 velocity;                     // Cached velocity to avoid redundant calls

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure the house is moving smoothly backward over time
        if (houseTransform != null)
        {
            StartCoroutine(MoveHouseBackward());
        }
        else
        {
            Debug.LogError("House Transform is not assigned.");
        }

        // Start smooth movement to the target position
        StartCoroutine(MoveToTargetPosition());

        // Start the microphone if available
        if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0];
            micClip = Microphone.Start(microphoneName, true, 1, 44100);
            StartCoroutine(CheckMicrophoneVolume());
        }
        else
        {
            Debug.LogError("No microphone detected.");
        }
    }

    void Update()
    {
        if (!reachedTarget)
            return; // Skip tilting logic until the target is reached

        velocity = rb.linearVelocity;  // Cache velocity

        if (isScreaming)
        {
            // Apply upward force based on volume
            float upwardForce = volume * upwardForceMultiplier;
            velocity.y = upwardForce;
            targetAngle = Mathf.Clamp(velocity.y * 3f, 0, maxTiltAngle);
        }
        else
        {
            // Apply gravity with multiplier and tilt for natural fall
            targetAngle = Mathf.Clamp(velocity.y * 3f, -maxTiltAngle, 0);
            if (velocity.y < 0)
            {
                velocity.y += Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
        }

        // Only apply vertical velocity; no forward speed
        rb.linearVelocity = new Vector2(0, velocity.y);

        // Apply tilt if targetAngle changes significantly
        if (Mathf.Abs(transform.rotation.eulerAngles.z - targetAngle) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, targetAngle), Time.deltaTime * tiltSpeed);
        }

        // Clamp the position within the screen bounds
        ClampPositionToScreen();
    }

    // Coroutine to smoothly move the arrow to the target position without tilting
    IEnumerator MoveToTargetPosition()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it reaches the target position exactly
        transform.position = targetPosition;

        // Set the flag to enable scream-based movement and tilting after reaching the target
        reachedTarget = true;
    }

    // Coroutine to smoothly move the house backward over time
    IEnumerator MoveHouseBackward()
    {
        Vector3 startPosition = houseTransform.position;
        Vector3 targetHousePosition = startPosition - new Vector3(houseBackwardDistance, 0, 0); // Move left

        float elapsedTime = 0f;

        // Move house smoothly to the target position
        while (elapsedTime < houseBackwardDistance / houseMoveSpeed)
        {
            houseTransform.position = Vector3.Lerp(startPosition, targetHousePosition, elapsedTime * houseMoveSpeed / houseBackwardDistance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the house reaches the target position exactly
        houseTransform.position = targetHousePosition;
    }

    // Coroutine to check microphone volume at intervals
    IEnumerator CheckMicrophoneVolume()
    {
        while (true)
        {
            volume = GetMicrophoneVolume();
            isScreaming = volume > screamThreshold;
            yield return new WaitForSeconds(volumeCheckInterval);
        }
    }

    // Function to get volume from the microphone input
    float GetMicrophoneVolume()
    {
        float[] samples = new float[64];  // Reduced sample size for optimization
        int micPosition = Microphone.GetPosition(microphoneName) - 64 + 1;
        if (micPosition < 0) return 0;
        micClip.GetData(samples, micPosition);

        float sum = 0;
        foreach (float sample in samples)
        {
            sum += sample * sample;
        }

        return Mathf.Sqrt(sum / samples.Length) * 10f;  // Amplified volume
    }

    // Clamp the object's position to stay within the screen bounds
    void ClampPositionToScreen()
    {
        // Get the screen bounds in world coordinates
        Vector3 lowerLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 upperRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

        // Clamp the object's position to stay within the bounds
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, lowerLeft.x, upperRight.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, lowerLeft.y, upperRight.y);

        // Apply the clamped position
        transform.position = clampedPosition;
    }
}
