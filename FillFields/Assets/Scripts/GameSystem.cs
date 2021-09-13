using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public UISystem ui;
    public AdSystem ad;

    private XmlDocument doc = new XmlDocument();

    public GameObject LevelManagerPrefab;
    public Transform Levels;

    public AudioSource[] audioObject;
    public AudioSource musicObject;

    [HideInInspector]public int movementBackLimit;

    private void Awake()
    {
        LoadDefaultValues();
        LoadXmlDocument();
        movementBackLimit = PlayerPrefs.GetInt("BackMovementLimit");
    }

    public void startNewLevel(bool hint)
    {
        if (ui.MainMenuIsOpen) { return; }

        ad.ChangeInterstitialAdValue(1);
        ad.CheckInterstitalAdValue();

        //Destroy all existing levels.
        for (int i = 0; i < Levels.childCount; i++)
        {
            Destroy(Levels.GetChild(i).gameObject);
        }

        int levelNr = PlayerPrefs.GetInt("levelNr");

        GameObject lvlObj = Instantiate(LevelManagerPrefab);
        lvlObj.transform.SetParent(Levels);

        lvlObj.GetComponent<LevelManager>().LoadNewLevel(levelNr, doc, this, ui, hint);
    }
    public void ChangeMovementBackLimit(int value, bool set = false)
    {
        if (set) { movementBackLimit = value; }
        else { movementBackLimit += value; }
        
        PlayerPrefs.SetInt("BackMovementLimit", movementBackLimit);
        PlayerPrefs.Save();

        ui.SetMovementBackLimit(movementBackLimit);
    }

    public void UnlockLevel()
    {
        int unlockLevelNr = PlayerPrefs.GetInt("unlockLevelNr");

        PlayerPrefs.SetInt("levelNr", unlockLevelNr);
        PlayerPrefs.SetInt("Level1." + unlockLevelNr, 1);
        PlayerPrefs.Save();
        ui.RefreshLevelButton(unlockLevelNr);
    }

    public void LoadXmlDocument()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            TextAsset levelAsset = (TextAsset)Resources.Load("Levels");
            doc.LoadXml(levelAsset.text);
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            string filePath = "Assets/Resources/Levels.xml";
            doc.Load(filePath);
        }
    }
    private void LoadDefaultValues()
    {
        if (PlayerPrefs.GetInt("DefaultValuesV2.0") == 0)
        {
            Debug.Log("LoadDefaultValues");
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