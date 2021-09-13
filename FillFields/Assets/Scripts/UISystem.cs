using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    public GameSystem gameSystem;
    bool nextLevelIsLocked;
    int levelNr;

    [HideInInspector]public LevelManager lvl;
    [HideInInspector]public bool isPaused;

    private Animator UIAnimator;
    public Animator PSettings_Anim;
    public Text TErrorInfo;
    public Animator ErrorObj;

    [Header("MainMenu")]
    public BLevel[] BLevel;
    public Animator StarAmount_Anim;
    public Text[] starAmount_Text;
    public bool MainMenuIsOpen;

    [Header("LevelComplete")]
    public Animator LevelComplete_Anim;
    public Text LevelComplete_TLevelNr;
    public ParticleSystem[] confettiParticle;

    [Header("LevelFailed")]
    public Animator LevelFailed_Anim;
    public Button LevelFailed_BRestart;
    public GameObject LevelFailed_RestartIcon;
    public Text LevelFailed_Time;
    public GameObject[] LevelFailed_BSkip;

    [Header("Hint")]
    public Animator PHint_Anim;
    public GameObject[] Arrows;

    [Header("Level")]
    public Animator LevelUI_Anim;
    public Text[] LevelNr_Text;
    public Text[] MovementLimit_Text;
    public Text[] MovementBackLimit_Text;
    public GameObject BBack_AdInformation, BBack_LimitInformation;
    public Animator BMenu;
    public GameObject[] BHint;

    [Header("Pause")]
    public GameObject[] BSkip;

    [Header("Settings")]
    public Slider audioSlider, musicSlider;
    public Image BGPAutoLogin;
    public Sprite[] SGPAutoLogin;

    private void Start()
    {
        UIAnimator = GetComponent<Animator>();

        ShowMainMenu();
        SetAudio();
        SetMusic();

        //Refresh all level button
        for (int i = 0; i < BLevel.Length; i++)
        {
            RefreshLevelButton(i + 1);
            BLevel[i].Initialize(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowMainMenu();
        }
    }

    public void ShowMainMenu()
    {
        UIAnimator.SetBool("showPause", false);
        UIAnimator.SetBool("showMainMenu", true);
        UIAnimator.SetBool("showLevelList", false);

        MainMenuIsOpen = true;

        if (lvl != null) { lvl.HidePlatforms(false); ShowLevelUI(false); }


        PauseGame(false);

        StarAmount_Anim.SetBool("show", true);
        RefreshStarAmount();

        void RefreshStarAmount()
        {
            string value = PlayerPrefs.GetInt("Stars").ToString();
            starAmount_Text[0].text = value;
            starAmount_Text[1].text = value;
        }
    }
    public void ShowLevelList()
    {
        UIAnimator.SetBool("showPause", false);
        UIAnimator.SetBool("showMainMenu", false);
        UIAnimator.SetBool("showLevelList", true);
    }

    public void PlayGame(bool playLastGame = false)
    {
        UIAnimator.SetBool("showPause", false);
        UIAnimator.SetBool("showMainMenu", false);
        UIAnimator.SetBool("showLevelList", false);

        if (playLastGame)
        {
            PlayerPrefs.SetInt("levelNr", PlayerPrefs.GetInt("LevelToPlay"));
        }

        MainMenuIsOpen = false;

        isPaused = false;

        StarAmount_Anim.SetBool("show", false);

        StartCoroutine(wait());
        IEnumerator wait()
        {
            yield return new WaitForSeconds(0.25f);
            gameSystem.startNewLevel(false);

            //Show level UI.
            ShowLevelUI(true);
        }
    }

    public void RestartGame()
    {
        ShowLevelFailed(false);

        gameSystem.startNewLevel(false);
    }
    public void ChangePause()
    {
        isPaused = !isPaused;
        PauseGame(isPaused);
    }
    public void PauseGame(bool value)
    {
        BMenu.SetBool("show", value);
        isPaused = value;
        UIAnimator.SetBool("showPause", value);
    }

    public void LoadNewLevel(int levelNr, int movementLimit, int movementBackLimit, bool nextLevelIsLocked)
    {
        SetLevelNr();
        SetMovementLimit(movementLimit);
        SetMovementBackLimit(movementBackLimit);

        ShowLevelFailed(false);

        RefreshHintButton();

        PauseGame(false);

        this.levelNr = levelNr;
        this.nextLevelIsLocked = nextLevelIsLocked;

        if (nextLevelIsLocked)
        {
            BSkip[0].SetActive(true);
            BSkip[1].SetActive(false);
            LevelFailed_BSkip[0].SetActive(true);
            LevelFailed_BSkip[1].SetActive(false);
        }
        else
        {
            BSkip[0].SetActive(false);
            BSkip[1].SetActive(true);
            LevelFailed_BSkip[0].SetActive(false);
            LevelFailed_BSkip[1].SetActive(true);
        }

        void SetLevelNr()
        {
            LevelNr_Text[0].text = "lvl " + levelNr;
            LevelNr_Text[1].text = "lvl " + levelNr;
        }
    }

    public void MoveBack()
    {
        if (lvl.gameSystem.movementBackLimit > 0)
        {
            if (lvl.player.currentMove > 0)
            {
                lvl.player.MovePlayerBack();
            }
        }
        else
        {
            gameSystem.ad.ShowRewardedAd(2);
        }
    }

    public void ShowLevelComplete(int levelNr)
    {
        LevelComplete_Anim.Play("ShowLevelCompleted");
        LevelComplete_TLevelNr.text = "Level " + levelNr;

        gameSystem.audioObject[2].Play();

        ShowConfetti();
        void ShowConfetti()
        {
            confettiParticle[0].Play();
            confettiParticle[1].Play();
        }
    }
    public void ShowLevelFailed(bool value)
    {
        LevelFailed_Anim.SetBool("showLevelFailed", value);

        if (!value) { return; }

        gameSystem.audioObject[3].Play();

        int time = 3;

        LevelFailed_BRestart.interactable = false;
        LevelFailed_RestartIcon.SetActive(false);
        LevelFailed_Time.text = time.ToString();

        StartCoroutine(wait());
        IEnumerator wait()
        {
            while (time > 0)
            {
                yield return new WaitForSeconds(1f);
                time--;

                LevelFailed_Time.text = time.ToString();
                if (time == 0)
                {
                    LevelFailed_Time.text = null;
                    LevelFailed_BRestart.interactable = true;
                    LevelFailed_RestartIcon.SetActive(true);
                }

            }
        }
    }
    public void ShowLevelUI(bool value)
    {
        LevelUI_Anim.SetBool("show", value);
        RefreshHintButton();
    }
    public void ShowSettings(bool value)
    {
        PSettings_Anim.SetBool("showPanel", value);

        audioSlider.value = PlayerPrefs.GetFloat("Settings-Volume");
        musicSlider.value = PlayerPrefs.GetFloat("Settings-Volume-Music", 0.2f);

        BGPAutoLogin.sprite = SGPAutoLogin[PlayerPrefs.GetInt("GooglePlayAutomaticLogin")];
    }
    public void ShowHint(bool value)
    {
        PHint_Anim.SetBool("show", value);
    }

    public void ShowHint()
    {
        if (lvl.Hint) { return; }
        gameSystem.ad.ShowRewardedAd(3);
    }
    public void SkipLevel()
    {
        if (nextLevelIsLocked)
        {
            PlayerPrefs.SetInt("unlockLevelNr", (levelNr+1));
            PlayerPrefs.Save();
            gameSystem.ad.ShowRewardedAd(1);
        }
        else
        {
            PlayerPrefs.SetInt("levelNr", (levelNr+1));
            PlayerPrefs.Save();
            gameSystem.startNewLevel(false);
        }
    }

    public void ShowLeaderboard()
    {
        CheckInternetConnection();
        GooglePlayServices.ShowLeaderboards();
    }
    public void ShowAchievements()
    {
        CheckInternetConnection();
        GooglePlayServices.ShowAchievements();
    }

    public void SetMovementLimit(int newValue)
    {
        MovementLimit_Text[0].text = newValue.ToString();
        MovementLimit_Text[1].text = newValue.ToString();
    }
    public void SetMovementBackLimit(int newValue)
    {
        MovementBackLimit_Text[0].text = newValue.ToString();
        MovementBackLimit_Text[1].text = newValue.ToString();

        if (newValue >= 1)
        {
            BBack_AdInformation.SetActive(false);
            BBack_LimitInformation.SetActive(true);
        }
        else
        {
            BBack_AdInformation.SetActive(true);
            BBack_LimitInformation.SetActive(false);
        }
    }

    public void RefreshLevelButton(int levelNr)
    {
        if (BLevel.Length >= levelNr)
        {
            BLevel[levelNr - 1].RefreshButton();
        }
    }
    public void RefreshHintButton()
    {
        int value = PlayerPrefs.GetInt("BHintValue");

        switch (value)
        {
            case 0:
                BHint[0].SetActive(true);
                BHint[1].SetActive(false);
                BHint[2].SetActive(false);
                break;
            case 1:
                BHint[0].SetActive(false);
                BHint[1].SetActive(true);
                BHint[2].SetActive(false);
                break;
            case 2:
                BHint[0].SetActive(false);
                BHint[1].SetActive(false);
                BHint[2].SetActive(true);
                break;
        }
    }

    public void SetAudioVolume(float vol)
    {
        PlayerPrefs.SetFloat("Settings-Volume", vol);
        PlayerPrefs.Save();

        SetAudio();
    }

    public void SetMusicVolume(float vol)
    {
        PlayerPrefs.SetFloat("Settings-Volume-Music", vol);
        PlayerPrefs.Save();

        SetMusic();
    }

    public void ChangeGPAutoLogin()
    {
        if (PlayerPrefs.GetInt("GooglePlayAutomaticLogin") == 1)
        {
            PlayerPrefs.SetInt("GooglePlayAutomaticLogin", 0);
        }
        else
        {
            PlayerPrefs.SetInt("GooglePlayAutomaticLogin", 1);
        }

        BGPAutoLogin.sprite = SGPAutoLogin[PlayerPrefs.GetInt("GooglePlayAutomaticLogin")];

        PlayerPrefs.Save();
    }

    private void SetMusic()
    {
        gameSystem.musicObject.volume = PlayerPrefs.GetFloat("Settings-Volume-Music", 0.2f);
    }

    private void SetAudio()
    {
        for(int i = 0; i < gameSystem.audioObject.Length; i++)
        {
            gameSystem.audioObject[i].volume = PlayerPrefs.GetFloat("Settings-Volume");
        }
    }

    public void RateGame()
    {
        CheckInternetConnection();

        Application.OpenURL("market://details?id=com.Coresaken.FilltheFields");
    }
    public void CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowErrorInfo("Error! Check your Internet connection!");
        }
    }


    public void ShowErrorInfo(string error)
    {
        TErrorInfo.text = error;
        ErrorObj.Play("ShowError");
    }

    public void RefreshHintDirection(int directionID)
    {
        for(int i = 0; i < 4; i++)
        {
            Arrows[i].SetActive(false);
        }

        Arrows[directionID - 1].SetActive(true);
    }
}