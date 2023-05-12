using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class Settings : MonoBehaviour
{
    private static Settings instance;
    private BackgroundSystem backgroundSystem;


    private float musicVolume, audioVolume;
    //Auto login to google play account
    public bool autoLogin;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadSettings();

            backgroundSystem = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundSystem>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSettings()
    {
        audioVolume = PlayerPrefs.GetFloat("Settings-Volume", 0.4f);
        musicVolume = PlayerPrefs.GetFloat("Settings-Volume-Music", 0.4f);

        autoLogin = Convert.ToBoolean(PlayerPrefs.GetInt("GooglePlayAutomaticLogin"));

        SoundSystem.GetInstance().ChangeSoundVolume(audioVolume);
    }

    

    public static Settings GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<Settings>();

            if(instance = null)
            {
                GameObject settingsObj = new GameObject("Settings");
                instance = settingsObj.AddComponent<Settings>();

            }
        }

        return instance;
    }

    public float MusicVolume
    {
        get { return musicVolume; }
        set { 
            musicVolume = value;

            PlayerPrefs.SetFloat("Settings-Volume-Music", value);
            PlayerPrefs.Save();

            backgroundSystem.ChangeMusicVolume(value);
        }
    }

    public float AudioVolume
    {
        get { return audioVolume; }
        set
        {
            audioVolume = value;

            PlayerPrefs.SetFloat("Settings-Volume", value);
            PlayerPrefs.Save();

            SoundSystem.GetInstance().ChangeSoundVolume(value);
        }
    }
}
