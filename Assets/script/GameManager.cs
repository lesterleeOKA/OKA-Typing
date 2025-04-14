using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    [SerializeField] private AudioSource audioScoure;

    public string targetSceneName = "Gameplay";
    public QuestionData questionData;
    public Button soundOnOffButton;
    public GameObject soundOnOffPanel;
    public Sprite[] musicBtnStatus;
    public bool pauseGame = false;

    public float currentTime = 60f;
    public float startingTime = 60f;
    public TextMeshProUGUI timeText;
    public bool timesup = false;
    public GameObject endGameObject;
    public GameObject playerGameControl0;
    public GameObject playerGameControl1;
    public GameObject playerEndGameTxt0;
    public GameObject playerEndGameTxt1;
    public CanvasGroup playerbroad;
    bool timeUpSoundPlay = false;
    public AudioSource TimerAudioSource;
    public bool isGameStarted;
    public GameObject playerIcon1;
    public GameObject endGamePanelPlayerIcon1;


    public EndGamePage endGamePage;
    public RawImage backGroundImage;

    public GameObject progressBar;
    public Image progressBarImage;

    private Tween timerTween;

    public GameObject hintsPanel;

    public bool isHintPanelActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }

    }

    void Start()
    {
        if (LoaderConfig.Instance.gameSetup.gameTime != 0) this.startingTime = LoaderConfig.Instance.gameSetup.gameTime;
        this.currentTime = startingTime;
        backGroundImage.texture = LoaderConfig.Instance.gameSetup.bgTexture;

        if (LoaderConfig.Instance.apiManager.IsLogined)
        {
            progressBar.GetComponent<CanvasGroup>().alpha = 1f;
            progressBar.GetComponentInChildren<NumberCounter>().Unit = "/" + questionData.questions.Count;
            progressBar.GetComponentInChildren<NumberCounter>().Value = 0;
        }
        else progressBar.GetComponent<CanvasGroup>().alpha = 0f;
    }

    void Update()
    {

        if (isGameStarted && !timesup)
        {
            if (currentTime > 0)
            {
                if (currentTime < 10)
                {
                    if (timeUpSoundPlay == false)
                    {
                        if (AudioController.Instance.audioStatus != false)
                        {
                            timeUpSoundPlay = true;
                            TimerAudioSource.Play();
                        }

                        timeText.color = Color.yellow;
                        if (timerTween == null)
                        {
                            timerTween = timeText.rectTransform.DOAnchorPosY(timeText.rectTransform.anchoredPosition.y + 10, 0.5f)
                                .SetLoops(-1, LoopType.Yoyo)
                                .SetEase(Ease.InOutQuad);
                        }
                    }
                }
                currentTime -= Time.deltaTime;
                int minutes = Mathf.FloorToInt(currentTime / 60);
                int seconds = Mathf.FloorToInt(currentTime % 60);
                timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                this.currentTime = 0f;
                timesup = true;
                TimerAudioSource.Stop();
                Debug.Log("Time's up");
                if (timerTween != null)
                {
                    timerTween.Kill();
                    timerTween = null;
                }
                EndGame();
            }
        }
    }

    public void GetQuestionAnswer()
    {
        if (questionData != null && questionData.questions.Count > 0)
        {

        }
    }


    public void ReplayGame()
    {

        Time.timeScale = 1f;

        LoaderConfig.Instance.changeScene(1);
        isGameStarted = false;
        Debug.Log("replay");
    }

    public void TogglePause()
    {
        pauseGame = !pauseGame;
        Time.timeScale = pauseGame ? 0f : 1f;
    }
    public string GetCurrentDomainName
    {
        get
        {
            string absoluteUrl = Application.absoluteURL;
            Uri url = new Uri(absoluteUrl);
            Debug.Log("Host Name:" + url.Host);
            return url.Host;
        }
    }

    public void BackToHome()
    {

        ExternalCaller.BackToHomeUrlPage(LoaderConfig.Instance.apiManager.IsLogined);
    }

    public void EndGame()
    {
        isGameStarted = false;


        playerbroad.DOFade(0f, 2f).SetUpdate(true);

        Time.timeScale = 0f;

        int player1Id = 0;
        int player2Id = 1;

        int player1Score = endGamePage.scoreEndings[0].totalScore;
        int player2Score = endGamePage.scoreEndings[1].totalScore;
        endGamePage.updateFinalScore(player1Id, player1Score);
        endGamePage.updateFinalScore(player2Id, player2Score);
        endGamePage.EndGameScoreEffect(player1Id);
        endGamePage.EndGameScoreEffect(player2Id);
        if (player1Score != 0 || player2Score != 0)
        {
            endGamePage.setStatus(true, true);

        }
        else
        {
            endGamePage.setStatus(true, false);
        }

    }

    public void StartGame()
    {
        isGameStarted = true;
        Debug.Log("1111StartGame");
        Time.timeScale = 1f;
        //PlaybroadDisplay(1f);
        playerbroad.DOFade(1f, 1f);
    }

    //public void updatePlayerIcon()
    //{
    //    if (LoaderConfig.Instance.apiManager.peopleIcon != null)
    //    {
    //        var icon = SetUI.ConvertTextureToSprite(LoaderConfig.Instance.apiManager.peopleIcon as Texture2D);


    //        playerIcon1.GetComponent<Image>().sprite = icon;
    //        endGamePanelPlayerIcon1.GetComponent<Image>().sprite = icon;

    //        //playerIcon.GetComponent<Image>().enabled = true;

    //    }
    //    else
    //    {
    //        //playerIcon.GetComponent<Image>().enabled = false;
    //        playerIcon1.GetComponent<Image>().color = Color.clear;
    //    }
    //}

    //public void PlaybroadDisplay(float alpha)
    //{
    //    Debug.Log("PlaybroadDisplay"+alpha);
    //    playerbroad0.GetComponent<CanvasGroup>().DOFade(alpha, 2f).SetUpdate(true);
    //    playerbroad1.GetComponent<CanvasGroup>().DOFade(alpha, 2f).SetUpdate(true);
    //}

    public void DisplaySoundOnOffPanel()
    {
        soundOnOffButton.interactable = false;
        soundOnOffPanel.GetComponent<CanvasGroup>().DOFade(1f, 1f).SetUpdate(true);

        soundOnOffPanel.GetComponent<RectTransform>().DOLocalMoveY(-100f, 1f).SetUpdate(true).OnComplete(() =>
        {
            soundOnOffPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            soundOnOffPanel.GetComponent<CanvasGroup>().interactable = true;
        });


    }

    public void SoundbtnOnOff(bool on)
    {
        AudioController.Instance.changeBGMStatus(on);
        soundOnOffPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.3f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                soundOnOffButton.interactable = true;
                soundOnOffPanel.GetComponent<RectTransform>().localPosition = new Vector3(0f, 100f, 0f);
            });
        soundOnOffPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        soundOnOffPanel.GetComponent<CanvasGroup>().interactable = false;

        // ³]¸m«ö¶s¹Ï¤ù
        soundOnOffButton.image.sprite = on ? musicBtnStatus[1] : musicBtnStatus[0];

        //TimerAudioSource.enabled = on;
    }

    public void Hintsbtn(Button hintsbtn)
    {
        isHintPanelActive = !isHintPanelActive;
        if (hintsPanel != null)
        {
            CanvasGroup cg = hintsPanel.GetComponent<CanvasGroup>();
            Button hintsButton = hintsPanel.GetComponentInChildren<Button>();
            hintsButton.interactable = isHintPanelActive;
            
            if (isHintPanelActive)
            {

                cg.DOFade(1f, 0.5f).SetUpdate(true);
                cg.interactable = true;
                hintsbtn.interactable = false;
            }
            else
            {
                cg.interactable = false;
                cg.DOFade(0f, 0.5f).SetUpdate(true);
                hintsbtn.interactable = true;
            }


        }

    }

}
