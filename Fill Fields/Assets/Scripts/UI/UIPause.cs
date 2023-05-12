using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPause : MonoBehaviour
{
    [HideInInspector] public bool isPaused;

    [SerializeField] private Animator buttonAnim;
    
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PauseGame(bool value)
    {
        buttonAnim.SetBool("show", value);
        animator.SetBool("showPause", value);

        isPaused = value;
    }

    public void ChangePause()
    {
        isPaused = !isPaused;
        PauseGame(isPaused);
    }
}
