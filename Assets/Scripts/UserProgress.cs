using System;
using System.Collections.Generic;

[Serializable]
public class UserProgress
{
    [Serializable]
    public class LevelStarData
    {
        public string levelId;
        public int stars;

        public LevelStarData(string levelId, int stars)
        {
            this.levelId = levelId;
            this.stars = stars;
        }
    }

    public List<LevelStarData> levelStarList;
    public UserProgress()
    {
        levelStarList = new List<LevelStarData>();
    }

    // 添加一个方法来获取特定关卡的星星数
    public int GetStarsForLevel(string levelId)
    {
        var levelData = levelStarList.Find(x => x.levelId == levelId);
        return levelData?.stars ?? 0;
    }

    // 添加一个方法来设置特定关卡的星星数
    public void SetStarsForLevel(string levelId, int stars)
    {
        var levelData = levelStarList.Find(x => x.levelId == levelId);
        if (levelData != null)
        {
            levelData.stars = stars;
        }
        else
        {
            levelStarList.Add(new LevelStarData(levelId, stars));
        }
    }
}