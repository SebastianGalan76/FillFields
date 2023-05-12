using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    public BLevel[] levelsBtn;
    public Text[] starAmountText;

    [HideInInspector] public GameSystem gameSystem;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gameSystem = GameObject.FindGameObjectWithTag("GameSystem").GetComponent<GameSystem>();

        //Refresh all level button
        for (int i = 0; i < levelsBtn.Length; i++)
        {
            RefreshLevelButton(i + 1);
            levelsBtn[i].Initialize(this);
        }

        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        ChangeStarAmount();

        animator.SetBool("showMainMenu", true);
        animator.SetBool("showLevelList", false);

        void ChangeStarAmount()
        {
            string value = PlayerPrefs.GetInt("Stars").ToString();
            starAmountText[0].text = value;
            starAmountText[1].text = value;
        }
    }
    public void HideMainMenu()
    {
        animator.SetBool("showMainMenu", false);
    }
    public void ShowLevelList()
    {
        animator.SetBool("showMainMenu", false);
        animator.SetBool("showLevelList", true);
    }

    public void PlayGame(bool playLastGame = false)
    {
        animator.SetBool("showMainMenu", false);
        animator.SetBool("showLevelList", false);

        if (playLastGame)
        {
            PlayerPrefs.SetInt("levelNr", PlayerPrefs.GetInt("LevelToPlay"));
        }

        StartCoroutine(wait());
        IEnumerator wait()
        {
            yield return new WaitForSeconds(0.25f);
            SceneManager.LoadScene("Game");
        }
    }

    public void ShowLeaderboard()
    {
        if(CheckInternetConnection())
            GooglePlayServices.ShowLeaderboards();
    }
    public void ShowAchievements()
    {
        if (CheckInternetConnection())
            GooglePlayServices.ShowAchievements();
    }

    public void RefreshLevelButton(int levelNr)
    {
        if (levelsBtn.Length >= levelNr)
        {
            levelsBtn[levelNr - 1].RefreshButton();
        }
    }

    public void RateGame()
    {
        if (CheckInternetConnection())
            Application.OpenURL("market://details?id=com.Coresaken.FilltheFields");
    }

    public bool CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UIError.GetInstance().Show("Error! Check your Internet connection!");
            return false;
        }

        return true;
    }
}