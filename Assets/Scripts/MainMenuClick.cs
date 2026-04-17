using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuClick : MonoBehaviour
{
    [System.Serializable]
    public struct ButtonPlayerPrefs
    {
        public GameObject gameObject;
        public string playerPrefKey;
    }
    [System.Serializable]
    public struct DiffScenePanel
    {
        public GameObject diffScenePanel;
        public string playerPrefKey;
    }
    public ButtonPlayerPrefs[] buttons;
    public DiffScenePanel[] diffScene;
    [HideInInspector]
    public string currentScene;
    public Text userNameTex;
    public InputField UNameInput;
    [SerializeField]
    private GameObject LoginOutPanel;
    void Start()
    {
        if (GameProgressManager.Instance == null) return;

        for (int i = 0; i < buttons.Length; i++)
        {
            // 从GameProgressManager获取进度，而不是使用PlayerPrefs
            int score = GameProgressManager.Instance.GetProgressForLevel(buttons[i].playerPrefKey);

            for (int starIndex = 1; starIndex <= 3; starIndex++)
            {
                Transform star = buttons[i].gameObject.transform.Find("star" + starIndex);
                star.gameObject.SetActive(starIndex <= score);
            }
        }
        UNameInput.text = GameProgressManager.Instance.GetUserName();
    }
    public void Level1Clicked()
    {
        SceneManager.LoadScene("Level_2");
    }
    public void Level2Clicked()
    {
        SceneManager.LoadScene("Level_3");
    }
    public void GameSceneClicked()
    {
        SceneManager.LoadScene("Level_1");
    }
    public async void OnLoginOutClicked()
    {
        await GameProgressManager.Instance.SaveUserName(userNameTex.text);
        SceneManager.LoadScene("LoginLoaded");
        GameProgressManager.Instance.ClearProgressOnLogout();
    }
    public async void OnCancelClicked()
    {
        await GameProgressManager.Instance.SaveUserName(userNameTex.text);
        LoginOutPanel.SetActive(false);
    }
    public void OnGoGameBtnClicked()
    {
        currentScene ="";
        if (GameProgressManager.Instance == null) return;
        //Debug.Log("here");
        for (int i = 0; i < diffScene.Length; i++)
        {
            int score = GameProgressManager.Instance.GetProgressForLevel(diffScene[i].playerPrefKey);
            if (score == 0)
            {
                diffScene[i].diffScenePanel.SetActive(true);
                currentScene= diffScene[i].playerPrefKey;
                return;
            }
        }
        Debug.Log("恭喜您成功通关");
    }
}
