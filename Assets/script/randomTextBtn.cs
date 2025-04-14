using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class randomTextBtn : MonoBehaviour
{
    public List<Button> letterButtons;
    public TextMeshProUGUI targetText;

    private List<char> targetLetters;

    public void UpdateTargetLetters(string target)
    {
        /*string targetWord = target.ToUpper();*/
        string targetWord = target;

        /*targetLetters = new List<char>(targetWord.ToUpper().ToCharArray());*/
        targetLetters = new List<char>(targetWord.ToCharArray());
        targetLetters = targetLetters.Distinct().ToList();
        Debug.Log("randomTextBtn.UpdateTargetLetters: " + string.Join("", targetLetters));
    }


    public void FillRandomLetters()
    {
        int totalLetterCount = letterButtons.Count;

        // Remove duplicate letters from targetLetters
        HashSet<char> uniqueTargetLetters = new HashSet<char>(targetLetters);
        List<char> allLetters = new List<char>(uniqueTargetLetters);

        while (allLetters.Count < totalLetterCount)
        {
            char randomLetter;
            do
            {
                randomLetter = (char)UnityEngine.Random.Range('A', 'z' + 1);
            } while (!char.IsLetter(randomLetter) || char.IsWhiteSpace(randomLetter) || allLetters.Contains(randomLetter));

            allLetters.Add(randomLetter);
        }

        // Shuffle all letters
        for (int i = 0; i < allLetters.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, allLetters.Count);
            char temp = allLetters[i];
            allLetters[i] = allLetters[randomIndex];
            allLetters[randomIndex] = temp;
        }

        // Assign letters to buttons
        for (int i = 0; i < totalLetterCount; i++)
        {
            letterButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = allLetters[i].ToString();
        }
    }


}
