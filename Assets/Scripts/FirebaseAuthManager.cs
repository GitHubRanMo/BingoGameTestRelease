using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

public class FirebaseAuthManager : MonoBehaviour
{
    public static FirebaseAuthManager Instance { get; private set; }

    private FirebaseAuth auth;
    private bool isInitialized = false;

    public bool IsLoggedIn => auth != null && auth.CurrentUser != null;

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
            return;
        }
    }

    async void Start()
    {
        try
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                isInitialized = true;
                Debug.Log("Firebase Auth initialized successfully");

                // 如果已经登录，自动加载进度
                if (IsLoggedIn)
                {
                    await GameProgressManager.Instance.LoadProgress();
                    await GameProgressManager.Instance.LoadUserName();
                }
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firebase initialization error: {e.Message}");
        }
    }

    public async Task RegisterUser(string email, string password)
    {
        if (!isInitialized)
        {
            throw new System.Exception("Firebase Auth is not initialized yet");
        }

        try
        {
            // 注册成功后，Firebase会自动登录该用户
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            if (result != null)
            {
                Debug.Log($"User registered and logged in successfully: {result.User.UserId}");
                PlayerPrefs.SetString("LastLoginEmail", email); // 保存邮箱，下次自动登录

                Debug.Log("正在发送验证邮件...");
                await result.User.SendEmailVerificationAsync();
                Debug.Log("验证邮件已发送。请提示用户检查收件箱。");

                //通知 GameProgressManager 创建新用户的进度，此时会创建一个空的
                await GameProgressManager.Instance.LoadProgress();
                await GameProgressManager.Instance.LoadUserName();
                Debug.Log("Initiated progress loading for new user.");
            }
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"Registration failed: {e.Message}");
            throw; // 重新抛出异常，让UI层处理
        }
    }

    public async Task LoginUser(string email, string password)
    {
        if (!isInitialized)
        {
            throw new System.Exception("Firebase Auth is not initialized yet");
        }

        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            if (result != null)
            {
                Debug.Log($"User logged in successfully: {result.User.UserId}");
                PlayerPrefs.SetString("LastLoginEmail", email);

                //根据之前的修改。登录成功后直接调用GameProgressManager的加载方法
                await GameProgressManager.Instance.LoadProgress();
                await GameProgressManager.Instance.LoadUserName();
                Debug.Log("Initiated progress loading for existing user.");
            }
        }
        catch (FirebaseException e)
        {
            Debug.Log($"Login failed: {e.Message}");
            throw;
        }
    }

    //添加登出功能，清除相关数据
    public void LogoutUser()
    {
        if (!IsLoggedIn) return;

        auth.SignOut();
        PlayerPrefs.DeleteKey("LastLoginEmail"); // 可选：清除记住的邮箱
        Debug.Log("User signed out.");

        //通知GameProgressManager清除本地缓存的进度数据
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.ClearProgressOnLogout();
        }
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }
}