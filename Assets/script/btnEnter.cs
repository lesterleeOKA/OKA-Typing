using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class btnEnter : MonoBehaviour
{

    public Button button;

    public wordfall wordfall;

    public int score;
    
    public TextMeshProUGUI playerScore;

    private bool isButtonEnabled = true;

    public TextMeshProUGUI playerTextUI;
    
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(SendAnswer);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SendAnswer()
    {
        if (isButtonEnabled)
        {
            /*string originalWord = wordfall.originalWord;*/
            string answer = wordfall.randomAnswer;
            string targetText = Regex.Replace(wordfall.targetText.text, @"<color=.*?>|</color>", "");

            /*if (originalWord.ToUpper() == targetText.ToUpper())*/
            if (answer == targetText)
            {
                score += 1;
                if(GameManager.Instance.endGamePage != null)
                {
                GameManager.Instance.endGamePage.scoreEndings[wordfall.userId].totalScore = this.score;
               // GameManager.Instance.endGamePage.updateFinalScore(wordfall.userId,this.score);
                }
                
            }
            else
            {
                //Debug.Log("originalWord: " + answer);
                //Debug.Log("TargetText: " + targetText);
                //Debug.Log("Wrong answer.");
            }

            playerScore.text = score.ToString();

            /*wordfall.SetRandomWord();*/
            wordfall.ExtractRandomWord();

            button.interactable = false;

            isButtonEnabled = false;

            StartCoroutine(EnableButtonAfterDelay());

            playerTextUI.text = "";

        }
    }

    private IEnumerator EnableButtonAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        button.interactable = true;

        isButtonEnabled = true;
    }

}
