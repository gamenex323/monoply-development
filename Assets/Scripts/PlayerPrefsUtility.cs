//using UnityEngine;

//public static class PlayerPrefsUtility
//{
//    // Generic setter for PlayerPrefs
//    public static string money = "money";
//    public static string shields = "shields";
//    public static void Set<T>(string key, T value)
//    {
//        if (value is int)
//        {
//            PlayerPrefs.SetInt(key, (int)(object)value);
//        }
//        else if (value is float)
//        {
//            PlayerPrefs.SetFloat(key, (float)(object)value);
//        }
//        else if (value is string)
//        {
//            PlayerPrefs.SetString(key, (string)(object)value);
//        }
//        else
//        {
//            Debug.LogError("Unsupported type for PlayerPrefs: " + typeof(T));
//        }
//    }

//    // Generic getter for PlayerPrefs
//    public static T Get<T>(string key, T defaultValue = default)
//    {
//        if (typeof(T) == typeof(int))
//        {
//            return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
//        }
//        else if (typeof(T) == typeof(float))
//        {
//            return (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
//        }
//        else if (typeof(T) == typeof(string))
//        {
//            return (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue);
//        }
//        else
//        {
//            Debug.LogError("Unsupported type for PlayerPrefs: " + typeof(T));
//            return defaultValue;
//        }
//    }
//}
