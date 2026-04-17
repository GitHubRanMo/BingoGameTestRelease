using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;

public class Level : MonoBehaviour
{
    public enum LevelType
    {
        Timer,
        Obstacle,
        Moves,
    };
    public AudioClip unBelievableSource;
    public GameGrid gameGrid;
    public HUD hud;
    public GameObject compedPanel;
    public TextMeshProUGUI compedPanelText;

    public int score1Star;
    public int score2Star;
    public int score3Star;
    protected LevelType type;
    [HideInInspector]
    public bool isWon;
    public LevelType Type
    {
        get { return type; }
    }

    protected int currentScore;
    void Start()
    {
        hud.SetScore(currentScore);
    }
    public virtual void GameWin()
    {
        isWon= true;
        AudioSource.PlayClipAtPoint(unBelievableSource, transform.position);
        hud.OnGameWin(currentScore);
        StartCoroutine(CompletedPanel());
    }
    public virtual IEnumerator CompletedPanel()
    {
        yield return new WaitForSeconds(2.5f);
        gameGrid.GameOver();
        compedPanel.SetActive(true);

        Animator animator = compedPanel.GetComponent<Animator>();
        if (animator)
        {
            animator.Play("CompedPanelShow");
        }

        if (isWon)
        {
            hud.CompedStar(currentScore);
            compedPanelText.text = hud.scoreText.text;

            if (GameProgressManager.Instance != null)
            {
                // 将异步保存转换为协程方式
                yield return StartCoroutine(SaveProgressCoroutine());
            }
        }
        else
        {
            compedPanelText.text = "You Lose";
            hud.CompedStar(currentScore);
            compedPanelText.fontSize = 100;
        }
    }

    private IEnumerator SaveProgressCoroutine()
    {
        bool isDone = false;
        bool success = false;
        GameProgressManager.Instance.SaveProgress(hud.levelId, hud.starIndex)
            .ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.LogError($"Failed to save progress: {task.Exception.Message}");
                }
                else
                {
                    Debug.Log("Progress saved successfully");
                }
                isDone = true;
                success = !task.IsFaulted;
            });

        while (!isDone)
        {
            yield return null;
        }

        if (!success)
        {
            // 可以在这里添加保存失败的UI提示
        }
    }
    public virtual void GameLose()
    {
        isWon= false;
        hud.OnGameLose();
        StartCoroutine(CompletedPanel());

    }
    public virtual void OnMove() { }
    public virtual void OnPieceCleared(GamePiece piece)
    {
        //Update Score
        currentScore += piece.score;
        //Debug.Log("Score:" + currentScore);
        hud.SetScore(currentScore);
    }
}
