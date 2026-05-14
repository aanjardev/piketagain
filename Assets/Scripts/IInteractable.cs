// =============================================================================
//  INTERFACES  (put in their own files or keep here — both work in Unity)
// =============================================================================

/// <summary>Every interactable item in the classroom implements this.</summary>
public interface IInteractable
{
    /// <summary>Called when the player's crosshair lands on this item.</summary>
    void OnLookAt();

    /// <summary>Called when the player's crosshair leaves this item.</summary>
    void OnLookAway();

    /// <summary>The hint text displayed in the UI prompt, e.g. "[E] Pick up Book".</summary>
    string GetPromptText();

    /// <summary>Called when the player presses the interact key.</summary>
    void Interact(UnityEngine.GameObject player);
}

/// <summary>Implemented by items the player can physically carry.</summary>
public interface IPickupable
{
    /// <summary>Returns true if the item should be moved to the holdPoint this frame.</summary>
    bool WantsPickup();
}