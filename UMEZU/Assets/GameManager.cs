using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;
    private void Awake()
    {
       
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    #endregion

    [SerializeField] AudioSource blackHoleSound;
    [SerializeField] AudioSource menuTheme;
    [SerializeField] AudioSource gameTheme;
    

    public bool timeStopped;
    public void StopTime()
    {
        timeStopped = true;
        Time.timeScale = 0;
    }
    public void ContinueTime()
    {
        timeStopped = false;
        Time.timeScale = 1;
    }

    

    public void StartGame()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(2).AppendCallback(() =>
        {
            SceneManager.LoadScene("Level1");
        });

        

    }

    public void LoadNextLevel(string nextLevelName)
    {
        Debug.Log("GameManager got the signal");
        string levelName = nextLevelName;
        if (loadSceneRoutine != null)
        {
            StopCoroutine(loadSceneRoutine);
        }
        loadSceneRoutine = StartCoroutine(LoadSceneRoutine(levelName));
        VfxManager.instance.LoadSceneEffect();

       

    }


    Coroutine loadSceneRoutine;

    public IEnumerator LoadSceneRoutine(string nextLevelName)
    {
        menuTheme.Stop();

        if (SceneManager.GetActiveScene().name != "MenuOnly")
        {
            //Immobile the player
            var playerSlime = GetPlayerSlime();
            playerSlime.SetImmovable(true);
        }
        if (blackHoleSound != null)
        {
            blackHoleSound.Play();
        }

        // Start loading the next scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextLevelName);
        asyncLoad.allowSceneActivation = false; // Prevents the scene from activating until explicitly set

        float delay = 1;
        float timer = 0f;

        // Wait until the scene is fully loaded and the delay has passed
        while (!asyncLoad.isDone)
        {
            // Increment the timer
            timer += Time.deltaTime;

            // Check if the loading has finished and the delay has passed
            if (asyncLoad.progress >= 0.9f && timer >= delay)
            {
                VfxManager.instance.StartSceneEffect();

                // Allow the scene to activate
                asyncLoad.allowSceneActivation = true;

                if (SceneManager.GetActiveScene().name != "MenuOnly" && gameTheme.isPlaying == false)
                {
                    menuTheme.Stop();
                    gameTheme.Play();
                    Debug.Log("Game theme a�mal�yd�");
                }

            }
            yield return null; // Continue waiting in the next frame
        }

        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
       

        string thisScene = SceneManager.GetActiveScene().name;
        
        Debug.Log(thisScene);
        LoadNextLevel(thisScene);
    }
    public ozController GetPlayerSlime()
    {
        GameObject[] objectsTaggedPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject objectTaggedPlayer in objectsTaggedPlayer)
        {
            ozController playerSlime;
            objectTaggedPlayer.TryGetComponent<ozController>(out playerSlime);
            if (playerSlime != null)
            {
                return playerSlime;
            }
        }
        Debug.Log("Couldn't find player");
        return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadNextLevel("MenuOnly");
        }
    }

    public void SetMusicVolume(float value)
    {
        menuTheme.volume = value;
        gameTheme.volume = value;
    }

}
