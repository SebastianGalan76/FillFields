using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHint : MonoBehaviour
{
    [SerializeField] private GameObject[] directions;
    [SerializeField] private GameObject[] hintBtn;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowHints()
    {
        animator.SetBool("show", true);
    }

    public void HideHints()
    {
        animator.SetBool("show", false);
    }

    public void ChangeDirection(int directionID)
    {
        for (int i = 0; i < 4; i++)
        {
            directions[i].SetActive(false);
        }

        directions[directionID].SetActive(true);
    }

    public void RefreshHintButton()
    {
        int value = PlayerPrefs.GetInt("BHintValue");

        hintBtn[0].SetActive(false);
        hintBtn[1].SetActive(false);
        hintBtn[2].SetActive(false);

        hintBtn[value].SetActive(true);
    }
}
