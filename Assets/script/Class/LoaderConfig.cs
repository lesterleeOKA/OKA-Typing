using System;
using UnityEngine;


public class LoaderConfig : GameSetting
{
    public static LoaderConfig Instance = null;
    protected override void Awake()
    {
        if (Instance == null)
            Instance = this;

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        this.LoadGameData();
#endif
    }

    protected override void Update()
    {
        base.Update();
    }

    public void LoadGameData()
    {
        RegisterCustomHandler("wordSpeed", (value) =>
        {
            float wordFallingSpeed = float.Parse(value);
            this.gameSetup.wordFallingSpeed = wordFallingSpeed;
            LogController.Instance?.debug("Word falling speed: " + this.gameSetup.wordFallingSpeed);
        });

        this.apiManager.PostGameSetting(this.GetParseURLParams,
                                        ()=> StartCoroutine(this.apiManager.postGameSetting(this.LoadQuestions)), //success action
                                        this.LoadQuestions
                                        );
    }

    public void LoadQuestions()
    {
        this.InitialGameImages(()=>
        {
            QuestionManager.Instance?.LoadQuestionFile(this.unitKey, () => this.finishedLoadQuestion());
        });    
    }

    void finishedLoadQuestion()
    {
        ExternalCaller.HiddenLoadingBar();
        this.changeScene(1);
    }

    public void SubmitAnswer(int duration, int playerScore, float statePercent, int stateProgress,
                             int correctId, float currentQADuration, string qid, int answerId, string answerText, 
                             string correctAnswerText, float currentQAscore, float currentQAPercent, Action onCompleted = null)
    {
        /*string jsonPayload = $"[{{\"payloads\":{playloads}," +
        $"\"role\":{{\"uid\":{uid}}}," +
        $"\"state\":{{\"duration\":{stateDuration},\"score\":{stateScore},\"percent\":{statePercent},\"progress\":{stateProgress}}}," +
        $"\"currentQuestion\":{{\"correct\":{correct},\"duration\":{currentQADuration},\"qid\":\"{currentqid}\",\"correctAnswer\":{answerId},\"answerText\":\"{answerText}\",\"correctAnswerText\":\"{correctAnswerText}\",\"score\":{currentQAscore},\"percent\":{currentQAPercent}}}}}]";*/

       var answer = this.apiManager.answer;
        answer.state.duration = duration;
        answer.state.score = playerScore;
        answer.state.percent = statePercent;
        answer.state.progress = stateProgress;

        answer.currentQA.correctId = correctId;
        answer.currentQA.duration = currentQADuration;
        answer.currentQA.qid = qid;
        answer.currentQA.answerId = answerId;
        answer.currentQA.answerText = answerText;
        answer.currentQA.correctAnswerText = correctAnswerText;
        answer.currentQA.score = currentQAscore;
        answer.currentQA.percent = currentQAPercent;

        StartCoroutine(this.apiManager.SubmitAnswer(onCompleted));
    }

    public void closeLoginErrorBox()
    {
        this.apiManager.resetLoginErrorBox();
    }

    public void exitPage(string state = "", Action<bool> leavePageWithValue = null, Action leavePageWithoutValue = null)
    {
        bool isLogined = this.apiManager.IsLogined;
        if (isLogined)
        {
            LogController.Instance?.debug($"{state}, called exit api.");
            StartCoroutine(this.apiManager.ExitGameRecord(() =>
            {
                leavePageWithValue?.Invoke(true);
                leavePageWithoutValue?.Invoke();
            }));
        }
        else
        {
            leavePageWithValue?.Invoke(false);
            leavePageWithoutValue?.Invoke();
            LogController.Instance?.debug($"{state}.");
        }
    }

    public void QuitGame()
    {
        this.exitPage("Quit Game", null);
    }

    private void OnApplicationQuit()
    {
        this.QuitGame();
    }

}

