using UnityEngine;

public class DoorEvent : MonoBehaviour
{
    public Timer timer;

    public DoorNPCInteraction[] doors;

    public AudioSource knockSound;

    private bool triggered = false;

    void Update()
    {
        // sisa 7 menit
        if (!triggered && timer.timeRemaining <= 590f)
        {
            TriggerDoorEvent();
        }
    }

    void TriggerDoorEvent()
    {
        triggered = true;

        foreach (DoorNPCInteraction door in doors)
        {
            door.EnableInteraction();
        }

        if (knockSound != null)
            knockSound.Play();

        Debug.Log("TOK TOK TOK");
    }
}