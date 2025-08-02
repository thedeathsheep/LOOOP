using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public string mainMenuSceneName = "MainMenu";
    public string firstLevelSceneName = "Level1";
    
    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 游戏开始时加载保存的设置
        LoadGameSettings();
    }
    
    public void SaveGameProgress(string sceneName)
    {
        PlayerPrefs.SetString("SavedGame", sceneName);
        PlayerPrefs.Save();
        Debug.Log($"Game progress saved at scene: {sceneName}");
    }
    
    public void LoadGameProgress()
    {
        if (PlayerPrefs.HasKey("SavedGame"))
        {
            string savedScene = PlayerPrefs.GetString("SavedGame");
            SceneManager.LoadScene(savedScene);
        }
        else
        {
            Debug.Log("No saved game found");
        }
    }
    
    public void StartNewGame()
    {
        // 清除保存的游戏数据
        PlayerPrefs.DeleteKey("SavedGame");
        PlayerPrefs.Save();
        
        // 加载第一个关卡
        SceneManager.LoadScene(firstLevelSceneName);
    }
    
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void LoadGameSettings()
    {
        // 加载音频设置
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            AudioManager.Instance?.SetMasterVolume(masterVolume);
        }
        
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            AudioManager.Instance?.SetMusicVolume(musicVolume);
        }
        
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            AudioManager.Instance?.SetSFXVolume(sfxVolume);
        }
        
        // 加载分辨率设置
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            Resolution[] resolutions = Screen.resolutions;
            if (resolutionIndex < resolutions.Length)
            {
                Resolution resolution = resolutions[resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
        }
        
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            bool fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
            Screen.fullScreen = fullscreen;
        }
    }
} 