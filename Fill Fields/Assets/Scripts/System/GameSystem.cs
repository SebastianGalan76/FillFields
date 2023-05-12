using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    private void Awake()
    {
        LoadDefaultValues();
    }

    private void LoadDefaultValues()
    {
        if (PlayerPrefs.GetInt("DefaultValuesV2.0") == 0)
        {
            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetInt("Stars", 0);
            PlayerPrefs.SetInt("BHintValue",1);
            PlayerPrefs.SetInt("levelNr", 1);
            PlayerPrefs.SetInt("LevelToPlay", 1);
            PlayerPrefs.SetInt("BackMovementLimit", 15);
            PlayerPrefs.SetInt("Level1.1", 1);
            PlayerPrefs.SetInt("DefaultValuesV2.0", 1);
            PlayerPrefs.SetFloat("Settings-Volume", 0.5f);
            PlayerPrefs.SetFloat("Settings-Volume-Music", 0.2f);
            PlayerPrefs.Save();
        }
    }
}