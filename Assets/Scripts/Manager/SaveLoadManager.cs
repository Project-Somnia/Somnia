//using System;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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
    private const string KEY_PREF = "USER_AES_KEY";
    private const int KEY_SIZE = 32;
    private const int IV_SIZE = 16;  // AES IV 크기

    private static string SaveFilePath
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            return Path.Combine(Application.dataPath, "UserData.json");
#else
            return Path.Combine(Application.persistentDataPath, "UserData.json");
#endif
        }
    }

    // ================================
    //  유저별 AES KEY 가져오기
    // ================================
    private static byte[] GetOrCreateUserKey()
    {
        for (int i = 0; i < 2; i++)
        {
            if (PlayerPrefs.HasKey(KEY_PREF))
            {
                string wrappedKey = PlayerPrefs.GetString(KEY_PREF);
                byte[] key = UnwrapKey(wrappedKey);

                if (key != null)
                {
                    return key; // 성공
                }

                Debug.LogError("키 복호화 실패 - 새 키를 생성합니다");
                PlayerPrefs.DeleteKey(KEY_PREF);
                PlayerPrefs.Save();
                // 재귀 대신 반복문으로 재시도 (i가 1이면 다음 루프에서 새 키 생성)
                continue;
            }

            // 새 키 생성
            byte[] newKey = new byte[KEY_SIZE];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(newKey);
            }

            // 암호화해서 저장
            PlayerPrefs.SetString(KEY_PREF, WrapKey(newKey));
            PlayerPrefs.Save();

            Debug.Log("<color=yellow>새 AES 사용자 키 생성 완료!</color>");

            return newKey;
        }
        // 두 번 시도 후에도 실패하면 null 반환
        Debug.LogError("키 생성 및 복구 실패");
        return null;
    }

    // ================================
    //  JSON 저장
    // ================================
    public static void Save(SaveData saveData)
    {
        try
        {
            string json = JsonUtility.ToJson(saveData);
            byte[] key = GetOrCreateUserKey();

            if (key == null)
            {
                Debug.LogError("저장 실패: 키를 생성할 수 없습니다");
                return;
            }

            // 기존 파일 백업 (선택사항)
            if (File.Exists(SaveFilePath))
            {
                string backupPath = SaveFilePath + ".bak";
                File.Copy(SaveFilePath, backupPath, true);
                Debug.Log("백업 생성: " + backupPath);
            }

            // AES 암호화
            byte[] encryptedBytes = Encrypt(json, key);

            File.WriteAllBytes(SaveFilePath, encryptedBytes);
            Debug.Log("<color=green>Save Success (Encrypted): " + SaveFilePath + "</color>");
        }
        catch (Exception e)
        {
            Debug.LogError($"저장 중 오류 발생: {e.Message}\n{e.StackTrace}");
        }
    }

    // ================================
    //  JSON 불러오기
    // ================================
    public static SaveData Load()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("저장 파일이 없습니다");
            return null;
        }

        try
        {
            byte[] encryptedBytes = File.ReadAllBytes(SaveFilePath);
            byte[] key = GetOrCreateUserKey();

            if (key == null)
            {
                Debug.LogError("불러오기 실패: 키를 가져올 수 없습니다");
                return TryLoadBackup();
            }

            string json = Decrypt(encryptedBytes, key);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (CryptographicException e)
        {
            Debug.LogError($"복호화 실패: {e.Message}");
            Debug.LogWarning("백업 파일 복구를 시도합니다...");
            return TryLoadBackup();
        }
        catch (Exception e)
        {
            Debug.LogError($"불러오기 중 오류 발생: {e.Message}\n{e.StackTrace}");
            return TryLoadBackup();
        }
    }

    // ================================
    //  백업 파일 복구 시도
    // ================================
    private static SaveData TryLoadBackup()
    {
        string backupPath = SaveFilePath + ".bak";

        if (!File.Exists(backupPath))
        {
            Debug.LogWarning("백업 파일도 없습니다");
            return null;
        }

        try
        {
            Debug.Log("백업 파일에서 복구 시도...");
            byte[] encryptedBytes = File.ReadAllBytes(backupPath);
            byte[] key = GetOrCreateUserKey();

            if (key == null)
            {
                Debug.LogError("백업 복구 실패: 키를 가져올 수 없습니다");
                return null;
            }

            string json = Decrypt(encryptedBytes, key);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("<color=yellow>백업에서 복구 성공!</color>");

            // 복구된 데이터를 메인 파일로 저장
            File.Copy(backupPath, SaveFilePath, true);

            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"백업 복구 실패: {e.Message}");
            return null;
        }
    }

    // ================================
    //  키 보호 (Wrap)
    // ================================
    private static string WrapKey(byte[] key)
    {
        try
        {
            byte[] masterKey = GetMasterKey();

            using (Aes aes = Aes.Create())
            {
                aes.Key = masterKey;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] encrypted = encryptor.TransformFinalBlock(key, 0, key.Length);

                    byte[] result = new byte[aes.IV.Length + encrypted.Length];
                    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                    Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"키 암호화 실패: {e.Message}");
            return null;
        }
    }

    // ================================
    //  키 보호 (Unwrap)
    // ================================
    private static byte[] UnwrapKey(string wrappedBase64)
    {
        try
        {
            byte[] data = Convert.FromBase64String(wrappedBase64);

            if (data.Length < IV_SIZE)
            {
                Debug.LogError($"손상된 키 데이터: 길이가 {data.Length}바이트 (최소 {IV_SIZE}바이트 필요)");
                return null;
            }

            byte[] masterKey = GetMasterKey();

            using (Aes aes = Aes.Create())
            {
                aes.Key = masterKey;

                byte[] iv = new byte[IV_SIZE];
                byte[] encrypted = new byte[data.Length - IV_SIZE];

                Buffer.BlockCopy(data, 0, iv, 0, IV_SIZE);
                Buffer.BlockCopy(data, IV_SIZE, encrypted, 0, encrypted.Length);

                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                }
            }
        }
        catch (FormatException e)
        {
            Debug.LogError($"Base64 디코딩 실패: {e.Message}");
            return null;
        }
        catch (CryptographicException e)
        {
            Debug.LogError($"키 복호화 실패: {e.Message}");
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"키 Unwrap 중 오류: {e.Message}");
            return null;
        }
    }

    // ================================
    //  AES 암호화
    // ================================
    private static byte[] Encrypt(string plainText, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV();

            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                byte[] result = new byte[aes.IV.Length + cipherBytes.Length];
                Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

                return result;
            }
        }
    }

    // ================================
    //  AES 복호화
    // ================================
    private static string Decrypt(byte[] data, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;

            byte[] iv = new byte[IV_SIZE];
            byte[] cipherBytes = new byte[data.Length - iv.Length];

            Buffer.BlockCopy(data, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(data, iv.Length, cipherBytes, 0, cipherBytes.Length);

            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor())
            {
                byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
        }
    }

    // ================================
    //  마스터 키
    // ================================
    private static byte[] GetMasterKey()
    {
        return new byte[]
        {
            0x4A, 0xF2, 0x8B, 0x1C, 0xD5, 0x63, 0xA9, 0x7E,
            0x2F, 0xB4, 0x5D, 0xC8, 0x91, 0x3A, 0xE6, 0x77,
            0x0C, 0xF8, 0x45, 0xB1, 0x6E, 0xD2, 0x89, 0x3F,
            0xA5, 0x1B, 0x74, 0xC9, 0x52, 0xDE, 0x87, 0x30
        };
    }

    // ================================
    //  저장 파일 삭제 (디버깅용)
    // ================================
    public static void DeleteSaveFile()
    {
        try
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                Debug.Log("저장 파일 삭제 완료");
            }

            string backupPath = SaveFilePath + ".bak";
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
                Debug.Log("백업 파일 삭제 완료");
            }

            PlayerPrefs.DeleteKey(KEY_PREF);
            PlayerPrefs.Save();
            Debug.Log("암호화 키 삭제 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"파일 삭제 중 오류: {e.Message}");
        }
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
        // loadData = SaveSystem.Load();
        // if (loadData == null) SaveGameData();
    }
    // void Start()
    // {
    //     LoadGameData();
    // }

    public void LoadGameData()
    {
        loadData = SaveSystem.Load();
        if (loadData == null)
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
        coin = 1;
        eventNumber = "1_0";

        saveData = new SaveData(health, mental, coin, eventNumber, paintName);
        SaveSystem.Save(saveData);
    }
}
