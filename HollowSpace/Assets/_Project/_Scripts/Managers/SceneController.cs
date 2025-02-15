using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public const int MAIN_MENU = -1;
    public const int PLAYER = 0;
    public const int LEVEL_OFFSET = 1;

    [SerializeField] private GameObject loadingScreen;
    private int _activeLevel = -1;

    public static SceneController Instance;

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

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(PLAYER);
        SceneManager.LoadScene(LEVEL_OFFSET + level, LoadSceneMode.Additive);
        _activeLevel = level;
    }

    public void LoadLevelAsync(int level)
    {
        LoadSceneAsync(PLAYER);
        LoadSceneAsync(LEVEL_OFFSET + level, LoadSceneMode.Additive);
        _activeLevel = level;
    }


    public void LoadMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU);
    }

    public void LoadMainMenuAsync()
    {
        LoadSceneAsync(MAIN_MENU);
    }

    public void LoadSceneAsync(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(buildIndex, mode));
    }
    private IEnumerator LoadSceneAsyncCoroutine(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        loadingScreen.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex, mode);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        loadingScreen.SetActive(false);
    }

    public int GetActiveLevel() => _activeLevel;
}