using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class GameController : GameBaseController
{
    public static GameController Instance = null;
    public QuestionData questionData;
    public CharacterSet[] characterSets;
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

        this.createPlayer();
    }

    public void EndGame()
    {
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

    void createPlayer()
    {
        for (int i = 0; i < this.maxPlayers; i++)
        {
            if (i < this.playerNumber)
            {
                if (this.playerControllers[i] != null) 
                {
                    this.playerControllers[i].gameObject.SetActive(true);
                    this.playerControllers[i].UserId = i;
                    this.playerControllers[i].Init(this.characterSets[i]);

                    if (i == 0 && LoaderConfig.Instance != null && LoaderConfig.Instance.apiManager.peopleIcon != null)
                    {
                        var _playerName = LoaderConfig.Instance?.apiManager.loginName;
                        var icon = SetUI.ConvertTextureToSprite(LoaderConfig.Instance.apiManager.peopleIcon as Texture2D);
                        this.playerControllers[i].UserName = _playerName;
                        this.playerControllers[i].updatePlayerIcon(true, _playerName, icon);
                    }
                    else
                    {
                        var icon = SetUI.ConvertTextureToSprite(this.characterSets[i].defaultIcon as Texture2D);
                        this.playerControllers[i].updatePlayerIcon(true, null, icon);
                    }
                }
            }
            else
            {
                int notUsedId = i + 1;
                var notUsedPlayerIcon = GameObject.FindGameObjectWithTag("P" + notUsedId + "_Icon");
                if (notUsedPlayerIcon != null)
                {
                    var notUsedIcon = notUsedPlayerIcon.GetComponent<PlayerIcon>();

                    if (notUsedIcon != null)
                    {
                        notUsedIcon.HiddenIcon();
                    }
                    //notUsedPlayerIcon.SetActive(false);
                }
                if (this.playerControllers[i] != null) {
                    SetUI.Set(this.playerControllers[i].playerGameBroadCanvas, false);
                    this.playerControllers[i].gameObject.SetActive(false);
                }

            }
        }
    }
}
