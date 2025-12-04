//using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public SaveData(int health, int mental, int coin, string eventNumber, string paintName)
    {
        _health = health;
        _mental = mental;
        _coin = coin;
        _eventNumber = eventNumber;
        _paintName = paintName;
    }

    public int _health;
    public int _mental;
    public int _coin;
    public string _eventNumber;
    public string _paintName;
}

public static class SaveSystem
{
    private static string SavePath => Application.persistentDataPath + "/saves/";

    public static void Save(SaveData saveData)
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        string saveJson = JsonUtility.ToJson(saveData);

        string saveFilePath;

        // 각 플랫폼별 저장 경로 지정
        #if UNITY_EDITOR
            saveFilePath = Path.Combine(Application.dataPath, "UserData.json");

        #elif UNITY_STANDALONE_WIN
            saveFilePath = Path.Combine(Application.dataPath, "UserData.json");

        #else
            // 나머지(모바일 포함) 모든 플랫폼은 공통 처리
            saveFilePath = Path.Combine(Application.persistentDataPath, "UserData.json");
        #endif

        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
    }

    public static SaveData Load()
    {
        string saveFilePath;

        // 각 플랫폼별 저장 경로 지정
        #if UNITY_EDITOR
            saveFilePath = Path.Combine(Application.dataPath, "UserData.json");

        #elif UNITY_STANDALONE_WIN
            saveFilePath = Path.Combine(Application.dataPath, "UserData.json");

        #else
            // 나머지(모바일 포함) 모든 플랫폼은 공통 처리
            saveFilePath = Path.Combine(Application.persistentDataPath, "UserData.json");

        #endif
        if (!File.Exists(saveFilePath))
        {
            //Debug.LogError("No such saveFile exists");
            return null;
        }

        string saveFile = File.ReadAllText(saveFilePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(saveFile);
        return saveData;
    }
}
public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager instance;
    public static SaveLoadManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No SaveLoadManager Instance");
            }
            return instance;
        }
    }

    public int health = 3;
    public int mental = 3;
    public int coin = 1;
    public string eventNumber;
    public string paintName;

    SaveData saveData;
    SaveData loadData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // 저장데이터 만들기
        loadData = SaveSystem.Load();
        if(loadData == null) SaveGameData();
    }
    void Start()
    {
        LoadGameData();
    }

    public void LoadGameData()
    {
        loadData = SaveSystem.Load();
        if(loadData == null)
        {
            GameManager.Instance.IsCanContinue = false;
            Debug.LogError("No LoadData!");
            return;
        }

        GameManager.Instance.IsCanContinue = true;

        health = loadData._health;
        mental = loadData._mental;
        coin = loadData._coin;
        eventNumber = loadData._eventNumber;
        paintName = loadData._paintName;
    }
    public void SaveGameData()
    {
        saveData = new SaveData(health, mental, coin, eventNumber, paintName);
        SaveSystem.Save(saveData);
    }

    public void CleanUp()
    {
        health = 3;
        mental = 3;
        coin = 3;
        eventNumber = "1_1";
        
        saveData = new SaveData(health, mental, coin, eventNumber, paintName);
        SaveSystem.Save(saveData);
    }
}
