using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuMaker : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private string mainMenuSceneName = "Main Menu";
    
    
    private bool paused;
    private PlayerControls playerControls;
    
    public void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.PauseToggle.performed += PauseGame;
        SetupPause();
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        paused = !paused;
        SetupPause();
    }

    public void Resume()
    {
        paused = false;
        SetupPause();
    }

    public void SetupPause()
    {
        PlayerPrefs.Save();
        
        Time.timeScale = paused ? 0 : 1;
        menu.gameObject.SetActive(paused);

        if (PlayerCamera.Instance != null)
        {
            PlayerCamera.Instance.enabled = !paused;
        }
    }

    public void ReturnToMenu()
    {
        PlayerPrefs.Save();
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
