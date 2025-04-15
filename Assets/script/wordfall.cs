using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class wordfall : UserData
{
    public CharacterSet characterSet;
    public Scoring scoring;
    public CanvasGroup playerGameBroadCanvas;
    public RectTransform playerGameBroad;
    private RectTransform rectTransform;
    public float fallSpeed = 50f;
    public int missingLetterCount = 0;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI displayText;
    public string hiddenWord;
    private randomTextBtn randomTextBtn;
    public string originalWord;
    public string playerAnswer;
    public int selectedQAIndex;

    [SerializeField] public List<int> unusedIndices;
    public PlayerData playerData = new PlayerData { items = new List<playerQuestions>() };
    public string randomQuestion;
    public string randomAnswer;
    public string targetLetters;

    public void Init(CharacterSet characterSet = null)
    {
        this.fallSpeed = LoaderConfig.Instance ? LoaderConfig.Instance.gameSetup.wordFallingSpeed : this.fallSpeed;
        this.GetQuestionAnswer();
        this.characterSet = characterSet;
        LogController.Instance.debug("Get all questions");

        if (this.PlayerIcons[0] == null)
        {
            this.PlayerIcons[0] = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "_Icon").GetComponent<PlayerIcon>();
        }

        if (this.scoring.scoreTxt == null)
        {
            this.scoring.scoreTxt = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "_Score").GetComponent<TextMeshProUGUI>();
        }

        if (this.playerGameBroadCanvas == null)
        {
            this.playerGameBroadCanvas = GameObject.FindGameObjectWithTag("P" + this.RealUserId + "-controller").GetComponent<CanvasGroup>();
        }

        this.scoring.init();
        this.rectTransform = this.targetText.GetComponent<RectTransform>();
        this.randomTextBtn = GetComponent<randomTextBtn>();

        this.unusedIndices = Enumerable.Range(0, this.playerData.items.Count).ToList();
        this.ExtractRandomWord();
    }

    public void updatePlayerIcon(bool _status = false, string _playerName = "", Sprite _icon = null)
    {
        for (int i = 0; i < this.PlayerIcons.Length; i++)
        {
            if (this.PlayerIcons[i] != null)
            {
                this.PlayerColor = this.characterSet.playerColor;
                this.PlayerIcons[i].playerColor = this.characterSet.playerColor;
                this.PlayerIcons[i].SetStatus(_status, _playerName, _icon);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.playing && this.gameObject.activeInHierarchy)
        {
            this.rectTransform.anchoredPosition -= new Vector2(0, this.fallSpeed * Time.deltaTime);
            if (rectTransform.anchoredPosition.y < -this.playerGameBroad.rect.yMax)
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
                this.playerData.items.Add(new playerQuestions
                {
                    qNum = questions.id,
                    qid = questions.qid,
                    question = questions.question,
                    answer = questions.correctAnswer,
                    score = questions.score.full == 0 ? 10 : questions.score.full,
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
                GameController.Instance.EndGame();
            }
            else
            {
                this.unusedIndices = Enumerable.Range(0, playerData.items.Count).ToList();
                LogController.Instance?.debug("Refill the list when all indices have been used");
            }           
        }
        if (this.unusedIndices.Count > 0)
        {
            if (LoaderConfig.Instance.apiManager.IsLogined && UserId == 0)
            {
                GameController.Instance.progressBar.GetComponentInChildren<NumberCounter>().Unit = "/"+ playerData.items.Count;    
                
                float progress = playerData.items.Count- this.unusedIndices.Count;
                GameController.Instance.progressBar.GetComponentInChildren<NumberCounter>().Value = (int)progress;
                if (progress != 0)
                {
                    GameController.Instance.progressBarImage.fillAmount = progress/playerData.items.Count;
                }
                
            }
            int randomIndex = UnityEngine.Random.Range(0, this.unusedIndices.Count);
            this.selectedQAIndex = this.unusedIndices[randomIndex];

            LogController.Instance?.debug("randomIndex:" + randomIndex);
            LogController.Instance?.debug("selectedIndex:" + this.selectedQAIndex);

            randomQuestion = playerData.items[this.selectedQAIndex].question;
            randomAnswer = playerData.items[this.selectedQAIndex].answer;

            this.unusedIndices.RemoveAt(randomIndex);
            this.targetLetters = "";
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
            this.targetText.text = randomQuestion;
        }


        float textWidth = rectTransform.sizeDelta.x;
        float randomXPosition = UnityEngine.Random.Range(this.playerGameBroad.rect.xMin + textWidth / 2, this.playerGameBroad.rect.xMax - textWidth / 2);
        float panelTop = this.playerGameBroad.rect.yMax + 100f;

        this.rectTransform.anchoredPosition = new Vector2(randomXPosition, panelTop);
    }

    private int GetCurrentTimePercentage()
    {
        var gameTimer = GameController.Instance.gameTimer;
        return Mathf.FloorToInt(((gameTimer.gameDuration - gameTimer.currentTime) / gameTimer.gameDuration) * 100);
    }

    public void AddScore(string playerAnswer)
    {
        var loader = LoaderConfig.Instance;
        int eachQAScore = this.playerData.items[this.selectedQAIndex].score;
        string correctAnswer = this.playerData.items[this.selectedQAIndex].answer;
        int currentScore = this.Score;
        int resultScore = this.scoring.score(playerAnswer, currentScore, correctAnswer, eachQAScore);
        this.Score = resultScore;

        if (this.UserId == 0 && loader != null && loader.apiManager.IsLogined)
        {
            if (this.CorrectedAnswerNumber < QuestionManager.Instance.totalItems)
                this.CorrectedAnswerNumber += 1;

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
            correctAnswer,
            eachQAScore,
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


