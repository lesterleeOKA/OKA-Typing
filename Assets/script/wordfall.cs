using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class wordfall : UserData
{
    public GameObject playerGameBroad;
    public RectTransform playerGameBoardRectTransform;
    public float fallSpeed = 50f;
    public int missingLetterCount = 0;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI displayText;
    public string hiddenWord;
    private RectTransform rectTransform;
    private randomTextBtn randomTextBtn;
    public string originalWord;
    public string playerAnswer;
    public int selectedQAIndex;

    public GameObject playerScoretxt;

    [SerializeField] public List<int> unusedIndices;
    public PlayerData playerData = new PlayerData { items = new List<playerQuestions>() };
    public GameObject playerName;
    public Image playerIcon;
    public string randomQuestion;
    public string randomAnswer;
    public string targetLetters;
    void Awake()
    {
        GetQuestionAnswer();
    }
    void Start()
    {
        if (LoaderConfig.Instance.apiManager.IsLogined )
        {
            if (this.UserId == 0)
            {
                playerName.GetComponentInChildren<TextMeshProUGUI>().text = LoaderConfig.Instance.apiManager.loginName;           
                playerIcon.sprite = SetUI.ConvertTextureToSprite(LoaderConfig.Instance.apiManager.peopleIcon as Texture2D);          
            }
        }
        else
        {
            playerName.GetComponent<CanvasGroup>().alpha = 0f;
        }

        rectTransform = targetText.GetComponent<RectTransform>();

        playerGameBoardRectTransform = playerGameBroad.GetComponent<RectTransform>();
        randomTextBtn = GetComponent<randomTextBtn>();

        unusedIndices = Enumerable.Range(0, playerData.items.Count).ToList();
        ExtractRandomWord();
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance.timesup != true && GameManager.Instance.playing == true)
        {
            rectTransform.anchoredPosition -= new Vector2(0, fallSpeed * Time.deltaTime);
            if (rectTransform.anchoredPosition.y < -playerGameBoardRectTransform.rect.yMax)
            {
                AudioController.Instance.PlayAudio(0);
                ExtractRandomWord();
            }
        }

    }

    public void GetQuestionAnswer()
    {
        if (QuestionManager.Instance.questionData.questions != null)
        {
            foreach (var questions in QuestionManager.Instance.questionData.questions)
            {
                playerData.items.Add(new playerQuestions
                {
                    qNum = questions.id,
                    qid = questions.qid,
                    question = questions.question,
                    answer = questions.correctAnswer,
                    score = questions.score.full,
                });
            }
        }
        else
        {
            LogController.Instance?.debugError("Question List not found!");
        }
    }

    public void ExtractRandomWord()
    {
        this.displayText.GetComponent<TextMeshProUGUI>().text = "";
        if (this.unusedIndices.Count == 0)
        {
            if(LoaderConfig.Instance.apiManager.IsLogined && UserId == 0)
            {
                GameManager.Instance.EndGame();
            }
            else
            {
                // Refill the list when all indices have been used
            this.unusedIndices = Enumerable.Range(0, playerData.items.Count).ToList();
            LogController.Instance?.debug("Refill the list when all indices have been used");
            }

            
        }
        if (this.unusedIndices.Count > 0)
        {
            if (LoaderConfig.Instance.apiManager.IsLogined && UserId == 0)
            {
                GameManager.Instance.progressBar.GetComponentInChildren<NumberCounter>().Unit = "/"+ playerData.items.Count;    
                
                float progress = playerData.items.Count- this.unusedIndices.Count;
                GameManager.Instance.progressBar.GetComponentInChildren<NumberCounter>().Value = (int)progress;
                if (progress != 0)
                {
                    GameManager.Instance.progressBarImage.fillAmount = progress/playerData.items.Count;
                }
                
            }
            int randomIndex = UnityEngine.Random.Range(0, this.unusedIndices.Count);
            this.selectedQAIndex = this.unusedIndices[randomIndex];

            LogController.Instance?.debug("randomIndex:" + randomIndex);
            LogController.Instance?.debug("selectedIndex:" + this.selectedQAIndex);

            randomQuestion = playerData.items[this.selectedQAIndex].question;
            randomAnswer = playerData.items[this.selectedQAIndex].answer;

            this.unusedIndices.RemoveAt(randomIndex);
            targetLetters = "";
            int minLength = Mathf.Min(randomQuestion.Length, randomAnswer.Length);
            for (int i = 0; i < minLength; i++)
            {
                if (randomQuestion[i] != randomAnswer[i])
                {
                    targetLetters += randomAnswer[i];
                }

            }
            randomTextBtn.UpdateTargetLetters(targetLetters);
            randomTextBtn.FillRandomLetters();
            LogController.Instance?.debug($"Random Question: {randomQuestion}//");
            LogController.Instance?.debug($"Corresponding Answer: {randomAnswer}//");
            LogController.Instance?.debug($"Target Letters: {targetLetters} //");
            targetText.text = randomQuestion;
        }


        float textWidth = rectTransform.sizeDelta.x;
        float randomXPosition = UnityEngine.Random.Range(playerGameBoardRectTransform.rect.xMin + textWidth / 2, playerGameBoardRectTransform.rect.xMax - textWidth / 2);
        float panelTop = playerGameBoardRectTransform.rect.yMax + 100f;

        rectTransform.anchoredPosition = new Vector2(randomXPosition, panelTop);
    }

    private int GetCurrentTimePercentage()
    {
        var gameTimer = GameManager.Instance.gameTimer;
        return Mathf.FloorToInt(((gameTimer.gameDuration - gameTimer.currentTime) / gameTimer.gameDuration) * 100);
    }

    public void AddScore(string playerAnswer)
    {
        this.Score = int.Parse(playerScoretxt.GetComponent<TextMeshProUGUI>().text);
        this.CorrectedAnswerNumber++;
        int newScore = this.playerData.items[this.selectedQAIndex].score + this.Score;
        DOTween.To(() => this.Score, x => this.Score = x, newScore, 2f).OnUpdate(() =>
        {
            playerScoretxt.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            playerScoretxt.GetComponent<TextMeshProUGUI>().text = this.Score.ToString();
        }).OnComplete(() =>
        {
            playerScoretxt.GetComponent<TextMeshProUGUI>().color = Color.white;
        });


        if (LoaderConfig.Instance.apiManager.IsLogined && UserId == 0)
        {
            QuestionData data = QuestionManager.Instance.questionData;
            float statePrecentage = (data.questions.Count - unusedIndices.Count) / data.questions.Count * 100f;
            int progress = (int)statePrecentage;
            int correctAnswerID = 2;

            float duration = this.GetCurrentTimePercentage();

            this.AnswerTime = duration + this.AnswerTime;

            LoaderConfig.Instance.SubmitAnswer(
            Mathf.RoundToInt(duration),
            this.Score,
            statePrecentage,
            progress,
            correctAnswerID,
            this.AnswerTime,
            playerData.items[this.selectedQAIndex].qid,
            playerData.items[this.selectedQAIndex].qNum,
            playerAnswer,
            playerData.items[this.selectedQAIndex].answer,
            playerData.items[this.selectedQAIndex].score,
            100f
            );
        }


    }


}
[Serializable]
public class PlayerData
{
    public List<playerQuestions> items;
}
[Serializable]
public class playerQuestions
{
    public int qNum;
    public string qid;
    public string question;
    public string answer;
    public int score;

}


