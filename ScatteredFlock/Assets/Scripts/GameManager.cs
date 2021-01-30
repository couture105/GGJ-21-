using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameStates
    {
        Splash = 0,
        Menu = 1,
        Game = 2,
        PostGame = 3
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyChildOnLoad(_instance.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Instance fo the singleton
    /// </summary>
    public static GameManager Instance => _instance;

    private static GameManager _instance;

    public static void DontDestroyChildOnLoad(GameObject child)
    {
        Transform parentTransform = child.transform;

        // If this object doesn't have a parent then its the root transform.
        while (parentTransform.parent != null)
        {
            // Keep going up the chain.
            parentTransform = parentTransform.parent;
        }
        GameObject.DontDestroyOnLoad(parentTransform.gameObject);
    }

    public HUD hud;
    public Level level;
    public GameStates state = GameStates.Splash;

    public void SetState(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.Splash:
            {
                SceneManager.LoadScene("SplashScene");
                break;
            }

            case GameStates.Menu:
            {
                SceneManager.LoadScene("MenuScene");
                break;
            }

            case GameStates.Game:
            {
                SceneManager.LoadScene("GameScene");
                break;
            }

            case GameStates.PostGame:
            {
                SceneManager.LoadScene("PostGameScene");
                break;
            }
        }

        state = newState;
    }
}
