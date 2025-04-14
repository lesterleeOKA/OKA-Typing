using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class gamestart : MonoBehaviour
{
    
    public GameObject soundOnOffPanel;
    public GameObject gameStartPanel;
    public string targetSceneName = "Gameplay";
    //public AudioControl audioControl;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        //LoaderConfig.Instance?.InitialGameBackground();

    }

    private void Start()
    {
        
        soundOnOffPanel.transform.DOLocalMoveY(0f,1f);
        soundOnOffPanel.GetComponent<CanvasGroup>().DOFade(1f, 0.75f);
        soundOnOffPanel.GetComponent<CanvasGroup>().interactable = true;

    }
    public void soundOnbutton()
    {
        //audioControl.setAudioStatus(true);
        soundOnOffPanel.SetActive(false);
        gameStartPanel.GetComponent<CanvasGroup>().interactable = true;
        gameStartPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        gameStartPanel.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
        gameStartPanel.GetComponent<CanvasGroup>().DOFade(1f, 1f);
       

    }
    public void soundOffbutton()
    {
        //LoaderConfig.Instance.isSoundOn = false;
        //audioControl.setAudioStatus(false);
        soundOnOffPanel.SetActive(false);
        gameStartPanel.GetComponent<CanvasGroup>().interactable = true;
        gameStartPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        gameStartPanel.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
        gameStartPanel.GetComponent<CanvasGroup>().DOFade(1f, .25f);

    }
    public void StartGame()
    {
        gameStartPanel.GetComponent<CanvasGroup>().interactable = false;
        gameStartPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        gameStartPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
        gameStartPanel.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 1f);
        Debug.Log("Start Game");
        SceneManager.LoadScene(targetSceneName);
        if(GameManager.Instance != null)
            GameManager.Instance.timesup = false;
        //LoaderConfig.Instance.isGameStarted = false;
    }
}
