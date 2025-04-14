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

    public int correctCount = 0;


    public string playerAnswer;
    //private SoundEffectManager soundEffectManager;
    public int selectedIndex;

    public GameObject playerScoretxt;
    public float answerTime;

    [SerializeField] public List<int> unusedIndices;
    public PlayerData playerData = new PlayerData { items = new List<playerQuestions>() };

    public GameObject playerName;
    public Image playerIcon;
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
        /*SetRandomWord();*/
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
        //if (LoaderConfig.Instance.questionData.questions != null)
        if (QuestionManager.Instance.questionData.questions != null)
        {

            //foreach (var questions in LoaderConfig.Instance.questionData.questions)
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


    public string randomQuestion;
    public string randomAnswer;
    public string targetLetters;
    public void ExtractRandomWord()
    {

        if (unusedIndices.Count == 0)
        {
            if(LoaderConfig.Instance.apiManager.IsLogined && UserId == 0)
            {
                GameManager.Instance.EndGame();
            }
            else
            {
                // Refill the list when all indices have been used
            unusedIndices = Enumerable.Range(0, playerData.items.Count).ToList();
            LogController.Instance?.debug("Refill the list when all indices have been used");
            }

            
        }
        if (unusedIndices.Count > 0)
        {
            if (LoaderConfig.Instance.apiManager.IsLogined && UserId == 0)
            {
                GameManager.Instance.progressBar.GetComponentInChildren<NumberCounter>().Unit = "/"+ playerData.items.Count;    
                
                float progress = playerData.items.Count- unusedIndices.Count;
                GameManager.Instance.progressBar.GetComponentInChildren<NumberCounter>().Value = (int)progress;
                if (progress != 0)
                {
                    GameManager.Instance.progressBarImage.fillAmount = progress/playerData.items.Count;
                }
                
            }
            int randomIndex = UnityEngine.Random.Range(0, unusedIndices.Count);
            selectedIndex = unusedIndices[randomIndex];

            LogController.Instance?.debug("randomIndex:" + randomIndex);
            LogController.Instance?.debug("selectedIndex:" + selectedIndex);

            randomQuestion = playerData.items[selectedIndex].question;
            randomAnswer = playerData.items[selectedIndex].answer;

            unusedIndices.RemoveAt(randomIndex);
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
        correctCount++;
        int newScore = this.playerData.items[selectedIndex].score + this.Score;
        //GameManager.Instance.endGamePage.scoreEndings[UserId].totalScore = newScore;
        //GameManager.Instance.endGamePage.updateFinalScore(userId,newScore);

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

            answerTime = duration + answerTime;

            LoaderConfig.Instance.SubmitAnswer(
            Mathf.RoundToInt(duration),
            this.correctCount,
            statePrecentage,
            progress,
            correctAnswerID,
            answerTime,
            playerData.items[selectedIndex].qid,
            playerData.items[selectedIndex].qNum,
            playerAnswer,
            playerData.items[selectedIndex].answer,
            playerData.items[selectedIndex].score,
            correctCount / this.playerData.items.Count
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


