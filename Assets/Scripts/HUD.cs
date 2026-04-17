using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

public class HUD : MonoBehaviour
{
    [System.Serializable]
    public struct TargetPrefab
    {
        public Image[] fruits;
        public TextMeshProUGUI[] TargetGoals;
    }
    public Level level;
    public TextMeshProUGUI remainingText;
    public TextMeshProUGUI remainingSubText;
    public TextMeshProUGUI targetSubText;
    public TextMeshProUGUI scoreText;
    public Slider starSlider;
    public Image[] stars;

    public Image[] compedPanelStars;

    public TargetPrefab targetPrefab;
    public AudioSource audioSource;
    public AudioClip starFourSource;
    public GameObject tipsLabel;

    //private int starIndex = 0;
    private bool isGameOver = false;
    private int visibleStar;
    [HideInInspector]
    public int starIndex = 0;
    //private int changeScore;
    private bool isLeftPivot = false;
    [SerializeField] private RectTransform targetObject;
    [SerializeField] private Button button;
    private AudioSource BingoGameMus;
    [HideInInspector]
    public string levelId;
    //private int levelIdInt;
    [SerializeField]
    private GameObject[] GoalsPanels;
    void Start()
    {
        BingoGameMus = GameObject.Find("Board/bg_Board").GetComponent<AudioSource>();
        starSlider.value= 0;
        for(int i=0;i<stars.Length;i++)
        {
            stars[i].enabled = false;
        }
        levelId = SceneManager.GetActiveScene().name;
    }
    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
        
        // Calculate progress bar value (0-30)
        if (score >= 0 && score < level.score1Star)
        {
            visibleStar = 0;
            starSlider.value = (float)score * 10f / level.score1Star;
            //Debug.Log(starSlider.value);
        }
        else if(score >= level.score1Star && score < level.score2Star)
        {
            visibleStar = 1;
            starSlider.value = 10f + (float)(score - level.score1Star) * 10f / (level.score2Star - level.score1Star);
            //Debug.Log(starSlider.value);
        }
        else if(score >= level.score2Star && score < level.score3Star)
        {
            visibleStar = 2;
            starSlider.value = 20f + (float)(score - level.score2Star) * 10f / (level.score3Star - level.score2Star);
            //Debug.Log(starSlider.value);
        }
        else if(score >= level.score3Star)
        {
            visibleStar = 3;
            starSlider.value = 30f; // Set to maximum value when reaching 3 stars
        }

        // Update star visibility
        for(int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = (i <= visibleStar);
        }
        starIndex = visibleStar;
    }
    public void CompedStar(int score)
    {
        // First disable all stars
        for (int i = 0; i < compedPanelStars.Length; i++)
        {
            compedPanelStars[i].enabled = false;
        }

        if (level.isWon)
        {
            // Start coroutine to show stars sequentially
            StartCoroutine(ShowStarsSequentially());
        }
    }
    private IEnumerator ShowStarsSequentially()
    {
        for (int i = 0; i <= visibleStar; i++)
        {
            yield return new WaitForSeconds(0.5f);
            compedPanelStars[i].enabled = true;
            if (starFourSource != null&&i!=0)
            {
                audioSource.PlayOneShot(starFourSource);
                //Debug.Log("here");
            }
        }
    }
    public void SetTarget(int target1,int target2)
    {
        targetPrefab.TargetGoals[0].text=target1.ToString();
        targetPrefab.TargetGoals[1].text=target2.ToString();
    }
    public void SetTarget(int target)
    {
        targetPrefab.TargetGoals[0].text=target.ToString();
    }
    public void SetRemaining(int remaining)
    {
        remainingText.text=remaining.ToString();
    }
    public void SetLevelType(Level.LevelType type)
    {
        if (type == Level.LevelType.Moves)
        {
            //remainingSubText.text = "moves remaining";
            remainingSubText.text = "MOVE";
            targetSubText.text = "GOAL";
        }else if(type == Level.LevelType.Obstacle){
            remainingSubText.text = "MOVE";
            targetSubText.text = "GOAL";
        }else if (type == Level.LevelType.Timer)
        {
            remainingSubText.text = "TIME";
            targetSubText.text = "GOAL";
        }
    }
    public void OnGameWin(int score)
    {
        isGameOver = true;
    }
    public void OnGameLose()
    {
        isGameOver= true;
    }
    public void OnReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnNextGameSceneClicked()
    {
        if (level.isWon)
        {
            if (levelId=="Level_1")
            {
                GoalsPanels[0].gameObject.SetActive(true);
            }else if (levelId == "Level_2")
            {
                GoalsPanels[1].gameObject.SetActive(true);
            }
        }
        else
        {
            tipsLabel.SetActive(true);
            tipsLabel.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Current level failure,please replay!";
            StartCoroutine(TipsShow());
        }
    }
    public IEnumerator TipsShow()
    {
        //tipsLabel = this.tipsLabel;
        Animator tipsAni=tipsLabel.GetComponent<Animator>();
        if (tipsAni)
        {
            tipsAni.Play("TipsShow");
        }
        yield return new WaitForSeconds(2f);
        tipsLabel.SetActive(false);
    }
    public void OnNext2Clicked()
    {
        if (level.isWon)
        {
            tipsLabel.SetActive(true);
            tipsLabel.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Stay tuned!";
            StartCoroutine(TipsShow());
        }
        else
        {
            tipsLabel.SetActive(true);
            tipsLabel.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Current level failure,please replay!";
            StartCoroutine(TipsShow());
        }
    }
    public void OnQuitClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void OnMusicClicked()
    {
        Vector3 anchorPos = new Vector3(-100.5f,0,0);
        if (!isLeftPivot)
        {
            targetObject.anchoredPosition = -anchorPos;
            targetObject.anchorMin = new Vector2(0, 0.5f);
            targetObject.anchorMax = new Vector2(0, 0.5f);
            BingoGameMus.enabled = false;
        }
        else
        {
            targetObject.anchoredPosition = anchorPos;
            targetObject.anchorMin = new Vector2(1, 0.5f);
            targetObject.anchorMax = new Vector2(1, 0.5f);
            BingoGameMus.enabled=true;
        }
        isLeftPivot = !isLeftPivot;
    }
}
