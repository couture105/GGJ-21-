using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public SheepEdgeIndicator edgeIndicator;

    void Start()
    {
        GameManager.Instance.hud = this;
    }

    void Update()
    {
        
    }
}
