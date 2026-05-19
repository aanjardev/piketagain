using System.Collections;
using UnityEngine;
using TMPro;

public class QuestUIController : MonoBehaviour
{
    public static QuestUIController Instance;

    [Header("UI")]
    public GameObject questPanel;

    public TMP_Text titleText;
    public TMP_Text objectiveText;

    void Awake()
    {
        Instance = this;

        if (questPanel != null)
            questPanel.SetActive(false);
    }

    public void ShowQuest(string title, string objective)
    {
        questPanel.SetActive(true);

        titleText.text = title;
        objectiveText.text = objective;
    }

    public void ShowQuestSuccess(string title, string objective, float duration = 3f)
    {
        ShowQuest(title, objective);
        StopAllCoroutines();
        StartCoroutine(HideQuestAfterDelay(duration));
    }

    public void HideQuest()
    {
        StopAllCoroutines();
        questPanel.SetActive(false);
    }

    IEnumerator HideQuestAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideQuest();
    }
}