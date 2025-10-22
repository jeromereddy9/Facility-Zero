using UnityEngine;
using UnityEngine.SceneManagement;
using FacilityZero.Manager;

public class PauseMenuSimple : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    private bool isPaused = false;

    private FPInputManager inputManager;

    private void Start()
    {
        // Ensure pause menu is hidden at start
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Lock and hide the cursor initially
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;

        // Get FPInputManager
        inputManager = FindObjectOfType<FPInputManager>();
        if (inputManager == null)
            Debug.LogError("PauseMenu: FPInputManager not found in scene!");
    }

    private void Update()
    {
        if (inputManager != null && inputManager.PausePressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
