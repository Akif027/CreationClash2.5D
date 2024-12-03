using UnityEngine;

public class SpearFlightController : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            // Align the forward direction with the velocity
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);
        }
    }
}
