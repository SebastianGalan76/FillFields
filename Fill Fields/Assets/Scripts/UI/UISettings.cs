using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UISettings : MonoBehaviour
{
    [SerializeField] private UISystem ui;
    [SerializeField] private Slider audioSlider, musicSlider;

    [SerializeField] private Image autoLoginCurrentImage;
    [SerializeField] private Sprite[] autoLoginIcons;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAudioVolume(float vol)
    {
        Settings.GetInstance().AudioVolume = vol;
    }
    public void SetMusicVolume(float vol)
    {
        Settings.GetInstance().MusicVolume = vol;
    }

    public void ChangeGPAutoLogin()
    {
        bool autoLogin;
        if (PlayerPrefs.GetInt("GooglePlayAutomaticLogin") == 1)
        {
            PlayerPrefs.SetInt("GooglePlayAutomaticLogin", 0);

            autoLogin = false;
        }
        else
        {
            PlayerPrefs.SetInt("GooglePlayAutomaticLogin", 1);
            autoLogin = true;
        }

        autoLoginCurrentImage.sprite = autoLoginIcons[PlayerPrefs.GetInt("GooglePlayAutomaticLogin")];

        PlayerPrefs.Save();

        Settings.GetInstance().autoLogin = autoLogin;
    }

    public void ShowPanel()
    {
        if (ui != null)
        {
            ui.HideMainMenu();
        }

        animator.SetBool("showPanel", true);

        audioSlider.value = PlayerPrefs.GetFloat("Settings-Volume", 0.4f);
        musicSlider.value = PlayerPrefs.GetFloat("Settings-Volume-Music", 0.2f);

        autoLoginCurrentImage.sprite = autoLoginIcons[PlayerPrefs.GetInt("GooglePlayAutomaticLogin")];
    }

    public void HidePanel()
    {
        if (ui != null)
        {
            ui.ShowMainMenu();
        }

        animator.SetBool("showPanel", false);
    }
}
