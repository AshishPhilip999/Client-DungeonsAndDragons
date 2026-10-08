using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public float speed {
        get
        {
            return animator.speed;
        }

        set
        {
            animator.speed = value;
        }
    }

    private string walkAnimation = "isWalking";
    private string flipWalkAnimation = "isFlipped";

    public void walk(bool isFlipped)
    {
        animator.SetBool(flipWalkAnimation, isFlipped);
        animator.SetBool(walkAnimation, true);
    }

    public void idle()
    {
        animator.SetBool(walkAnimation, false);
    }
}
