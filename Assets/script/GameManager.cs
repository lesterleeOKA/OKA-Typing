using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class GameManager : GameBaseController
{
    public static GameManager Instance = null;
    public QuestionData questionData;
    public bool timesup = false;
    public List<wordfall> playerControllers;
    public GameObject progressBar;
    public Image progressBarImage;


    protected override void Awake()
    {
        if (Instance == null) Instance = this;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        if (LoaderConfig.Instance.apiManager.IsLogined)
        {
            this.progressBar.GetComponent<CanvasGroup>().alpha = 1f;
            this.progressBar.GetComponentInChildren<NumberCounter>().Unit = "/" + questionData.questions.Count;
            this.progressBar.GetComponentInChildren<NumberCounter>().Value = 0;
        }
        else this.progressBar.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void EndGame()
    {
        this.timesup = true;
        bool showSuccess = false;
        for (int i = 0; i < this.playerControllers.Count; i++)
        {
            if (i < this.playerNumber)
            {
                var playerController = this.playerControllers[i];
                if (playerController != null)
                {
                    if (playerController.Score >= 30)
                    {
                        showSuccess = true;
                    }
                    Debug.Log("Player " + i + " final score: " + playerController.Score);
                    this.endGamePage.updateFinalScore(i, playerController.Score);
                }
            }
        }
        this.endGamePage.setStatus(true, showSuccess);
        base.endGame();
    }

    public override void enterGame()
    {
        base.enterGame();
    }
}
