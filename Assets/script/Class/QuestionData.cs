using System;
using System.Collections;
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
    public string correctAnswer;
    public string questionType;
    public Texture texture;
    public AudioClip audioClip;
    public score score;
    public int star;
    public learningObjective learningObjective;
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