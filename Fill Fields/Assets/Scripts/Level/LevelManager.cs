using System.Collections;
using UnityEngine;
using System.Xml;
using System;

public class LevelManager : MonoBehaviour {
    public UILevel ui;

    [HideInInspector] public Player player;
    [HideInInspector] public PlatformManager platform;
    [HideInInspector] public LevelStatus levelStatus, nextLevelStatus;

    private int levelNr;
    private int movementLimit;
    private bool playerCanMove;
    private int movementBackLimit;

    private XmlDocument doc;

    public void Awake() {
        AdSystem.GetInstance().setLevelManager(this);

        platform = GetComponent<PlatformManager>();

        LoadXmlDocument();
        StartNewLevel(false);
    }

    public void StartNewLevel(bool loadHint) {
        ClearOldLevel();

        LevelNr = PlayerPrefs.GetInt("levelNr");

        levelStatus = (LevelStatus)PlayerPrefs.GetInt("Level1." + LevelNr);
        nextLevelStatus = (LevelStatus)PlayerPrefs.GetInt("Level1." + LevelNr + 1);

        MovementBackLimit = PlayerPrefs.GetInt("BackMovementLimit");

        XmlNode lvlNode = doc.SelectSingleNode("//level[@id='" + LevelNr + "']");

        LoadPlatforms();
        LoadPlayer();
        LoadCamera();
        LoadMovementLimit();

        ui.StartNewLevel(LevelNr, MovementLimit, MovementBackLimit);
        platform.ShowAllPlatforms();

        AdSystem.GetInstance().ChangeInterstitialAdValue(1);
        AdSystem.GetInstance().CheckInterstitalAdValue();

        void ClearOldLevel() {
            for(int i = 0;i < transform.childCount;i++) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        void LoadPlatforms() {
            platform.LoadAllPlatforms(lvlNode);
        }
        void LoadPlayer() {
            XmlNode playerNode = lvlNode.SelectSingleNode("playerBlock");

            int.TryParse(playerNode.ChildNodes[0].InnerText, out int position);
            int.TryParse(playerNode.ChildNodes[1].InnerText, out int posX);
            int.TryParse(playerNode.ChildNodes[2].InnerText, out int posY);

            GameObject playerObj = Instantiate(Resources.Load<GameObject>("Player"));

            playerObj.transform.name = "Player";
            playerObj.transform.position = new Vector2(posX, posY + 2);
            playerObj.transform.SetParent(transform, false);

            player = playerObj.GetComponent<Player>();
            player.Initialize(this, ui, position);

            LoadHint();

            void LoadHint() {
                player.hint = ScriptableObject.CreateInstance<Hint>();
                player.hint.Initialize(lvlNode, loadHint);

                if(loadHint) {
                    ui.hint.ShowHints();
                    ui.hint.ChangeDirection((int)player.hint.GetDirection(0));
                } else {
                    ui.hint.HideHints();
                    if(PlayerPrefs.GetInt("BHintValue") == 0) {
                        PlayerPrefs.SetInt("BHintValue", 2);
                        PlayerPrefs.Save();
                    }
                }
            }
        }
        void LoadCamera() {
            XmlNode cameraNode = lvlNode.SelectSingleNode("camera");

            string posX = cameraNode.SelectSingleNode("posX").InnerText;
            string posY = cameraNode.SelectSingleNode("posY").InnerText;
            string size = cameraNode.SelectSingleNode("size").InnerText;

            float.TryParse(posX, out float posXf);
            float.TryParse(posY, out float posYf);
            float.TryParse(size, out float sizef);

            Camera.main.transform.localPosition = new Vector3(posXf, posYf, -10f);
            Camera.main.orthographicSize = sizef;
        }
        void LoadMovementLimit() {
            int.TryParse(lvlNode.SelectSingleNode("movementLimit").InnerText, out int value);

            MovementLimit = value;
            ui.RefreshMovementLimit(MovementLimit);
        }
    }

    public bool IsFinished() {
        for(int i = 0;i < platform.platforms.Length;i++) {
            try {
                if(platform.platforms[i].status == PlatformStatus.UNLOCKED) {
                    PlayerCanMove = true;
                    return false;
                }
            } catch(NullReferenceException) {

            }
        }

        FinishLevel();
        return true;
    }

    public void FinishLevel() {
        if(PlayerPrefs.GetInt("Level1." + LevelNr) != (int)LevelStatus.FINISHED) {
            PlayerPrefs.SetInt("Level1." + LevelNr, (int)LevelStatus.FINISHED);
            PlayerPrefs.SetInt("Stars", PlayerPrefs.GetInt("Stars") + 1);

            GooglePlayServices.PostToAchievements();
            GooglePlayServices.PostToLeaderboard();

            ui.RefreshLevelNr(LevelNr);

            if(nextLevelStatus == LevelStatus.LOCKED) {
                PlayerPrefs.SetInt("Level1." + (LevelNr + 1), (int)LevelStatus.UNLOCKED);
                PlayerPrefs.SetInt("LevelToPlay", LevelNr + 1);

                ui.RefreshLevelNr(LevelNr + 1);
            }
        }

        int BHintValue = PlayerPrefs.GetInt("BHintValue");
        if(BHintValue == 0) {
            PlayerPrefs.SetInt("BHintValue", 2);
        }

        PlayerPrefs.SetInt("levelNr", LevelNr + 1);
        PlayerPrefs.Save();

        ui.levelComplete.ShowPanel(LevelNr);

        StartCoroutine(wait());
        IEnumerator wait() {
            yield return new WaitForSeconds(1.5f);

            platform.HideAllPlatforms();
        }
    }

    public void ChangeMovementLimit(int value) {
        MovementLimit += value;

        ui.RefreshMovementLimit(MovementLimit);
    }
    public void ChangeMovementBackLimit(int value, bool set = false) {
        if(set) { MovementBackLimit = value; } else { MovementBackLimit += value; }

        PlayerPrefs.SetInt("BackMovementLimit", MovementBackLimit);
        PlayerPrefs.Save();

        ui.RefreshMovementBackLimit(MovementBackLimit);
    }
    public void UnlockNextLevel() {
        int unlockLevelNr = PlayerPrefs.GetInt("unlockLevelNr");

        PlayerPrefs.SetInt("levelNr", unlockLevelNr);
        PlayerPrefs.SetInt("Level1." + unlockLevelNr, 1);
        PlayerPrefs.Save();

        StartNewLevel(false);
    }

    public void LoadXmlDocument() {
        doc = new XmlDocument();

        if(Application.platform == RuntimePlatform.Android) {
            TextAsset levelAsset = (TextAsset)Resources.Load("Levels");
            doc.LoadXml(levelAsset.text);
        } else if(Application.platform == RuntimePlatform.WindowsEditor) {
            string filePath = "Assets/Resources/Levels.xml";
            doc.Load(filePath);
        }
    }

    public bool PlayerCanMove {
        set { playerCanMove = value; }
        get {
            if(MovementLimit > 0 && playerCanMove) {
                return true;
            }
            return false;
        }
    }
    public int MovementLimit {
        set { movementLimit = value; }
        get { return movementLimit; }
    }
    public int MovementBackLimit {
        set { movementBackLimit = value; }
        get { return movementBackLimit; }
    }
    public int LevelNr {
        set {
            int maxLevel = 100;

            if(value > maxLevel) {
                value = maxLevel;
            }
            levelNr = value;
        }
        get { return levelNr; }
    }
}
