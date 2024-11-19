using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] AnimationType CurrentAnimation;
    [SerializeField] private Animator animator; // The Animator component
    [SerializeField] private bool resetTriggersBeforePlay = true; // Whether to reset all triggers before playing
    // Singleton instance
    public static AnimationController Instance { get; private set; }



    private void Awake()
    {
        // Check if an instance already exists
        if (Instance == null)
        {
            Instance = this;
            //  DontDestroyOnLoad(gameObject); // Ensure the singleton persists across scenes
        }

    }
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
}
