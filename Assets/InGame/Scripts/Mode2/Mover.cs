using UnityEngine;

// Base class for common functionality
public class Mover : MonoBehaviour
{
    public float speed = 5f; // Speed of movement
    private float lifetime = 5f; // Default lifetime

    // Update method for moving objects
    protected virtual void Update()
    {
        // Move the object to the left by default
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    // Set the lifetime of the object and schedule destruction
    public void SetLifetime(float time)
    {
        lifetime = time;
        Destroy(gameObject, lifetime);
    }
}

// Obstacle Mover
public class ObstacleMover : Mover
{
    // Custom update for obstacle behavior (if needed)
    protected override void Update()
    {
        base.Update(); // Use the base class movement logic
    }
}

// Boss Mover
public class BossMover : Mover
{
    // Custom behavior for boss movement
    protected override void Update()
    {
        // Example: Boss moves in a wave-like pattern
        float wave = Mathf.Sin(Time.time * speed) * 2f; // Wave effect
        transform.Translate(new Vector2(-speed * Time.deltaTime, wave * Time.deltaTime));
    }
}
