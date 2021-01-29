using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyAnim : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator != null && animator.enabled)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            {
                Destroy(gameObject);
            }
        }
    }
}
