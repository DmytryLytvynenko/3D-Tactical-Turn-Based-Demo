using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetTrigger(CharacterAnimParameters parameter)
    {
        animator.SetTrigger(parameter.ToString());
    }
    public void SetBool(CharacterAnimParameters parameter, bool value)
    {
        animator.SetBool(parameter.ToString(), value);
    }
}

public enum CharacterAnimParameters
{
    Jump,
    RangeAttack,
    Attack,
    BladeSlash,
    Kick,
    Hit,
    Dead,
    Idle
}
