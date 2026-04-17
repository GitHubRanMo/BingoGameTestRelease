using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnGame3ClickMa : MonoBehaviour
{
    [SerializeField]
    private GameObject energeCountImg3;
    private TextMeshProUGUI energeImgTex;
    [SerializeField]
    private GameObject nextIcon;
    [SerializeField]
    private GameObject tipsLabel;
    public void OnStartGame3Clicked()
    {
        if (GameDataManager.Instance.numsEnerge >= 5)
        {
            energeCountImg3.SetActive(true);
            energeImgTex = energeCountImg3.transform.Find("LabelBg/NumsLabel").GetComponent<TextMeshProUGUI>();
            energeImgTex.text = GameDataManager.Instance.numsEnerge.ToString();
            StartCoroutine(EnergeShow());
        }
        else
        {
            tipsLabel.SetActive(true);
            tipsLabel.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Energe low. Use a potion to refill?";
            StartCoroutine(TipsShow());
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
    public IEnumerator EnergeShow()
    {
        GameDataManager.Instance.numsEnerge -= 5;
        //Debug.Log(GameDataManager.Instance.numsEnerge);
        GameDataManager.Instance.numsEnergeTex.text = GameDataManager.Instance.numsEnerge.ToString();
        //Debug.Log(numsEnergeTex.text);
        GameDataManager.Instance.energeBtnTex.text = GameDataManager.Instance.numsEnergeTex.text;
        yield return new WaitForSeconds(1.1f);
        energeImgTex.text = GameDataManager.Instance.numsEnerge.ToString();
        nextIcon.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Level_3");
    }
}
