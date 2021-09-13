using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class LevelManager : MonoBehaviour
{
    [HideInInspector]public GameSystem gameSystem;
    [HideInInspector]public Player player;
    private UISystem ui;

    int _levelNr;
    int _levelStatus;
    bool _hint;
    bool _playerCanMove;

    float speed; //Speed of showing and hiding the level depending of the number of platforms.

    public int movementLimit;
    public int[] hintDirection;

    [HideInInspector]public int[] platformValue = new int[324];

    private Animator[] platforms;
    private Animator[] blocks = new Animator[324];

    public GameObject PlatformPrefab, PlayerPrefab, BlockPrefab;

    public void LoadNewLevel(int levelNr, XmlDocument doc, GameSystem gameSystem, UISystem uiSystem, bool hint)
    {
        Hint = hint;
        LevelNr = levelNr;
        LevelStatus = PlayerPrefs.GetInt("Level1." + LevelNr);

        this.gameSystem = gameSystem;
        ui = uiSystem;

        ui.lvl = this;

        XmlNode lvl = doc.SelectSingleNode("//level[@id='" + LevelNr + "']");

        LoadPlatforms();
        LoadPlayer();
        LoadCamera();
        LoadMovementLimit();

        speed = GetSpeed();

        ui.ShowHint(Hint);
        if (Hint)
        {
            LoadHint();
        }
        else
        {
            if (PlayerPrefs.GetInt("BHintValue") == 0)
            {
                PlayerPrefs.SetInt("BHintValue", 2);
                PlayerPrefs.Save();
            }
        }

        bool nextLevelIsLocked = false;
        if (PlayerPrefs.GetInt("Level1." + (LevelNr + 1)) == 0)
        {
            nextLevelIsLocked = true;
        }

        ui.LoadNewLevel(LevelNr, movementLimit, PlayerPrefs.GetInt("BackMovementLimit"), nextLevelIsLocked);

        ShowPlatforms();

        void LoadPlatforms()
        {
            int platformNumber = 0;
            XmlNode platformNode = lvl.SelectSingleNode("platform");
            for (int i = 0; i < 324; i++)
            {
                int.TryParse(platformNode.InnerText[i].ToString(), out int value);
                if (value == 1) { platformNumber++; }

                platformValue[i] = value;
            }
            platforms = new Animator[platformNumber];

            platformNumber = 0;
            GameObject platform, block;
            for (int y = 0; y < 18; y++)
            {
                for (int x = 0; x < 18; x++)
                {
                    if (platformValue[y * 18 + x] == 1)
                    {
                        platform = Instantiate(PlatformPrefab);
                        platform.transform.localPosition = new Vector3(x, (-y + 2), 1);
                        platform.transform.SetParent(gameObject.transform);
                        platform.name = (18 * y + x).ToString();
                        platforms[platformNumber] = platform.GetComponent<Animator>();

                        block = Instantiate(BlockPrefab);
                        block.transform.localPosition = new Vector3(x, -y, 1);
                        block.transform.SetParent(gameObject.transform);
                        platform.name = (18 * y + x).ToString();
                        blocks[18 * y + x] = block.GetComponent<Animator>();

                        platformNumber++;
                    }
                }
            }
        }
        void LoadPlayer()
        {
            GameObject player;
            XmlNode playerNode = lvl.SelectSingleNode("playerBlock");

            int.TryParse(playerNode.ChildNodes[0].InnerText, out int posPlatform);
            int.TryParse(playerNode.ChildNodes[1].InnerText, out int posX);
            int.TryParse(playerNode.ChildNodes[2].InnerText, out int posY);

            player = Instantiate(PlayerPrefab);
            this.player = player.GetComponent<Player>();
            this.player.Initialize(posPlatform, posX, posY + 2, this, ui, gameSystem.audioObject[0]);
            this.player.transform.SetParent(gameObject.transform);
        }
        void LoadCamera()
        {
            XmlNode cameraNode = lvl.SelectSingleNode("camera");

            string posX = cameraNode.SelectSingleNode("posX").InnerText;
            string posY = cameraNode.SelectSingleNode("posY").InnerText;
            string size = cameraNode.SelectSingleNode("size").InnerText;

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                posX = posX.Replace(".", ",");
                posY = posY.Replace(".", ",");
                size = size.Replace(".", ",");
            }

            float.TryParse(posX, out float posXf);
            float.TryParse(posY, out float posYf);
            float.TryParse(size, out float sizef);

            Camera.main.transform.localPosition = new Vector3(posXf, posYf, -10f);
            Camera.main.orthographicSize = sizef;
        }
        void LoadMovementLimit()
        {
            XmlNode movementNode = lvl.SelectSingleNode("movementLimit");
            int.TryParse(movementNode.InnerText, out int value);

            movementLimit = value;
            ui.SetMovementLimit(movementLimit);
        }
        void LoadHint()
        {
            hintDirection = new int[movementLimit];
            XmlNode hintNode = lvl.SelectSingleNode("hint");
            for (int i = 0; i < movementLimit; i++)
            {
                int.TryParse(hintNode.InnerText[i].ToString(), out int value);

                hintDirection[i] = value;
            }

            ui.RefreshHintDirection(GetHintDirectionID(0));
        }
        float GetSpeed()
        {
            return (35 / platforms.Length) * Time.deltaTime;
        }
    }

    public void ShowPlatforms()
    {
        int showPlatformNumber = platforms.Length - 1;

        StartCoroutine(wait());
        IEnumerator wait()
        {
            while (showPlatformNumber >= 0)
            {
                yield return new WaitForSeconds(speed);

                platforms[showPlatformNumber].SetBool("showPlatform", true);
                if (player.PosPlatform.ToString() == platforms[showPlatformNumber].name)
                {
                    player.GetComponent<Animator>().Play("showPlayer");
                }

                showPlatformNumber--;
            }

            StartCoroutine(wait2());
            IEnumerator wait2()
            {
                yield return new WaitForSeconds(0.1f);

                PlayerCanMove = true;
            }
        }
    }
    public void HidePlatforms(bool loadNewLevel = true)
    {
        int hidePlatformNumber = platforms.Length - 1;
        int[] platformIndex;

        randomPlatformIndex();

        StartCoroutine(wait());
        IEnumerator wait()
        {
            while (hidePlatformNumber >= 0)
            {
                yield return new WaitForSeconds(speed);

                platforms[platformIndex[hidePlatformNumber]].SetBool("showPlatform", false);

                int.TryParse(platforms[platformIndex[hidePlatformNumber]].name, out int blockPos);
                blocks[blockPos].SetBool("showBlock", false);

                if (player.PosPlatform.ToString() == platforms[platformIndex[hidePlatformNumber]].name)
                {
                    player.GetComponent<Animator>().Play("hidePlayer");
                }

                hidePlatformNumber--;

                if (hidePlatformNumber == 0)
                {
                    StartCoroutine(wait2());

                    IEnumerator wait2()
                    {
                        yield return new WaitForSeconds(1f);
                        if (loadNewLevel)
                        {
                            gameSystem.startNewLevel(false);
                        }

                        Destroy(gameObject);
                    }
                }
            }
        }

        void randomPlatformIndex()
        {
            platformIndex = new int[platforms.Length];

            int randomValue;
            for (int i = 0; i < platformIndex.Length; i++)
            {
            RandomAgain:
                randomValue = Random.Range(1, platformIndex.Length + 1);

                if (!checkNumber(randomValue)) { goto RandomAgain; }

                platformIndex[i] = randomValue;
            }

            decrementValue();

            void decrementValue()
            {
                for (int i = 0; i < platformIndex.Length; i++)
                {
                    platformIndex[i] = platformIndex[i] - 1;
                }
            }
            bool checkNumber(int number)
            {
                for (int i = 0; i < platformIndex.Length; i++)
                {
                    if (platformIndex[i] == number)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
    public void ShowBlock(int posPlatform, bool show = true)
    {
        if (show)
        {
            platformValue[posPlatform] = 2;
        }
        else
        {
            platformValue[posPlatform] = 1;
        }

        blocks[posPlatform].GetComponent<Animator>().SetBool("showBlock", show);
    }

    public bool CheckFinish()
    {
        for (int i = 0; i < platformValue.Length; i++)
        {
            if (platformValue[i] == 1)
            {
                PlayerCanMove = true;
                return false;
            }
        }
        return true;
    }

    public void FinishLevel()
    {
        if (PlayerPrefs.GetInt("Level1." + LevelNr) != 2)
        {
            PlayerPrefs.SetInt("Level1." + LevelNr, 2);
            PlayerPrefs.SetInt("Stars", PlayerPrefs.GetInt("Stars") + 1);

            GooglePlayServices.PostToAchievements();
            GooglePlayServices.PostToLeaderboard();

            ui.RefreshLevelButton(LevelNr);

            if (PlayerPrefs.GetInt("Level1." + (LevelNr + 1)) == 0)
            {
                PlayerPrefs.SetInt("Level1." + (LevelNr + 1), 1);
                PlayerPrefs.SetInt("LevelToPlay", LevelNr + 1);

                ui.RefreshLevelButton(LevelNr + 1);
            }
        }

        int BHintValue = PlayerPrefs.GetInt("BHintValue");
        if (BHintValue == 0)
        {
            PlayerPrefs.SetInt("BHintValue", 2);
        }

        PlayerPrefs.SetInt("levelNr", LevelNr + 1);
        PlayerPrefs.Save();

        ui.ShowLevelComplete(LevelNr);

        StartCoroutine(wait());
        IEnumerator wait()
        {
            yield return new WaitForSeconds(1.5f);
            HidePlatforms();
        }
        
        ui.ShowHint(false);  
    }

    public void ChangeMovementLimit(int value)
    {
        movementLimit += value;

        ui.SetMovementLimit(movementLimit);
    }

    public int GetHintDirectionID(int currentMove)
    {
        if (hintDirection.Length > currentMove)
        {
            return hintDirection[currentMove];
        }
        return 0;
    }

    public bool PlayerCanMove
    {
        set { _playerCanMove = value; }
        get {
            if(movementLimit > 0 && _playerCanMove)
            {
                return true;
            }
            return false;
        }
    }
    public int LevelStatus
    {
        set { _levelStatus = value; }
        get { return _levelStatus; }
    }
    public int LevelNr
    {
        set
        {
            int maxLevel = 100;

            if (value > maxLevel)
            {
                value = maxLevel;
            }
            _levelNr = value;
        }
        get { return _levelNr; }
    }
    public bool Hint
    {
        set { _hint = value; }
        get { return _hint; }
    }
}
