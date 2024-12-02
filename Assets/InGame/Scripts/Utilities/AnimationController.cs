using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] AnimationType CurrentAnimation;
    [SerializeField] private Animator animator; // The Animator component
    [SerializeField] private bool resetTriggersBeforePlay = true; // Whether to reset all triggers before playing
                                                                  // Singleton instance




    /// <summary>
    /// Plays an animation by AnimationType.
    /// </summary>
    /// <param name="animationType">The AnimationType to play.</param>
    public void PlayAnimation(AnimationType animationType)
    {
        if (resetTriggersBeforePlay)
        {
            BaseAnimationManager.ResetAllAnimations(animator);
        }
        CurrentAnimation = animationType;
        BaseAnimationManager.PlayAnimation(animator, animationType);
    }
    // public void Release()
    // {
    //     projectileWeapon = WeaponManager.Instance.GetCurretWeapon();
    //     if (projectileWeapon != null)
    //     {
    //         projectileWeapon.transform.SetParent(null);

    //         projectileWeapon.LaunchWithDelay();
    //     }
    //     else
    //     {

    //         Debug.LogError("No Weapon");
    //     }

    // }


}
