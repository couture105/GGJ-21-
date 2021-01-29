using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Sheep;

public class Pen : MonoBehaviour
{
    public Sheep.SheepType acceptedSheepType;
    public float penRadius = 4;
    public Level level;

    public int winScore = 10;
    public int currentScore = 0;

    protected SpriteRenderer spriteRenderer;

    public TextMeshPro scoreText;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        switch (acceptedSheepType)
        {
            case SheepType.Red:
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.red;
                }
                break;
            }

            case SheepType.Green:
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.green;
                }
                break;
            }

            case SheepType.Blue:
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.blue;
                }
                break;
            }
        }
    }

    public void DeltaUpdate(float dt)
    {
        for (int i = 0; i < level.maxSheeps; i++)
        {
            Sheep sheep = level.sheeps[i];
            if (sheep.active && (sheep.type == acceptedSheepType))
            {
                if ((sheep.transform.position - transform.position).magnitude < penRadius)
                {
                    level.DestroySheep(sheep);
                    currentScore++;
                }
            }
        }

        if (scoreText != null)
        {
            scoreText.text = currentScore + "/" + winScore;
        }
    }

    public void CleanUp()
    {
        currentScore = 0;
    }

}
