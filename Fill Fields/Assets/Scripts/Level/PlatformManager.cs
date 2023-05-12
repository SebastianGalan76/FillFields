using System.Collections;
using UnityEngine;
using System.Xml;

public class PlatformManager : MonoBehaviour {
    public Platform[] platforms;

    [SerializeField] private GameObject PlatformPrefab;
    [SerializeField] private GameObject BlockPrefab;

    private LevelManager lvl;

    private int platformAmount;

    private void Awake() {
        lvl = GetComponent<LevelManager>();
    }

    public void LoadAllPlatforms(XmlNode lvlNode) {
        platforms = new Platform[324];
        XmlNode platformNode = lvlNode.SelectSingleNode("platform");

        GameObject platform, block;
        int platformAmount = 0;
        int positionIndex;

        for(int y = 0;y < 18;y++) {
            for(int x = 0;x < 18;x++) {
                positionIndex = y * 18 + x;

                int.TryParse(platformNode.InnerText[positionIndex].ToString(), out int value);

                if(value == 1) {
                    platform = Instantiate(PlatformPrefab);
                    platform.transform.localPosition = new Vector3(x, (-y + 2), 1);
                    platform.transform.SetParent(transform, false);

                    platforms[positionIndex] = platform.GetComponent<Platform>();
                    platforms[positionIndex].platform = platform.GetComponent<Animator>();

                    block = Instantiate(BlockPrefab);
                    block.transform.localPosition = new Vector3(x, -y, 1);
                    block.transform.SetParent(transform, false);
                    platforms[positionIndex].block = block.GetComponent<Animator>();

                    platforms[positionIndex].positionIndex = positionIndex;
                    platforms[positionIndex].status = (PlatformStatus)value;

                    platformAmount++;
                }
            }
        }

        this.platformAmount = platformAmount;
    }
    public void ShowAllPlatforms() {
        int showPlatformNumber = 0;
        float speed = GetSpeed();

        StartCoroutine(wait());
        IEnumerator wait() {
            yield return new WaitForSeconds(speed);

            while(showPlatformNumber < platforms.Length) {
                if(platforms[showPlatformNumber] != null) {
                    platforms[showPlatformNumber].ShowPlatform();

                    if(lvl.player.Position == showPlatformNumber) {
                        lvl.player.GetComponent<Animator>().Play("showPlayer");
                    }
                }

                showPlatformNumber++;
            }

            StartCoroutine(wait2());
            IEnumerator wait2() {
                yield return new WaitForSeconds(0.1f);

                lvl.PlayerCanMove = true;
            }
        }
    }
    public void HideAllPlatforms(bool loadNewLevel = true) {
        int hidePlatformNumber = 0;
        float speed = GetSpeed();

        int[] platformIndex;

        randomPlatformIndex();

        StartCoroutine(wait());
        IEnumerator wait() {
            yield return new WaitForSeconds(speed);

            while(hidePlatformNumber < platforms.Length) {
                if(platforms[hidePlatformNumber] != null) {
                    platforms[hidePlatformNumber].HidePlatform();
                    platforms[hidePlatformNumber].HideFillBlock();

                    if(lvl.player.Position == hidePlatformNumber) {
                        lvl.player.GetComponent<Animator>().Play("hidePlayer");
                    }
                }

                hidePlatformNumber++;
            }

            StartCoroutine(wait2());
            IEnumerator wait2() {
                yield return new WaitForSeconds(0.5f);
                if(loadNewLevel) {
                    lvl.StartNewLevel(false);
                }
            }
        }

        void randomPlatformIndex() {
            platformIndex = new int[platformAmount];

            int randomValue;
            for(int i = 0;i < platformIndex.Length;i++) {
            RandomAgain:
                randomValue = Random.Range(1, platformIndex.Length + 1);

                if(!checkNumber(randomValue)) { goto RandomAgain; }

                platformIndex[i] = randomValue;
            }

            decrementValue();

            void decrementValue() {
                for(int i = 0;i < platformIndex.Length;i++) {
                    platformIndex[i] = platformIndex[i] - 1;
                }
            }
            bool checkNumber(int number) {
                for(int i = 0;i < platformIndex.Length;i++) {
                    if(platformIndex[i] == number) {
                        return false;
                    }
                }
                return true;
            }
        }
    }

    public void ShowFillBlock(int platformIndex, bool show = true) {
        switch(show) {
            case true:
                platforms[platformIndex].ShowFillBlock();
                break;
            case false:
                platforms[platformIndex].HideFillBlock();
                break;
        }
    }

    private float GetSpeed() {
        return (35 / platformAmount) * Time.deltaTime;
    }
}
