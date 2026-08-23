using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameHUD; 
    
    [Header("Audio")]
    public AudioClip buttonSound;
    private AudioSource audioSource;
    
    private const string AutoStartKey = "AutoStartGame";
    private const string SnorlaxSpeedKey = "SnorlaxSpeed";

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        // Check if game should auto-start (e.g., after a restart)
        if (PlayerPrefs.GetInt(AutoStartKey, 0) == 1)
        {
            PlayerPrefs.SetInt(AutoStartKey, 0); // Reset the flag
            if (gameHUD != null) gameHUD.SetActive(true); 
            StartGame();
        }
        else
        {
            // Pause the game and show the main menu
            Time.timeScale = 0;
            mainMenuPanel.SetActive(true);
            if (gameHUD != null) gameHUD.SetActive(false); 
        }
    }

    public void SetEasyDifficulty()
    {
        // Set speed to 2.0 (Easy)
        PlayerPrefs.SetFloat(SnorlaxSpeedKey, 2.0f);
        PlayButtonSound();
        Debug.Log("Difficulty Set: Easy (Speed 2)");
    }

    public void SetMediumDifficulty()
    {
        // Set speed to 5.0 (Medium)
        PlayerPrefs.SetFloat(SnorlaxSpeedKey, 5.0f);
        PlayButtonSound();
        Debug.Log("Difficulty Set: Medium (Speed 5)");
    }

    public void SetHardDifficulty()
    {
        // Set speed to 10.0 (Hard)
        PlayerPrefs.SetFloat(SnorlaxSpeedKey, 10.0f);
        PlayButtonSound();
        Debug.Log("Difficulty Set: Hard (Speed 10)");
    }

    public void PlayButtonSound()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
    }

    public void StartGame()
    {
        PlayButtonSound();
        
        // Ensure a default difficulty is set if none exists
        if (!PlayerPrefs.HasKey(SnorlaxSpeedKey))
        {
            PlayerPrefs.SetFloat(SnorlaxSpeedKey, 2.0f); // Default to Easy
        }

        // Update enemy speed based on settings
        EnemyMovement enemy = FindObjectOfType<EnemyMovement>();
        if (enemy != null)
        {
            enemy.UpdateSpeed();
        }

        // Unpause the game
        Time.timeScale = 1;
        
        // Hide menu and show game HUD
        mainMenuPanel.SetActive(false);
        if (gameHUD != null) gameHUD.SetActive(true);
    }
    
    public void RestartGame()
    {
        PlayButtonSound();
        PlayerPrefs.SetInt(AutoStartKey, 0); 
        PlayerPrefs.Save();
        
        // Unpause to allow scene reload to work correctly
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
