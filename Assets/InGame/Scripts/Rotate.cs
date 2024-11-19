using UnityEngine;

public class Rotate : MonoBehaviour
{

    public Vector3 rotationSpeed = new Vector3(0, 0, 50); // Rotation speed in degrees per second

    void Update()
    {
        // Rotate the object based on rotationSpeed and time
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}

