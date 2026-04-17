using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public int numsEnerge = 20;
    public Text numsEnergeTex;
    public TextMeshProUGUI energeBtnTex;
    private void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InvokeRepeating("UpdateNumsEnerge", 2f, 5f);
            
            // 添加场景加载事件监听
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 移除场景加载事件监听
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 延迟一帧执行，确保所有对象都已加载
        StartCoroutine(ReassignReferences());
    }

    private IEnumerator ReassignReferences()
    {
        // 等待一帧，确保所有对象都已完全加载
        yield return null;
        // 查找所有Canvas
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            // 在Canvas下查找所有Text组件
            Text[] allTexts = canvas.GetComponentsInChildren<Text>(true); // true表示包含未激活的对象
            foreach (Text text in allTexts)
            {
                // 根据组件名称或其他特征来识别目标组件
                if (text.name == "NumsEnergeTex")
                {
                    numsEnergeTex = text;
                }
            }

           //在Canvas下查找所有TextMeshProUGUI组件
           TextMeshProUGUI[] allTMPs = canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI tmp in allTMPs)
            {
                // 根据组件名称或其他特征来识别目标组件
                if (tmp.name == "EnergeBtnTex")
                {
                    energeBtnTex = tmp;
                }
            }
        }
        //energeBtnTex = GameObject.Find("Canvas/EnergeCountBtn/LabelBg/EnergeBtnTex").GetComponent<TextMeshProUGUI>();
        // 如果找到了组件，更新显示
        if (numsEnergeTex != null)
        {
            numsEnergeTex.text = numsEnerge.ToString();
        }
        if (energeBtnTex != null)
        {
            energeBtnTex.text = numsEnergeTex.text;
        }
    }

    private void UpdateNumsEnerge()
    {
        if (SceneManager.GetActiveScene().name == "LoginLoaded")
        {
            numsEnerge = 20;
            // 更新显示
            if (numsEnergeTex != null)
            {
                numsEnergeTex.text = numsEnerge.ToString();
            }
            if (energeBtnTex != null)
            {
                energeBtnTex.text = numsEnergeTex.text;
            }
        }
    }
}
