using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void LoadYandex();

    [DllImport("__Internal")]
    private static extern void SaveYandex(string strJson);

    [DllImport("__Internal")]
    private static extern void SetToLeaderboard(int value);


    public PlayerInfo currentPlayer = new PlayerInfo();
    [SerializeField] private MainUIControl mm_control;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Invoke("LoadGame", 0.08f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    //public void LoadLevel()
    //{
    //    SceneManager.LoadScene("LevelScene");
    //}

    void LoadGame()
    {
#if UNITY_WEBGL
        LoadYandex();
#endif
#if UNITY_EDITOR
        string filePath = Application.persistentDataPath + "/MySaveData.dat";
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileInfo fileInfo = new FileInfo(filePath);
            Debug.Log($"filePath = <{filePath}>    size = {fileInfo.Length}");
            FileStream file = File.Open(filePath, FileMode.Open);

            if (fileInfo.Length > 0)
            {
                SaveData data = (SaveData)bf.Deserialize(file);
                //Debug.Log(data.ToString());
                UpdateLoadingData(data);
            }
            else
            {
                GameManager.Instance.currentPlayer = PlayerInfo.FirstGame();
                if (mm_control != null)
                {
                    mm_control.ViewRecord();
                }
            }
            file.Close();
        }
        else
        {
            Debug.Log("There is no save data!");
            GameManager.Instance.currentPlayer = PlayerInfo.FirstGame();
            if (mm_control != null)
            {
                mm_control.ViewRecord();
                //mm_control.OnPoleSelect(GameManager.Instance.currentPlayer.currentPole);
                //mm_control.OnModeSelect(GameManager.Instance.currentPlayer.currentMode);
                //mm_control.ViewScore();
                //mm_control.UpdateAudioSource();
            }
        }
#endif
    }

    public void UpdateLoadingData(SaveData data)
    {
        GameManager.Instance.currentPlayer.isLoaded = true;

        GameManager.Instance.currentPlayer.maxLevel = data.level;
        GameManager.Instance.currentPlayer.totalScore = data.score;
        GameManager.Instance.currentPlayer.totalGold = data.gold;
        GameManager.Instance.currentPlayer.nameOldScene = data.scene;
        GameManager.Instance.currentPlayer.CsvToPosAndRot(data.posAndRot);
        //GameManager.Instance.currentPlayer.inventory = new Inventory(data.csvInventory);
        if (data.csvGarage == "")
        {
            //PlayersGarage.Instance.AddCar(4);
        }
        else
        {
            //PlayersGarage.Instance.CreateAllPassports(data.csvGarage);
            //PlayersGarage.Instance.SetGarageCsvString(data.csvGarage);
        }

        GameManager.Instance.currentPlayer.numberBonusDay = data.currentBonusDay;
        GameManager.Instance.currentPlayer.SetAcceptBonusTime(data.bonusTime);

        GameManager.Instance.currentPlayer.isHintView = data.isHints;
        GameManager.Instance.currentPlayer.isSoundFone = data.isFone;
        GameManager.Instance.currentPlayer.isSoundEffects = data.isEffects;
        GameManager.Instance.currentPlayer.volumeFone = data.volFone;
        GameManager.Instance.currentPlayer.volumeEffects = data.volEffects;


        //Debug.Log("Game data loaded! Score=" + GameManager.Instance.currentPlayer.totalScore.ToString() + "  Gold=" + GameManager.Instance.currentPlayer.totalGold.ToString());
        Debug.Log($"Game data loaded! Score={GameManager.Instance.currentPlayer.totalScore}  Gold={GameManager.Instance.currentPlayer.totalGold}  garageStr=<{data.csvGarage}>");
        //Debug.Log($"Game data loaded! Score={GameManager.Instance.currentPlayer.totalScore} Pos={GameManager.Instance.currentPlayer.oldPosition}");

        if (mm_control != null)
        {
            mm_control.ViewRecord();
            //mm_control.ViewScore();
            //mm_control.UpdateSound();
            //mm_control.ViewScore();
            //mm_control.UpdateAudioSource();
        }
    }

    public void SaveGame()
    {
        if (GameManager.Instance.currentPlayer.totalScore == 0) return;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
          + "/MySaveData.dat");
        SaveData data = new SaveData();

        data.score = GameManager.Instance.currentPlayer.totalScore;
        data.gold = GameManager.Instance.currentPlayer.totalGold;
        data.level = GameManager.Instance.currentPlayer.maxLevel;

        data.currentBonusDay = GameManager.Instance.currentPlayer.numberBonusDay;
        data.bonusTime = GameManager.Instance.currentPlayer.AcceptBonusTimeToString();

        data.scene = GameManager.Instance.currentPlayer.nameOldScene;
        data.posAndRot = GameManager.Instance.currentPlayer.PosAndRotToCsvString();

        //data.csvGarage = PlayersGarage.Instance.GarageToCsvString();
        //data.csvGarage = PlayersGarage.Instance.PassportsToCsvString();
        //data.csvInventory = GameManager.Instance.currentPlayer.inventory.ToCsvString();

        data.isHints = GameManager.Instance.currentPlayer.isHintView;
        data.isFone = GameManager.Instance.currentPlayer.isSoundFone;
        data.isEffects = GameManager.Instance.currentPlayer.isSoundEffects;
        data.volFone = GameManager.Instance.currentPlayer.volumeFone;
        data.volEffects = GameManager.Instance.currentPlayer.volumeEffects;

        //DateTime dt = DateTime.Now;
        //data.timeString = $"{dt.Year:0000}-{dt.Month:00}-{dt.Day:00}-{dt.Hour:00}";

#if UNITY_WEBGL
        string jsonStr = JsonUtility.ToJson(data);
        SaveYandex(jsonStr);
        SetToLeaderboard(GameManager.Instance.currentPlayer.totalScore);
#endif

        //PlayerInfo.Instance.Save(jsonStr);
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }
}

public class PlayerInfo
{
    public bool isLoaded = false;
    public int totalScore = 0;
    public int sessionScore = 0;
    public int currentScore = 0;
    public int totalGold = 0;
    public int sessionGold = 0;
    public int maxLevel = 1;
    public int currentLevel = 1;
    public int numBoxRemoval = 0;
    public int countSecondRemoval = 300;

    public int numberBonusDay = 0;
    public DateTime acceptBonusTime = DateTime.Now;

    public string nameOldScene = "";
    public Vector3 oldPosition = Vector3.zero;
    public Vector3 oldRotation = Vector3.zero;

    //public Inventory inventory;

    public int countSecond = 0;
    public int countLine = 0;
    public int countFigure = 0;
    public int countHardFigure = 0;
    public bool isSurprise = false;

    public int maxQwScore = 0;
    public int maxHexScore = 0;
    public int maxPrismScore = 0;

    public bool isHintView = true;
    public bool isSoundFone = true;
    public bool isSoundEffects = true;
    public int volumeFone = 50;
    public int volumeEffects = 100;

    public string playerName = "-------";
    public Texture photo = null;


    public PlayerInfo()
    {
        maxLevel = 1;
        currentLevel = 1;
        //inventory = new Inventory();
    }

    public static PlayerInfo FirstGame()
    {
        //PlayersGarage.Instance.AddCar(4);
        return new PlayerInfo();
    }

    public void LevelComplete()
    {
        //UpdateReward(currentLevel);
        currentLevel++;
        if (currentLevel > maxLevel)
        {
            maxLevel = currentLevel;
        }

        //sessionScore += currentScore;
        //if (sessionScore > totalScore) totalScore = sessionScore;
    }

    public void LevelExpAndManyUpdate()
    {
        totalScore += sessionScore;
        totalGold += sessionGold;
    }

    public void ClearCurrentParam()
    {
        currentScore = 0;
        currentLevel = 1;
        /*currentGold = 0;
        currentHP = maxHP;
        currentMagic = 0;
        currentFire = 0;
        currentToxin = 0;*/
    }

    public string PosAndRotToCsvString()
    {
        return $"{oldPosition.x:0.000}#{oldPosition.y:0.000}#{oldPosition.z:0.000}#{oldRotation.x:0.000}#{oldRotation.y:0.000}#{oldRotation.z:0.000}#";
    }

    public bool CsvToPosAndRot(string csv)
    {
        string[] ar = csv.Split('#', StringSplitOptions.RemoveEmptyEntries);
        if (ar.Length >= 6)
        {
            if (float.TryParse(ar[0], out float x) && float.TryParse(ar[1], out float y) && float.TryParse(ar[2], out float z))
            {
                oldPosition = new Vector3(x, y, z);
            }
            else return false;
            if (float.TryParse(ar[3], out float rx) && float.TryParse(ar[4], out float ry) && float.TryParse(ar[5], out float rz))
            {
                oldRotation = new Vector3(rx, ry, rz);
            }
            else return false;
            return true;
        }
        return false;
    }

    public void SetAcceptBonusTime(string strTime)
    {
        if (strTime != null && strTime != "")
        {
            Debug.Log($"strTime=<{strTime}>");
            string[] ar = strTime.Split(":", StringSplitOptions.RemoveEmptyEntries);
            if (ar.Length >= 2)
            {
                if ((int.TryParse(ar[0], out int day)) && (int.TryParse(ar[1], out int month)) && (int.TryParse(ar[2], out int year)))
                {
                    acceptBonusTime = new DateTime(year, month, day);
                    return;
                }
            }
        }
        acceptBonusTime = DateTime.Now;
    }

    public string AcceptBonusTimeToString()
    {
        return $"{acceptBonusTime.Day:00}:{acceptBonusTime.Month:00}:{acceptBonusTime.Year:0000}";
    }
}

[Serializable]
public class SaveData
{
    public int score;   //  exp
    public int gold;
    public int level = 1;
    public string scene = "";
    public string posAndRot = "";
    public string csvGarage = "";
    public int currentBonusDay = 0;
    public string bonusTime = "";
    public string csvInventory = "";

    public bool isFone;
    public bool isEffects;
    public bool isHints;
    public int volFone;
    public int volEffects;
    public override string ToString()
    {
        return "SaveData: score=" + score + " gold=" + gold + " level=" + level;
    }
}