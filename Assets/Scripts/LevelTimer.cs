using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelTimer : Level
{
    public int timeInSeconds;
    public int targetScore;

    private float timer;
    private bool timeOut = false;
    private bool isPauseBtnActive;
    void Start()
    {
        type = LevelType.Timer;

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(targetScore);
        //Debug.Log("Time:" + timeInSeconds + "second.Target score:" + targetScore);
    }
    void Update()
    {
        isPauseBtnActive = !GameObject.Find("UI/Canvas/PausePopup").activeSelf;
        //Debug.Log(isPauseBtnActive);
        if (!timeOut && isPauseBtnActive)
        {
            timer += Time.deltaTime;
            hud.SetRemaining(timeInSeconds - (int)timer);
            if (timeInSeconds - timer <= 0)
            {
                if (currentScore >= targetScore)
                {
                    GameWin(); // Fire and forget the async operation
                }
                else
                {
                    GameLose();
                }
                timeOut = true;
            }
        }
        else if(!timeOut && !isPauseBtnActive)
        {
            hud.SetRemaining(timeInSeconds - (int)timer);
        }
    }
}
