using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using DG.Tweening;


public class btnInputLetter : MonoBehaviour
{
    public wordfall wordfall;
    public bool isAddScoreAnimationPlayed;
    public Color keyboardTextColor;

    public void setKeyboard(GameObject obj)
    {
        if(obj == null) return;
        var loader = LoaderConfig.Instance;
        if (loader.gameSetup.itemTexture != null && loader.apiManager.IsLogined)
        {
            var buttonTexture = loader.gameSetup.itemTexture;
            if (obj.GetComponent<Image>() != null)
            {
                Texture2D tex2d = ResizeTexture(buttonTexture as Texture2D, 150, 150);
                Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0.5f, 0.5f));
                obj.GetComponent<Image>().sprite = sprite;
            }

        }
        if (obj.GetComponentInChildren<TextMeshProUGUI>() != null && loader.gameSetup.keyboardTextColor != default)
        {
            this.keyboardTextColor = loader.gameSetup.keyboardTextColor;
            obj.GetComponentInChildren<TextMeshProUGUI>().color = this.keyboardTextColor;
        }

    }
    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        Texture2D newTexture = new Texture2D(newWidth, newHeight, source.format, false);
        Color[] pixels = source.GetPixels(0, 0, source.width, source.height);
        Color[] newPixels = new Color[newWidth * newHeight];

        float ratioX = (float)source.width / newWidth;
        float ratioY = (float)source.height / newHeight;

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                newPixels[y * newWidth + x] = pixels[(int)(y * ratioY) * source.width + (int)(x * ratioX)];
            }
        }

        newTexture.SetPixels(newPixels);
        newTexture.Apply();
        return newTexture;
    }

    // public EndGamePage endGamePage;

    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(ReplaceButtonText);
        this.setKeyboard(this.gameObject);
       
    }

    private void ReplaceButtonText()
    {
        int firstUnderscoreIndex = wordfall.targetText.text.IndexOf("_");

        if (firstUnderscoreIndex != -1)
        {
            string prefix = wordfall.targetText.text.Substring(0, firstUnderscoreIndex);
            string buttonText = this.GetComponentInChildren<TextMeshProUGUI>().text;
            AudioController.Instance.PlayAudio(0);
            // this game font don't support "␣"
            /*// Check if the button text is a space
            if (buttonText == " ")
            {
                // Replace with a space bar icon (you can customize this part)
                buttonText = "␣"; // Unicode representation for space 
                
            }
            else
            {
                wordfall.playerAnswer = buttonText;
                buttonText = $"<color=#FF0000>{buttonText}</color>";
            }*/
            wordfall.playerAnswer = buttonText;
            buttonText = $"<color=#FF0000>{buttonText}</color>";

            string replacementText = prefix + buttonText + wordfall.targetText.text.Substring(firstUnderscoreIndex + 1);

            wordfall.targetText.text = replacementText;
            wordfall.displayText.text = replacementText;
            if (!wordfall.targetText.text.Contains("_"))
            {
                CheckAnswer();
            }

        }

    }
    public void CheckAnswer()
    {

        /*string originalWord = wordfall.originalWord;*/
        string answer = wordfall.randomAnswer;
        string targetText = Regex.Replace(wordfall.targetText.text, @"<color=.*?>|</color>", "");

        /*if (originalWord.ToUpper() == targetText.ToUpper())*/

        if (answer == targetText)
        {
            CorrectAnswer(answer);
        }
        else
        {
            WrongAnswer(answer);

        }
        LogController.Instance.debug("" + this.wordfall.CorrectedAnswerNumber);
        
        //correctCount.GetComponent<TextMeshProUGUI>().text = this.wordfall.correctCount.ToString();
        wordfall.displayText.GetComponent<TextMeshProUGUI>().DOFade(0, 0.5f).OnComplete(() =>
        {
            wordfall.displayText.GetComponent<TextMeshProUGUI>().text = "";
            wordfall.displayText.GetComponent<TextMeshProUGUI>().DOFade(1, 0.1f);
        });

        this.wordfall.ExtractRandomWord();
    }
    public void CorrectAnswer(string playerAnswer)
    {

        this.wordfall.AddScore(playerAnswer);

        LogController.Instance.debug("Correct");

        AudioController.Instance.PlayAudio(2);
    }

    public void WrongAnswer(string playerAnswer)
    {
        LogController.Instance.debug("Wrong answer.");
        if (LoaderConfig.Instance.apiManager.IsLogined && wordfall.UserId == 0)
        {
            QuestionData data = QuestionManager.Instance.questionData;
            float statePrecentage = (data.questions.Count - wordfall.unusedIndices.Count) / data.questions.Count * 100f;
            int progress = (int)statePrecentage;
            int correctAnswerID = 0;
            float duration = GameManager.Instance.gameTimer.gameDuration - GameManager.Instance.gameTimer.currentTime;
            this.wordfall.AnswerTime = duration + this.wordfall.AnswerTime;
            var seletedItem = this.wordfall.playerData.items[this.wordfall.selectedQAIndex];

            LoaderConfig.Instance.SubmitAnswer(
            Mathf.RoundToInt(duration),
            this.wordfall.Score,
            statePrecentage,
            progress,
            correctAnswerID,
            this.wordfall.AnswerTime,
            seletedItem.qid,
            seletedItem.qNum,
            playerAnswer,
            seletedItem.answer,
            0,
            0f
            );
        }
        AudioController.Instance.PlayAudio(1);
        //wordfall.ExtractRandomWord();

    }

}
