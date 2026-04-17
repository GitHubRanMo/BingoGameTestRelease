using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitPiece : MonoBehaviour
{
    public enum FruitType
    {
        Apple,
        Orange,
        Grape,
        BlueBerry,
        StrawBerry,
        Banana,
        Pear,
        Any,
        Count
    }
    [System.Serializable]
    public struct FruitSprite
    {
        public FruitType fruit;
        public Sprite sprite;
    }
    public FruitSprite[] fruitSprites;

    private FruitType fruit;
    public FruitType Fruit
    {
        get { return fruit; }
        set { SetFruit(value); }
    }
    public int NumFruits
    {
        get { return fruitSprites.Length;}
    }

    private SpriteRenderer sprite;
    private Dictionary<FruitType, Sprite> fruitSpriteDict;
    void Awake()
    {
        fruitSpriteDict = new Dictionary<FruitType, Sprite>();
        sprite = transform.Find("Tile").GetComponent<SpriteRenderer>();
        for (int i = 0; i < fruitSprites.Length; i++)
        {
            //Debug.Log(fruitSprites.Length);
            if (!fruitSpriteDict.ContainsKey(fruitSprites[i].fruit))
            {
                fruitSpriteDict.Add(fruitSprites[i].fruit, fruitSprites[i].sprite);
                //Debug.Log(fruitSpriteDict.Count);
            }
        }
    }
    public void SetFruit(FruitType newFruit)
    {
        fruit = newFruit;
        //Debug.Log(newFruit);
        //Debug.Log(fruitSpriteDict.Count);
        if (fruitSpriteDict.ContainsKey(newFruit))
        {
            sprite.sprite = fruitSpriteDict[newFruit];
            //Debug.Log(sprite.sprite.name);
        }
    }
}
