using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 600f;

    public TMP_Text timerText;

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            DisplayTime(timeRemaining);

            if(timeRemaining <= 30)
            {
                timerText.color = Color.red;
            }
        }
        else
        {
            timeRemaining = 0;
            Debug.Log("Waktu habis!");
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}