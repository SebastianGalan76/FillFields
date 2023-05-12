using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class UILevel : MonoBehaviour
{
    public UILevelComplete levelComplete;
    public UILevelFailed levelFailed;
    public UIPause pause;
    public UIHint hint;

    [SerializeField] private LevelManager levelManager;

    [SerializeField] private Text[] levelNrText;
    [SerializeField] private Text[] movementLimitText;
    [SerializeField] private Text[] movementBackLimitText;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartNewLevel(int levelNr, int movementLimit, int movementBackLimit)
    {
        RefreshLevelNr(levelNr);
        RefreshMovementLimit(movementLimit);
        RefreshMovementBackLimit(movementBackLimit);

        levelFailed.HidePanel();
        hint.RefreshHintButton();

        ShowPanel();
        pause.PauseGame(false);
    }

    public void RefreshLevelNr(int levelNr)
    {
        levelNrText[0].text = "lvl "+levelNr;
        levelNrText[1].text = "lvl " + levelNr;
    }

    public void RefreshMovementLimit(int movementLimit)
    {
        movementLimitText[0].text = movementLimit.ToString();
        movementLimitText[1].text = movementLimit.ToString();
    }

    public void RefreshMovementBackLimit(int movementBackLimit)
    {
        movementBackLimitText[0].text = movementBackLimit.ToString();
        movementBackLimitText[1].text = movementBackLimit.ToString();
    }

    public void RestartGame()
    {
        levelFailed.HidePanel();
        levelManager.StartNewLevel(false);
    }

    public void OpenMainMenu()
    {
        animator.SetBool("show", false);

        pause.PauseGame(false);
        hint.HideHints();
        levelFailed.HidePanel();

        levelManager.platform.HideAllPlatforms(false);

        StartCoroutine(wait());
        IEnumerator wait()
        {
            yield return new WaitForSeconds(0.8f);

            SceneManager.LoadScene("MainMenu");
        }
    }

    public void MoveBack()
    {
        if (levelManager.MovementBackLimit > 0)
        {
            if (levelManager.player.currentMovement > 0)
            {
                levelManager.player.PrepareMovementBack();
            }
        }
        else
        {
            AdSystem.GetInstance().ShowRewardedAd(2);
        }
    }

    public void ShowHint()
    {
        if (!levelManager.player.hint.isEnable)
        {
            AdSystem.GetInstance().ShowRewardedAd(3);
        }
    }

    public void ShowPanel()
    {
        animator.SetBool("show", true);
        hint.RefreshHintButton();
    }

    public void HidePanel()
    {
        animator.SetBool("show", false);
    }

    public void SkipLevel()
    {
        if (levelManager.nextLevelStatus == LevelStatus.LOCKED)
        {
            PlayerPrefs.SetInt("unlockLevelNr", (levelManager.LevelNr + 1));
            PlayerPrefs.Save();

            AdSystem.GetInstance().ShowRewardedAd(1);
        }
        else
        {
            PlayerPrefs.SetInt("levelNr", (levelManager.LevelNr + 1));
            PlayerPrefs.Save();
            levelManager.StartNewLevel(false);
        }
    }
}
