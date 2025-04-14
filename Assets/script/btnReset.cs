using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class btnReset : MonoBehaviour
{
    public Button button;

    public wordfall wordfall;
    public TextMeshProUGUI textMeshProUGUI;

    void Start()
    {
        /*button.onClick.AddListener(BtnReset);*/
        button.onClick.AddListener(BtnBackSpace);
    }

    public void BtnReset()
    {
        wordfall.targetText.text = wordfall.hiddenWord;
        textMeshProUGUI.text = "";
    }

    public void BtnBackSpace()
    {
        string Question = wordfall.randomQuestion;
        
        string Answer = Regex.Replace(wordfall.targetText.text, @"<color=.*?>|</color>", "");
        if (string.IsNullOrEmpty(Question) || string.IsNullOrEmpty(Answer))
        {
            Debug.LogWarning("Question or Answer is null or empty.");
            return;
        }

        if (Question.Length != Answer.Length)
        {
            Debug.LogWarning("Question and Answer lengths do not match.");
            Debug.Log(Question+"////"+ Answer);
            return;
        }

        // 找到玩家答案中的第一個下劃線的位置
        int firstUnderscoreIndex = Answer.IndexOf('_');

        if (firstUnderscoreIndex != -1)
        {
            // 從後向前遍歷答案，找到最後一個與問題中對應位置不同的字符
            for (int i = Answer.Length - 1; i >= 0; i--)
            {
                if (i >= Question.Length)
                {
                    Debug.LogWarning("Index is outside the bounds of the Question string.");
                    return;
                }

                if (Answer[i] != Question[i])
                {
                    // 將這個字符替換為問題中的下劃線
                    Answer = Answer.Substring(0, i) + Question[i] + Answer.Substring(i + 1);
                    break;
                }
            }

            string replacementText = Answer;

            // 更新 UI 元素
            wordfall.playerAnswer = replacementText;
            wordfall.targetText.text = replacementText;
            textMeshProUGUI.text = replacementText;
        }
        else
        {
            Debug.LogWarning("No underscore found in the Answer.");
        }
    }
}
