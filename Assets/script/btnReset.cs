using System.Linq;
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
        this.button.onClick.AddListener(BtnBackSpace);
    }

    public void BtnReset()
    {
        this.wordfall.targetText.text = wordfall.hiddenWord;
        this.textMeshProUGUI.text = "";
    }

    public void BtnBackSpace()
    {
        string Question = wordfall.randomQuestion;
        
        string Answer = Regex.Replace(this.wordfall.targetText.text, @"<color=.*?>|</color>", "");
        if (string.IsNullOrEmpty(Question) || string.IsNullOrEmpty(Answer))
        {
            LogController.Instance.debug("Question or Answer is null or empty.");
            return;
        }

        if (Question.Length != Answer.Length)
        {
            LogController.Instance.debug("Question and Answer lengths do not match: " + Question + "////" + Answer);
            return;
        }

        int underscoreCountInQuestion = Question.Count(ch => ch == '_');
        int currentUnderscoreCount = Answer.Count(ch => ch == '_');
        // 找到玩家答案中的第一個下劃線的位置
        int firstUnderscoreIndex = Answer.IndexOf('_');

        if (firstUnderscoreIndex != -1 && currentUnderscoreCount < underscoreCountInQuestion)
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
                    Answer = Answer.Substring(0, i) + Question[i] + Answer.Substring(i + 1);
                    break;
                }
            }

            string replacementText = Answer;
            this.wordfall.playerAnswer = replacementText;
            this.wordfall.targetText.text = replacementText;
            this.textMeshProUGUI.text = replacementText;
        }
        else
        {
            LogController.Instance.debug("No underscore found in the Answer.");
        }
    }
}
