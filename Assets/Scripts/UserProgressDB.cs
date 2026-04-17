using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UserProgressDB", menuName = "Game/UserProgressDB")]
public class UserProgressDB : ScriptableObject
{
    public Dictionary<string, int> userLevels = new Dictionary<string, int>();

    public void SaveLocalProgress(string userId, int level)
    {
        if (!userLevels.ContainsKey(userId))
        {
            userLevels.Add(userId, level);
        }
        else
        {
            userLevels[userId] = level;
        }
        PlayerPrefs.SetInt(userId + "_Level", level);
    }
}
