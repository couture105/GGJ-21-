using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI losesText;
    void Start()
    {
        timeText.text = GameManager.Instance.time.ToString(@"mm\:ss");
        scoreText.text = GameManager.Instance.foundSheeps.ToString();
        losesText.text = GameManager.Instance.eatenSheeps.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
