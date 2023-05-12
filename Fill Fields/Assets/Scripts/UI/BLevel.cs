using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BLevel : MonoBehaviour
{
    private UISystem ui;
    private int levelNr;
    private int levelValue;

    public void ChooseLevel()
    {
        if (levelValue != 0)
        {
            PlayerPrefs.SetInt("levelNr", levelNr);
            PlayerPrefs.Save();

            ui.PlayGame();
            return;
        }

        UIError.GetInstance().Show("This level is locked. Complete the previous level to play this level.");
    }

    public void RefreshButton()
    {
        int.TryParse(gameObject.name, out levelNr);
        levelValue = PlayerPrefs.GetInt("Level1." + levelNr);

        if (levelValue == 0)
        {
            transform.Find("LevelNr").gameObject.SetActive(false);
            transform.Find("Background").GetComponent<Image>().color = new Color32(54, 12, 124, 125);
        }
        else {
            transform.Find("Background").GetComponent<Image>().color = new Color32(117, 61, 210, 104);
            transform.Find("LevelNr").gameObject.SetActive(true);

            if (levelValue == 2)
            {
                transform.Find("Star").gameObject.SetActive(true);
            }
        }
    }
    public void Initialize(UISystem ui)
    {
        this.ui = ui;
        GetComponent<Button>().onClick.AddListener(() => ChooseLevel());
    }
}
