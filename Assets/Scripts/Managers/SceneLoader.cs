using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    
    [Header("Loading UI")]
    public GameObject loadingPanel;
    public Slider progressBar;
    public Text progressText;
    public Text loadingText;
    
    [Header("Loading Animation")]
    public float loadingTextSpeed = 0.5f;
    public string[] loadingMessages = {
        "Loading...",
        "Loading..",
        "Loading.",
        "Loading.."
    };
    
    private void Awake()
    {
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
    
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        ShowLoadingPanel();
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        
        StartCoroutine(AnimateLoadingText());
        
        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            UpdateProgress(progress);
            yield return null;
        }
        
        UpdateProgress(1f);
        yield return new WaitForSeconds(0.5f);
        
        operation.allowSceneActivation = true;
        HideLoadingPanel();
    }
    
    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        ShowLoadingPanel();
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;
        
        StartCoroutine(AnimateLoadingText());
        
        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            UpdateProgress(progress);
            yield return null;
        }
        
        UpdateProgress(1f);
        yield return new WaitForSeconds(0.5f);
        
        operation.allowSceneActivation = true;
        HideLoadingPanel();
    }
    
    private void ShowLoadingPanel()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
    }
    
    private void HideLoadingPanel()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }
    
    private void UpdateProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
        
        if (progressText != null)
        {
            progressText.text = $"{(progress * 100):F0}%";
        }
    }
    
    private IEnumerator AnimateLoadingText()
    {
        int messageIndex = 0;
        
        while (loadingPanel != null && loadingPanel.activeSelf)
        {
            if (loadingText != null)
            {
                loadingText.text = loadingMessages[messageIndex];
            }
            
            messageIndex = (messageIndex + 1) % loadingMessages.Length;
            yield return new WaitForSeconds(loadingTextSpeed);
        }
    }
} 