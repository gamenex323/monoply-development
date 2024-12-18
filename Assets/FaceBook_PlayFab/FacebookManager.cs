using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine.UI;

public class FacebookManager : MonoBehaviour
{

    private bool isInitialized = false; // Track if Facebook SDK is initialized
    private bool LoggedIn = false; // Track login status

    // Unity Awake: Initialize the Facebook SDK
    void Awake()
    {
        Debug.Log("FacebookManager Awake");

#if UNITY_ANDROID || UNITY_IOS && !UNITY_EDITOR
        if (!FB.IsInitialized)
        {
            FB.Init(OnInitComplete, OnHideUnity);
        }
        else
        {
            // Already initialized
            isInitialized = true;
            FB.ActivateApp();
        }
#else
        // Simulate initialization in Unity Editor
        if (!FB.IsInitialized)
        {
            FB.Init(OnInitComplete, OnHideUnity);
        }
        else
        {
            // Already initialized
            isInitialized = true;
            FB.ActivateApp();
        }
        isInitialized = true;
        Debug.Log("Facebook SDK Simulated in Unity Editor.");
#endif
    }

    // Called when Facebook SDK initialization is complete
    private void OnInitComplete()
    {
        if (FB.IsInitialized)
        {
            Debug.Log("Facebook SDK Initialized.");
            isInitialized = true;
            FB.ActivateApp();
        }
        else
        {
            Debug.LogError("Failed to Initialize Facebook SDK.");
        }
    }

    // Called when the app is hidden/shown
    private void OnHideUnity(bool isGameShown)
    {
        Time.timeScale = isGameShown ? 1 : 0;
    }

    // Facebook Login
    public void FBLogin()
    {
        if (!isInitialized)
        {
            Debug.LogError("Facebook SDK is not initialized. Please wait.");
            return;
        }

        if (!LoggedIn)
        {
            var perms = new List<string> { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }
        else
        {
            Debug.Log("Already logged in.");
        }
    }

    // Facebook Login Callback
    private void AuthCallback(ILoginResult result)
    {
        if (result == null || result.Error != null)
        {
            Debug.LogError($"Facebook login failed: {result?.Error}");
            return;
        }

        if (FB.IsLoggedIn)
        {
            Debug.Log("Facebook login successful.");
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            Debug.Log($"User ID: {aToken.UserId}");
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log($"Granted Permission: {perm}");
            }
            LoggedIn = true;
            GlobalData.NickName = aToken.UserId;
            // Additional post-login logic (if needed)
            HandlePostLogin();
        }
        else
        {
            Debug.Log("User cancelled login.");
        }
    }

    // Guest Login
    public void GuestLogin()
    {
        if (!LoggedIn)
        {
            Debug.Log("Guest login.");
            // Implement guest login logic here
        }
    }

    // Post Login Logic
    private void HandlePostLogin()
    {
        Debug.Log("Handling post-login actions.");
        // Example: Load user profile picture or transition to a new scene
        LoadUserProfilePicture();
    }

    // Load Facebook User Profile Picture
    private void LoadUserProfilePicture()
    {
        FB.API("/me?fields=picture.width(200).height(200)", HttpMethod.GET, OnProfilePictureLoaded);
    }

    // Callback for Profile Picture
    private void OnProfilePictureLoaded(IGraphResult result)
    {
        if (result == null || result.Error != null)
        {
            Debug.LogError($"Failed to load profile picture: {result?.Error}");
            return;
        }

        var data = result.ResultDictionary["picture"] as Dictionary<string, object>;
        var pictureData = data["data"] as Dictionary<string, object>;
        string pictureUrl = pictureData["url"] as string;

        Debug.Log($"Profile Picture URL: {pictureUrl}");
        StartCoroutine(DownloadProfilePicture(pictureUrl));
    }

    // Coroutine to Download Profile Picture
    private IEnumerator DownloadProfilePicture(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.error == null)
        {
            Debug.Log("Profile picture downloaded successfully.");
            Sprite profileSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
            // Use the downloaded profile picture (e.g., assign it to UI)
        }
        else
        {
            Debug.LogError($"Error downloading profile picture: {www.error}");
        }
    }
}
