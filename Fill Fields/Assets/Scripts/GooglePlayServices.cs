using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;

public class GooglePlayServices : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.GetInt("GooglePlayAutomaticLogin") == 1)
        {
            AuthenticateUser();
        }

        DontDestroyOnLoad(gameObject);
    }

    public static void AuthenticateUser(int value = 1)
    {
        try
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
            Social.localUser.Authenticate((bool success) => {
                if (success)
                {
                    switch (value)
                    {
                        case 2:
                            ShowLeaderboards();
                            break;
                        case 3:
                            ShowAchievements();
                            break;
                    }
                }
            });
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
        }
    }

    public static void ShowLeaderboards()
    {
        if (!Social.localUser.authenticated)
        {
            AuthenticateUser(2);
            return;
        }

        PostToLeaderboard();
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_completed_levels);
    }
    public static void ShowAchievements()
    {
        if (!Social.localUser.authenticated)
        {
            AuthenticateUser(3);
            return;
        }

        PostToAchievements();
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    public static void PostToLeaderboard()
    {
        if (!Social.localUser.authenticated) { return; }

        Social.ReportScore(PlayerPrefs.GetInt("Stars"), GPGSIds.leaderboard_completed_levels, (bool success) =>
        {
            if (success)
            {
                Debug.Log("SUCCESS");
            }
            else
            {
                Debug.Log(":<<<");
            }
        });
    }
    public static void PostToAchievements()
    {
        if (!Social.localUser.authenticated) { return; }

        if (PlayerPrefs.GetInt("Stars") >= 10)
        {
            Social.ReportProgress(GPGSIds.achievement_newbies, 100f, success => { });
        }
        if (PlayerPrefs.GetInt("Stars") >= 25)
        {
            Social.ReportProgress(GPGSIds.achievement_player, 100f, success => { });
        }
        if (PlayerPrefs.GetInt("Stars") >= 50)
        {
            Social.ReportProgress(GPGSIds.achievement_professional, 100f, success => { });
        }
        if (PlayerPrefs.GetInt("Stars") >= 100)
        {
            Social.ReportProgress(GPGSIds.achievement_hacker, 100f, success => { });
        }
    }
}
