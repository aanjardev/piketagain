using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using StarterAssets;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 600f;
    public TMP_Text timerText;

    private bool timeEnded = false;
    private int lastSecond = -1;
    private float elapsedTime = 0f;

    void Start()
    {
        if (timeRemaining <= 0f)
        {
            Debug.LogWarning("Timer mulai dengan nilai 0 atau negatif. Mengatur default 600 detik.");
            timeRemaining = 600f;
        }

        DisplayTime(timeRemaining);
    }

    void Update()
    {
        if (timeEnded) return;

        elapsedTime += Time.deltaTime;
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            DisplayTime(timeRemaining);
            Debug.Log("Waktu habis!");
            timeEnded = true;

            // Mainkan bell sound saat waktu habis hanya jika game sudah berjalan lebih dari 0.1 detik.
            if (elapsedTime > 0.1f)
                AudioLibrary.Instance?.PlayBellSchool(transform.position);

            StarterAssets.FirstPersonController.SetPauseState(true);
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