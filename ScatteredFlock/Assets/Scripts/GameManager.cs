using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        }
        DontDestroyChildOnLoad(_instance.gameObject);
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
}
