using Firebase.Auth;
using Firebase.Database; //  Firestore
using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    // 用于缓存当前用户的进度
    private UserProgress currentUserProgress;
    private UserName currentUserName;
    private DatabaseReference dbReference;
    private FirebaseAuth auth;
    private string userId;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        try
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            auth = FirebaseAuth.DefaultInstance;
            Debug.Log("Successfully connected to database");
            userId = auth.CurrentUser?.UserId;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Firebase initialization or database URL error: " + e.Message);
        }
    }

    // 加载用户进度
    public async Task LoadProgress()
    {
        if (auth.CurrentUser == null)
        {
            Debug.Log("No user logged in, using temporary local progress.");
            currentUserProgress = new UserProgress();
            return;
        }

        userId = auth.CurrentUser.UserId;
        try
        {
            var dataSnapshot = await dbReference.Child("users").Child(userId).Child("progress").GetValueAsync();

            if (dataSnapshot.Exists)
            {
                string json = dataSnapshot.GetRawJsonValue();
                currentUserProgress = JsonUtility.FromJson<UserProgress>(json);
                Debug.Log("Successfully loaded user progress from Firebase");
                Debug.Log($"Loaded progress data: {json}");
            }
            else
            {
                currentUserProgress = new UserProgress();
                Debug.Log("No cloud progress found, created new local progress.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading progress: {e.Message}");
            currentUserProgress = new UserProgress();
        }
    }

    public async Task LoadUserName()
    {
        if (auth.CurrentUser == null)
        {
            Debug.Log("No user logged in, using temporary local progress.");
            currentUserName = new UserName();
            return;
        }

        userId = auth.CurrentUser.UserId;
        try
        {
            var dataSnapshot = await dbReference.Child("users").Child(userId).Child("username").GetValueAsync();

            if (dataSnapshot.Exists)
            {
                string json = dataSnapshot.GetRawJsonValue();
                currentUserName = JsonUtility.FromJson<UserName>(json);
                Debug.Log("Successfully loaded userName from Firebase");
                Debug.Log($"Loaded UserName data: {json}");
            }
            else
            {
                currentUserName = new UserName();
                Debug.Log("No cloud userName found, created new local userName.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading userName: {e.Message}");
            currentUserName = new UserName();
        }
    }
    // 保存用户进度
    public async Task SaveProgress(string levelId, int stars)
    {
        if (currentUserProgress == null)
        {
            Debug.LogError("Cannot save progress: User progress not loaded");
            return;
        }

        // 获取当前关卡的星星数
        int currentStars = currentUserProgress.GetStarsForLevel(levelId);

        // 业务逻辑：只保存更高的星级
        if (stars > currentStars)
        {
            currentUserProgress.SetStarsForLevel(levelId, stars);
            Debug.Log($"Updating progress for level {levelId} to {stars} stars");
        }
        else
        {
            Debug.Log("New score is not higher than existing score, skipping save.");
            return;
        }

        if (auth.CurrentUser == null)
        {
            Debug.Log("Guest mode: Progress not saved to cloud");
            return;
        }

        userId = auth.CurrentUser.UserId;
        try
        {
            string json = JsonUtility.ToJson(currentUserProgress);
            Debug.Log($"Saving progress data: {json}");
            await dbReference.Child("users").Child(userId).Child("progress").SetRawJsonValueAsync(json);
            Debug.Log($"Progress saved to Firebase: Level {levelId} - {stars} stars");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving progress: {e.Message}");
            throw;
        }
    }
    public async Task SaveUserName(string userName)
    {
        if (currentUserName == null)
        {
            Debug.LogError("Cannot save userName: User userName not loaded");
            return;
        }
        currentUserName.SetUserName(userName);
        if (auth.CurrentUser == null)
        {
            Debug.Log("Guest mode: UserName not saved to cloud");
            return;
        }
        userId = auth.CurrentUser.UserId;
        try
        {
            string json = JsonUtility.ToJson(currentUserName);
            Debug.Log($"Saving progress data: {json}");
            await dbReference.Child("users").Child(userId).Child("username").SetRawJsonValueAsync(json);
            Debug.Log($"UserName saved to Firebase: UserName {userName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving progress: {e.Message}");
            throw;
        }
    }
    // 获取特定关卡的进度
    public int GetProgressForLevel(string levelId)
    {
        if (currentUserProgress != null)
        {
            return currentUserProgress.GetStarsForLevel(levelId);
        }
        return 0;
    } 
    public string GetUserName()
    {
        if (currentUserName != null)
        {
            return currentUserName.GetUserName();
        }
        return "";
    }
    // 用户登出时清除进度
    public void ClearProgressOnLogout()
    {
        currentUserProgress = null;
        userId = null;
        Debug.Log("Progress cleared on logout");
    }
}