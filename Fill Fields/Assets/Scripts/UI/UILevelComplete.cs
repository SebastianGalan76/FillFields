using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UILevelComplete : MonoBehaviour
{
    [SerializeField] private Text levelNrText;
    [SerializeField] private ParticleSystem[] confettiParticle;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowPanel(int levelNr)
    {
        animator.Play("ShowLevelCompleted");
        levelNrText.text = "Level " + levelNr;

        SoundSystem.GetInstance().PlaySound("launchConfetti");

        ShowConfetti();
        void ShowConfetti()
        {
            confettiParticle[0].Play();
            confettiParticle[1].Play();
        }
    }
}
