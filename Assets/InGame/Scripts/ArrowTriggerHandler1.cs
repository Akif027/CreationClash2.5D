using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using UnityEngine.SceneManagement; // For scene reloading
using UnityEditor;

public class ArrowTriggerHandler : MonoBehaviour
{
    public TMP_Text scoreText;       // Reference to the score TMP_Text
    public GameObject retryButton;  // Reference to the Retry button
    private int score = 0;          // Player's score

    private void Start()
    {
        UpdateScoreText();
        retryButton.SetActive(false); // Ensure retry button is hidden initially
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectables")) // Check if collided with an obstacle
        {
            // Increment the score
            score += 10;

            // Update the score text
            UpdateScoreText();

            // Return the obstacle to the pool
            var obstacleSpawner = FindObjectOfType<ObstacleSpawner>();
            if (obstacleSpawner != null)
            {
                obstacleSpawner.ReturnObstacle(other.gameObject);
            }
        }
        else if (other.CompareTag("Boss")) // Check if collided with the boss
{
    // Check if the boss object is not null before disabling it
    if (other.gameObject != null)
    {
        other.gameObject.SetActive(false);
    }
    else
    {
        Debug.LogWarning("Boss object is already destroyed or missing!");
    }

    // Show the Retry button
    retryButton.SetActive(true);
}
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    // Button action to retry
    public void OnRetryButtonClicked()
    {
        // Reload the current scene to retry
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitApplication()
    {
        // Check if the game is running in the Unity Editor or a built version
#if UNITY_EDITOR
        // Stop the play mode in the Unity Editor
        EditorApplication.isPlaying = false;
#else
        // Quit the application when it's a built version
        Application.Quit();
#endif
    }

    // Example method to save game progress

}
