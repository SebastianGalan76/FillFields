using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GoogleMobileAds.Api;

public class UILevelFailed : MonoBehaviour
{
    [SerializeField] private Button restartBtn;
    [SerializeField] private GameObject restartIcon;
    [SerializeField] private Text timeLeftText;
    [SerializeField] private GameObject[] skipBtn;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowPanel()
    {
        animator.SetBool("showLevelFailed", true);

        //gameSystem.audioObject[3].Play();

        int time = 3;

        restartBtn.interactable = false;
        restartIcon.SetActive(false);
        timeLeftText.text = time.ToString();

        StartCoroutine(wait());
        IEnumerator wait()
        {
            while (time > 0)
            {
                yield return new WaitForSeconds(1f);
                time--;

                timeLeftText.text = time.ToString();
                if (time == 0)
                {
                    timeLeftText.text = null;
                    restartBtn.interactable = true;
                    restartIcon.SetActive(true);
                }

            }
        }
    }

    public void HidePanel()
    {
        animator.SetBool("showLevelFailed", false);
    }
}
