using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Collections;

public class LoginUI : MonoBehaviour
{
    [Header("Login Panel")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public Button loginButton;
    public Button registerButton;
    public Text loginErrorText;
    public GameObject LoadBar;

    [Header("Register Panel")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public Button submitRegisterButton;
    public Button backToLoginButton;
    public Text registerErrorText;

    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    private bool isInitializing = true;
    private bool isTransitioning = false;

    void Start()
    {
        // 设置按钮监听
        if (loginButton != null) loginButton.onClick.AddListener(OnLoginClick);
        if (registerButton != null) registerButton.onClick.AddListener(ShowRegisterPanel);
        if (submitRegisterButton != null) submitRegisterButton.onClick.AddListener(OnRegisterClick);
        if (backToLoginButton != null) backToLoginButton.onClick.AddListener(ShowLoginPanel);

        // 禁用按钮直到Firebase初始化完成
        SetButtonsInteractable(false);
        StartCoroutine(WaitForFirebaseInitialization());
    }

    private void OnDestroy()
    {
        // 移除所有按钮监听
        if (loginButton != null) loginButton.onClick.RemoveListener(OnLoginClick);
        if (registerButton != null) registerButton.onClick.RemoveListener(ShowRegisterPanel);
        if (submitRegisterButton != null) submitRegisterButton.onClick.RemoveListener(OnRegisterClick);
        if (backToLoginButton != null) backToLoginButton.onClick.RemoveListener(ShowLoginPanel);
    }

    private System.Collections.IEnumerator WaitForFirebaseInitialization()
    {
        while (!FirebaseAuthManager.Instance.IsInitialized())
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (!isTransitioning)
        {
            SetButtonsInteractable(true);
        }
        isInitializing = false;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (isTransitioning) return;

        if (loginButton != null) loginButton.interactable = interactable;
        if (registerButton != null) registerButton.interactable = interactable;
        if (submitRegisterButton != null) submitRegisterButton.interactable = interactable;
        if (backToLoginButton != null) backToLoginButton.interactable = interactable;
    }

    private async void OnLoginClick()
    {
        if (isInitializing || isTransitioning)
        {
            if (loginErrorText != null)
            {
                loginErrorText.text = "Please wait...";
            }
            return;
        }

        string email = loginEmailInput != null ? loginEmailInput.text : "";
        string password = loginPasswordInput != null ? loginPasswordInput.text : "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (loginErrorText != null)
            {
                loginErrorText.text = "Please enter email and password";
            }
            return;
        }

        try
        {
            isTransitioning = true;
            SetButtonsInteractable(false);
            await FirebaseAuthManager.Instance.LoginUser(email, password);
            loginPanel.SetActive(false);
            LoadBar.SetActive(true);
            // 登录成功，加载主菜单场景
            //UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        catch (System.Exception e)
        {
            if (loginErrorText != null)
            {
                loginErrorText.text = e.Message;
            }
            isTransitioning = false;
            SetButtonsInteractable(true);
        }
    }

    private async void OnRegisterClick()
    {
        if (isInitializing || isTransitioning)
        {
            if (registerErrorText != null)
            {
                registerErrorText.text = "Please wait...";
            }
            return;
        }

        string email = registerEmailInput != null ? registerEmailInput.text : "";
        string password = registerPasswordInput != null ? registerPasswordInput.text : "";
        string confirmPassword = confirmPasswordInput != null ? confirmPasswordInput.text : "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (registerErrorText != null)
            {
                registerErrorText.text = "Please enter email and password";
            }
            return;
        }

        if (password != confirmPassword)
        {
            if (registerErrorText != null)
            {
                registerErrorText.text = "Passwords do not match";
            }
            return;
        }

        try
        {
            isTransitioning = true;
            SetButtonsInteractable(false);
            await FirebaseAuthManager.Instance.RegisterUser(email, password);
            ShowLoginPanel();
            if (loginErrorText != null)
            {
                loginErrorText.text = "Registration successful, please login";
            }
        }
        catch (System.Exception e)
        {
            if (registerErrorText != null)
            {
                registerErrorText.text = e.Message;
            }
            isTransitioning = false;
            SetButtonsInteractable(true);
        }
    }

    public void ShowLoginPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(true);
        if (registerPanel != null) registerPanel.SetActive(false);
        ClearInputs();
    }

    private void ShowRegisterPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (registerPanel != null) registerPanel.SetActive(true);
        ClearInputs();
    }

    private void ClearInputs()
    {
        if (loginEmailInput != null) loginEmailInput.text = "";
        if (loginPasswordInput != null) loginPasswordInput.text = "";
        if (registerEmailInput != null) registerEmailInput.text = "";
        if (registerPasswordInput != null) registerPasswordInput.text = "";
        if (confirmPasswordInput != null) confirmPasswordInput.text = "";
        if (loginErrorText != null) loginErrorText.text = "";
        if (registerErrorText != null) registerErrorText.text = "";
    }
}