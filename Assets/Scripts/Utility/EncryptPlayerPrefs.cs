using UnityEngine;

// PlayerPrefs에 사용되는 키들
public struct PrefsKeys
{
    public const string PRIVATE_KEY = "PRIVATE_KEY";
    public const string IS_AUTO_LOGIN = "isAutoLogin";
    public const string NICK_NAME = "nickName";
    public const string PW = "pw";
}

public static class EncryptPlayerPrefs
{
    public static void DeleteAll() => PlayerPrefs.DeleteAll();

    public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);

    public static bool HasKey(string key) => PlayerPrefs.HasKey(key);

    public static string GetString(string key, string defaultValue)
    {
        string encryptedRes = PlayerPrefs.GetString(key);

        if (string.IsNullOrEmpty(encryptedRes))
        {
            return defaultValue;
        }

        return CryptoGraphy.Decrypt($"{encryptedRes}");
    }

    public static string GetString(string key) => GetString(key, string.Empty);

    public static float GetFloat(string key, float defaultValue)
    {
        string decryptedRes = GetString(key);

        if (string.IsNullOrEmpty(decryptedRes))
        {
            return defaultValue;
        }

        if (float.TryParse(decryptedRes, out float res) == false)
        {
            Debug.Assert(false);
            Debug.Log($"GetFloat({key}, {defaultValue}) 실패");
            return defaultValue;
        }

        return res;
    }

    public static float GetFloat(string key) => GetFloat(key, 0f);

    public static int GetInt(string key, int defaultValue)
    {
        string decryptedRes = GetString(key);

        if (string.IsNullOrEmpty(decryptedRes))
        {
            return defaultValue;
        }

        if (int.TryParse(decryptedRes, out int res) == false)
        {
            Debug.Assert(false);
            Debug.Log($"GetInt({key}, {defaultValue}) 실패");
            return defaultValue;
        }

        return res;
    }

    public static int GetInt(string key) => GetInt(key, 0);

    public static bool GetBool(string key, bool defaultValue = false)
    {
        string decryptRes = GetString(key);

        if (string.IsNullOrEmpty(decryptRes))
        {
            return defaultValue;
        }

        if (bool.TryParse(decryptRes, out bool res) == false)
        {
            Debug.Assert(false);
            Debug.Log($"GetBool({key}, {defaultValue}) 실패");
            return defaultValue;
        }

        return res;
    }

    public static void SetString(string key, string value)
    {
        string encryptedValue = CryptoGraphy.Encrypt(value);
        PlayerPrefs.SetString(key, encryptedValue);
    }

    public static void SetFloat(string key, float value)
    {
        string encryptedValue = CryptoGraphy.Encrypt($"{value}");
        PlayerPrefs.SetString(key, encryptedValue);
    }

    public static void SetInt(string key, int value)
    {
        string encryptedValue = CryptoGraphy.Encrypt($"{value}");
        PlayerPrefs.SetString(key, encryptedValue);
    }

    public static void SetBool(string key, bool value)
    {
        string encryptedValue = CryptoGraphy.Encrypt($"{value}");
        PlayerPrefs.SetString(key, encryptedValue);
    }
}
