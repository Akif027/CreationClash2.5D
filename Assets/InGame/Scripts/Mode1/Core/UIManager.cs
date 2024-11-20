using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] GameObject GameOverPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ToggleGameOverPanel(false);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    public void ToggleGameOverPanel(bool istrue)
    {

        GameOverPanel.SetActive(istrue);

    }


    public void LoadScene()
    {

        SceneManager.LoadScene(0);

    }
    // Other methods and properties can be added here
}