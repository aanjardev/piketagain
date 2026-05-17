using UnityEngine;
using StarterAssets;

public class DoorEvent : MonoBehaviour
{
    public Timer timer;

    public DoorNPCInteraction[] doors;

    public AudioSource knockSound;

    private bool triggered = false;
    private bool _knockPausedBecausePause = false;

    void Update()
    {
        // sisa 7 menit
        if (!triggered && timer.timeRemaining <= 420f)
        {
            TriggerDoorEvent();
        }

        HandleKnockPauseState();
    }

    void TriggerDoorEvent()
    {
        triggered = true;

        foreach (DoorNPCInteraction door in doors)
        {
            door.EnableInteraction();
        }

        if (knockSound != null)
        {
            knockSound.Play();
            _knockPausedBecausePause = false;
        }

        Debug.Log("TOK TOK TOK");
    }

    void HandleKnockPauseState()
    {
        if (knockSound == null)
            return;

        if (FirstPersonController.IsPaused)
        {
            if (knockSound.isPlaying)
            {
                knockSound.Pause();
                _knockPausedBecausePause = true;
            }
        }
        else if (_knockPausedBecausePause)
        {
            knockSound.UnPause();
            _knockPausedBecausePause = false;
        }
    }
}