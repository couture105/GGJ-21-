using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class ButtonsHandler : MonoBehaviour
{
    public void ChangeState(int state)
    {
        GameManager.Instance.SetState((GameStates)state);
    }
}
