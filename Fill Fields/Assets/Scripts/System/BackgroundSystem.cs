using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSystem : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    private static BackgroundSystem instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            ChangeMusicVolume(Settings.GetInstance().MusicVolume);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeMusicVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public static BackgroundSystem GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<BackgroundSystem>();

            if (instance == null)
            {
                GameObject gameObject = new GameObject("BackgroundSystem");
                instance = gameObject.AddComponent<BackgroundSystem>();
                DontDestroyOnLoad(gameObject);
            }
        }

        return instance;
    }
}
