using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    private static SoundSystem instance;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip confettiSound;
    [SerializeField] private AudioClip playerMovementSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string soundName = "buttonClick")
    {
        switch(soundName)
        {
            case "buttonClick":
                audioSource.PlayOneShot(buttonClickSound); break;
            case "playerMovement":
                audioSource.PlayOneShot(playerMovementSound); break;
            case "launchConfetti":
                audioSource.PlayOneShot(confettiSound); break;
        }
    }

    public void ChangeSoundVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public static SoundSystem GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<SoundSystem>();
            if (instance == null)
            {
                GameObject go = new GameObject("SoundSystem");
                instance = go.AddComponent<SoundSystem>();
                DontDestroyOnLoad(go);
            }
        }
        return instance;
    }
}
