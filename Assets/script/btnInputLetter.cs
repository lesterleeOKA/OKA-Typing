using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Experimental.AI;


public class btnInputLetter : MonoBehaviour
{



    public wordfall wordfall;
    //public GameObject correctCount;
    // public int player_score;

    //public GameObject AddScore;
    public bool isAddScoreAnimationPlayed;

    // public EndGamePage endGamePage;

    void Start()
    {

        if (LoaderConfig.Instance != null)
        {

        }
        else
        {
            Debug.LogWarning("SoundEffectManager not found in the scene.");
        }


        this.GetComponent<Button>().onClick.AddListener(ReplaceButtonText);
        if (LoaderConfig.Instance.apiManager.IsLogined)
        {
            LoaderConfig.Instance.gameSetup.setKeyboard(this.gameObject);
        }
        
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
        Debug.Log(this.wordfall.correctCount);
        
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

        Debug.Log("Correct");

        AudioController.Instance.PlayAudio(2);
    }

    public void WrongAnswer(string playerAnswer)
    {
        Debug.Log("Wrong answer.");
        if (LoaderConfig.Instance.apiManager.IsLogined && wordfall.userId == 0)
        {
            QuestionData data = QuestionManager.Instance.questionData;
            float statePrecentage = (data.questions.Count - wordfall.unusedIndices.Count) / data.questions.Count * 100f;
            int progress = (int)statePrecentage;
            int correctAnswerID = 0;//wrong Answer ID = 0

            float duration = GameManager.Instance.startingTime - GameManager.Instance.currentTime;

            this.wordfall.answerTime = duration + this.wordfall.answerTime;
            var seletedItem = this.wordfall.playerData.items[this.wordfall.selectedIndex];

            LoaderConfig.Instance.SubmitAnswer(
            Mathf.RoundToInt(duration),
            this.wordfall.correctCount,
            statePrecentage,
            progress,
            correctAnswerID,
            this.wordfall.answerTime,
            seletedItem.qid,
            seletedItem.qNum,
            playerAnswer,
            seletedItem.answer,
            0,
            this.wordfall.correctCount / this.wordfall.playerData.items.Count
            );
        }
        AudioController.Instance.PlayAudio(1);
        //wordfall.ExtractRandomWord();

    }

}
