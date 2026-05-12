using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    public CharacterController characterController;

    void Update()
    {
        if (animator == null || characterController == null) return;

        Vector3 velocity = characterController.velocity;
        velocity.y = 0f;

        animator.SetFloat("Speed", velocity.magnitude);
    }
}