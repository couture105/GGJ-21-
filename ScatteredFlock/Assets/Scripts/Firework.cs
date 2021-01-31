using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour
{
    public Color[] colors;
    
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("speedMultiplier", Random.Range(0.6f, 2.0f));
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && colors != null && colors.Length > 0)
        {
            int i = Random.Range(0, colors.Length);
            spriteRenderer.color = colors[i];
        }

        float scale = Random.Range(0.8f, 3.0f);
        transform.localScale = new Vector3(scale, scale, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
