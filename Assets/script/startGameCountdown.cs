using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections.Generic;

public class startGameCountdown : MonoBehaviour
{
    public float countdownDuration = 4f; // Start game timer
    private float currentTime;
    private int CountdownNum;
    public Sprite[] NumberSprites;
    public Image CountDown;

    public void setCountDown(bool status, int id)
    {

        if (this.CountDown != null)
        {
            this.CountDown.GetComponent<Image>().color = new Color(1, 1, 1, 0);

            
            this.CountDown.GetComponent<Transform>().localScale = Vector3.one;
            if (status)
            {
                this.CountDown.GetComponent<Image>().DOFade(1f, 0.5f);
                this.CountDown.GetComponent<Transform>().DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f);
            }
            this.CountDown.sprite = id == -1 ? null : this.NumberSprites[id];
            this.CountDown.SetNativeSize();
        }
    }

    public void Awake()
    {
        //GameManager.Instance.updatePlayerIcon();

    }
    void Start()
    {
        this.currentTime = countdownDuration;
        CountdownNum = Mathf.CeilToInt(this.currentTime);
        ShowCountdownSprite(CountdownNum);
        playCountdownAudio(CountdownNum);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameStarted && GameManager.Instance.currentTime != 0)
        {
            if (this.currentTime > 0f && this.currentTime != 0)
            {
                this.currentTime -= Time.deltaTime;
                int currentCountdownNum = Mathf.CeilToInt(currentTime);

                if (currentCountdownNum != CountdownNum)
                {
                    CountdownNum = currentCountdownNum;
                    ShowCountdownSprite(CountdownNum);
                    playCountdownAudio(CountdownNum);
                }
            }
            else
            {

                this.currentTime = 0f;
                /* playerControls[0].GetComponent<CanvasGroup>().DOFade(1f, 1f);
                 playerControls[1].GetComponent<CanvasGroup>().DOFade(1f, 1f);*/
                GameManager.Instance.StartGame();



            }
        }
    }


    private void playCountdownAudio(int number)
    {
        if (number > 1)
        {
            AudioController.Instance.PlayAudio(4);
        }
        else if (number == 1)
        {
            AudioController.Instance.PlayAudio(5);
        }
    }

    private void ShowCountdownSprite(int number)
    {
        if (number > 0 && number <= NumberSprites.Length)
        {
            this.setCountDown(true, number - 1);
        }
        else
        {
            this.setCountDown(false, -1);
        }
    }

}
