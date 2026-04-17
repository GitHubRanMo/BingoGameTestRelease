using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnergeMenuClick : MonoBehaviour
{
    //public Text numsEnergeTex;
    [SerializeField]
    private Text timeTex;
    //public TextMeshProUGUI energeBtnTex;
    [SerializeField]
    private GameObject useEnergePanel;
    [SerializeField]
    private TextMeshProUGUI addOneTex;
    [SerializeField]
    private TextMeshProUGUI addFiveTex;
    [SerializeField]
    private TextMeshProUGUI addFullTex;
    [SerializeField]
    private Button sureBtn;
    [SerializeField]
    private GameObject tipsLabel;


    [SerializeField]
    private GameObject energeCountImg1;
    [SerializeField]
    private GameObject energeCountImg2;
    [SerializeField]
    private GameObject energeCountImg3;

    [SerializeField]
    private GameObject nextIcon;
    [SerializeField]
    private MainMenuClick mainMenuClick;
    private int timeInSeconds = 30;
    private float timer;
    //private int numsEnerge;
    private TextMeshProUGUI energeImgTex;
    private bool isAddOne=false;
    private bool isAddFive=false;
    private bool isAddFull=false;
    void Start()
    {
        StartCoroutine(InitializeUI());
    }

    private IEnumerator InitializeUI()
    {
        // 等待一帧，确保GameDataManager已经初始化
        yield return null;

        // 等待直到GameDataManager的组件被正确赋值
        while (GameDataManager.Instance.numsEnergeTex == null || GameDataManager.Instance.energeBtnTex == null)
        {
            yield return null;
        }

        // 设置初始值
        GameDataManager.Instance.numsEnergeTex.text = GameDataManager.Instance.numsEnerge.ToString();
        GameDataManager.Instance.energeBtnTex.text = GameDataManager.Instance.numsEnergeTex.text;
        timeTex.text = $"{timeInSeconds / 60} : {timeInSeconds % 60:00}";
        addOneTex.text = "5";
    }
    void Update()
    {
        timer += Time.deltaTime;
        TimeChangeEnerge();
    }
    private void TimeChangeEnerge()
    {
        if (GameDataManager.Instance.numsEnerge >= 30)
        {
            GameDataManager.Instance.numsEnergeTex.text = "30";
            timeTex.text = $"{0} : {0:00}";
            timer = 0;
        }
        else
        {
            timeTex.text = $"{(int)Mathf.Max((timeInSeconds-timer) / 60,0)} : {(int)Mathf.Max((timeInSeconds - timer) % 60, 0):00}";
            if (timer >= timeInSeconds)
            {
                timer= 0 ;
                GameDataManager.Instance.numsEnerge++;
                GameDataManager.Instance.numsEnergeTex.text = GameDataManager.Instance.numsEnerge.ToString();
            }
        }
        GameDataManager.Instance.energeBtnTex.text= GameDataManager.Instance.numsEnergeTex.text;
    }
    public void OnAddOneClicked()
    {
        isAddOne = true;
        useEnergePanel.SetActive(true);
        sureBtn.onClick.AddListener(OnSureBtnClick);
    }
    public void OnAddFiveClicked()
    {
        isAddFive = true;
        useEnergePanel.SetActive(true);
        sureBtn.onClick.AddListener(OnSureBtnClick);
    }
    public void OnAddFullClicked()
    {
        isAddFull = true;
        useEnergePanel.SetActive(true);
        sureBtn.onClick.AddListener(OnSureBtnClick);
    }
    private void OnSureBtnClick()
    {
        useEnergePanel.SetActive(false);
        if (isAddOne)
        {
            int numsAdd = int.Parse(addOneTex.text);
            if (GameDataManager.Instance.numsEnerge <= 29)
            {
                if (numsAdd > 0)
                {
                    numsAdd--;
                    addOneTex.text = numsAdd.ToString();
                    GameDataManager.Instance.numsEnerge += 1;
                    GameDataManager.Instance.numsEnergeTex.text = GameDataManager.Instance.numsEnerge.ToString();
                    GameDataManager.Instance.energeBtnTex.text = GameDataManager.Instance.numsEnergeTex.text;
                }
            }
            else
            {
                tipsLabel.SetActive(true);
                tipsLabel.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Energy is already full. No need to use Energy Bottle";
                StartCoroutine(TipsShow());
            }
            isAddOne = false;
            //Debug.Log("1");
        }else if (isAddFive)
        {
            int numsAdd = int.Parse(addFiveTex.text);
            if (GameDataManager.Instance.numsEnerge <= 25)
            {
                if (numsAdd > 0)
                {
                    numsAdd--;
                    addFiveTex.text = numsAdd.ToString();
                    GameDataManager.Instance.numsEnerge += 5;
                    GameDataManager.Instance.numsEnergeTex.text = GameDataManager.Instance.numsEnerge.ToString();
                    GameDataManager.Instance.energeBtnTex.text = GameDataManager.Instance.numsEnergeTex.text;
                }
            }else if (GameDataManager.Instance.numsEnerge <30)
            {
                if (numsAdd > 0)
                {
                    numsAdd--;
                    addFiveTex.text = numsAdd.ToString();
                    GameDataManager.Instance.numsEnerge = 30;
                    GameDataManager.Instance.numsEnergeTex.text = GameDataManager.Instance.numsEnerge.ToString();
                    GameDataManager.Instance.energeBtnTex.text = GameDataManager.Instance.numsEnergeTex.text;
                }
            }
            else
            {
                tipsLabel.SetActive(true);
                tipsLabel.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Energy is already full. No need to use Energy Bottle";
                StartCoroutine(TipsShow());
            }
            isAddFive = false;
            //Debug.Log("2");
        }
        else if(isAddFull)
        {
            int numsAdd = int.Parse(addFullTex.text);
            if (GameDataManager.Instance.numsEnerge < 30)
            {
                if (numsAdd > 0)
                {
                    numsAdd--;
                    addFullTex.text = numsAdd.ToString();
                    GameDataManager.Instance.numsEnerge = 30;
                    GameDataManager.Instance.numsEnergeTex.text = GameDataManager.Instance.numsEnerge.ToString();
                    GameDataManager.Instance.energeBtnTex.text = GameDataManager.Instance.numsEnergeTex.text;
                }
            }
            else
            {
                tipsLabel.SetActive(true);
                tipsLabel.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Energy is already full. No need to use Energy Bottle";
                StartCoroutine(TipsShow());
            }
            isAddFull = false;
            //Debug.Log("3");
        }
        
    }
    public IEnumerator TipsShow()
    {
        //tipsLabel = this.tipsLabel;
        Animator tipsAni = tipsLabel.GetComponent<Animator>();
        if (tipsAni)
        {
            tipsAni.Play("TipsShow");
        }
        yield return new WaitForSeconds(2f);
        tipsLabel.SetActive(false);
    }
    public void OnStartGame1Clicked()
    {
        if (GameDataManager.Instance.numsEnerge >= 5)
        {
            energeCountImg1.SetActive(true);
            energeImgTex = energeCountImg1.transform.Find("LabelBg/NumsLabel").GetComponent<TextMeshProUGUI>();
            energeImgTex.text = GameDataManager.Instance.numsEnergeTex.text;
            StartCoroutine(EnergeShow());
        }
        else
        {
            LessEnerge();
        }
    }
    public void LessEnerge()
    {
        tipsLabel.SetActive(true);
        tipsLabel.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Energe low. Use a potion to refill?";
        StartCoroutine(TipsShow());
    }
    public void OnStartGame2Clicked()
    {
        if (GameDataManager.Instance.numsEnerge >= 5)
        {
            energeCountImg2.SetActive(true);
            energeImgTex = energeCountImg2.transform.Find("LabelBg/NumsLabel").GetComponent<TextMeshProUGUI>();
            energeImgTex.text = GameDataManager.Instance.numsEnergeTex.text;
            StartCoroutine(EnergeShow());
        }
        else
        {
            LessEnerge();
        }
    }
    public void OnStartGame3Clicked()
    {
        if (GameDataManager.Instance.numsEnerge >= 5)
        {
            energeCountImg3.SetActive(true);
            energeImgTex = energeCountImg3.transform.Find("LabelBg/NumsLabel").GetComponent<TextMeshProUGUI>();
            energeImgTex.text = GameDataManager.Instance.numsEnergeTex.text;
            StartCoroutine(EnergeShow());
        }
        else
        {
            LessEnerge();
        }
    }
    public IEnumerator EnergeShow()
    {
        GameDataManager.Instance.numsEnerge -= 5;
        //Debug.Log(GameDataManager.Instance.numsEnerge);
        GameDataManager.Instance.numsEnergeTex.text = GameDataManager.Instance.numsEnerge.ToString();
        //Debug.Log(numsEnergeTex.text);
        GameDataManager.Instance.energeBtnTex.text = GameDataManager.Instance.numsEnergeTex.text;
        yield return new WaitForSeconds(1.1f);
        energeImgTex.text = GameDataManager.Instance.numsEnergeTex.text;
        nextIcon.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        if (mainMenuClick.currentScene != "")
        {
            SceneManager.LoadScene(mainMenuClick.currentScene);
        }
    }
}
