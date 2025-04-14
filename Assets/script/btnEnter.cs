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
                this.score += 1;
            }

            this.playerScore.text = this.score.ToString();

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
