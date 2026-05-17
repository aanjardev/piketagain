using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 600f;
    public TMP_Text timerText;

    private bool timeEnded = false;
    private int lastSecond = -1;

    void Update()
    {
        if (timeEnded) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            DisplayTime(timeRemaining);
            Debug.Log("Waktu habis!");
            timeEnded = true;
            SceneManager.LoadScene("LoseScreen");
            return;
        }

        int currentSecond = Mathf.FloorToInt(timeRemaining);

        if (currentSecond != lastSecond)
        {
            lastSecond = currentSecond;
            DisplayTime(timeRemaining);
        }

        if (timeRemaining <= 30 && timerText.color != Color.red)
        {
            timerText.color = Color.red;
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}