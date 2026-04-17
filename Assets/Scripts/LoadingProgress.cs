using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingProgress : MonoBehaviour
{
    public Slider progressBar;
    public float fillSpeed = 0.5f;
    void Start()
    {
        StartCoroutine(LoadGameAsync());
    }

    IEnumerator LoadGameAsync()
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime * 0.5f;
            progressBar.value = progress;
            yield return null;
        }
        SceneManager.LoadScene("MainMenu");
    }
}
