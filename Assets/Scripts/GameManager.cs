using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int playerScore = 0; // This persists between scenes
    private int numWaves = 0; 

    public int Score {get => playerScore; set => playerScore = value; }
    public int Waves {get => numWaves; set => numWaves = value; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps it alive across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    public static void EnsureInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("GameManager");
            Instance = obj.AddComponent<GameManager>();
            DontDestroyOnLoad(obj);
        }
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void LoadNextWave()
    {
        StartCoroutine(LoadSceneAfterDelay("LevelScene", 3.0f)); 
    }

    public void StartNewGame()
    {
        Score = 0; 
        SceneManager.LoadScene("LevelScene");
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }

}
