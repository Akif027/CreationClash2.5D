using System.Collections;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{



    [System.Serializable]
    public class BackgroundLayer
    {
        public Transform[] layerTransforms; // Array of two Transforms for each layer
        public float parallaxFactor;        // Speed factor for the layer (1 for base layer)
        public float layerWidth;            // Width of the layer for repositioning
    }

    public BackgroundLayer[] layers;       // Array to store each background layer (two copies per layer)
    public float scrollSpeed = 2f;         // Base scroll speed

    void Update()
    {
        // Move each layer based on its parallax factor and the scroll speed
        foreach (var layer in layers)
        {
            float moveDistance = scrollSpeed * layer.parallaxFactor * Time.deltaTime;

            // Move both copies of each layer
            foreach (var layerTransform in layer.layerTransforms)
            {
                layerTransform.position -= new Vector3(moveDistance, 0, 0);
            }

            // Check if the leftmost copy has moved completely out of view
            if (layer.layerTransforms[0].position.x <= -layer.layerWidth)
            {
                RepositionLayer(layer);
            }
        }
    }

    void RepositionLayer(BackgroundLayer layer)
    {
        // Determine which is the leftmost and which is the rightmost layer
        Transform leftmost = layer.layerTransforms[0];
        Transform rightmost = layer.layerTransforms[1];

        if (leftmost.position.x > rightmost.position.x)
        {
            leftmost = layer.layerTransforms[1];
            rightmost = layer.layerTransforms[0];
        }

        // Reposition the leftmost layer to the right of the rightmost layer
        leftmost.position = new Vector3(rightmost.position.x + layer.layerWidth, leftmost.position.y, leftmost.position.z);

        // Swap references to keep them in order
        layer.layerTransforms[0] = rightmost;
        layer.layerTransforms[1] = leftmost;
    }
}
