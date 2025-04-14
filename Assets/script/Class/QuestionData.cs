using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestionDataWrapper
{
    public QuestionList[] QuestionDataArray;
}

[Serializable]
public class QuestionData
{

    public List<QuestionList> questions;
}
[Serializable]
public class QuestionList
{
    public int id;
    public string qid;
    public string question;
    public string questionType;
    public string[] answers;
    public string correctAnswer;
    public int star;
    public score score;
    public int correctAnswerIndex;
    public int maxScore;
    public learningObjective learningObjective;
    public string[] media;
    public Texture texture;
    public AudioClip audioClip;
}
[Serializable]
public class score
{
    public int full;
    public int n;
    public int unit;
}
[Serializable]
public class learningObjective
{

}

public enum QuestionType
{
    None = 0,
    Text = 1,
    Picture = 2,
    Audio = 3,
    FillInBlank = 4
}