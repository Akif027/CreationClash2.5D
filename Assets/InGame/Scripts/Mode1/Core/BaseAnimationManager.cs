using System.Collections.Generic;
using UnityEngine;

public static class BaseAnimationManager
{
    // Dictionary to map AnimationType to animator parameter names
    private static Dictionary<AnimationType, string> animationMapping;

    /// <summary>
    /// Initializes the animation mappings (AnimationType → Animator parameter).
    /// </summary>
    public static void InitializeAnimationMappings()
    {
        animationMapping = new Dictionary<AnimationType, string>
        {
            { AnimationType.Idle, "Idle" },
            { AnimationType.Catch, "Catch" },
            { AnimationType.Throw, "Throw" },
            { AnimationType.BodyHit, "BodyHit" },
            { AnimationType.HeadHit, "HeadHit" },
   { AnimationType.Dead, "Dead" }
        };
    }

    /// <summary>
    /// Plays an animation based on the AnimationType.
    /// </summary>
    /// <param name="animator">The Animator component.</param>
    /// <param name="animationType">The AnimationType to play.</param>
    public static void PlayAnimation(Animator animator, AnimationType animationType)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null. Cannot play animation.");
            return;
        }

        // Ensure the mapping is initialized
        if (animationMapping == null)
        {
            InitializeAnimationMappings();
        }

        // Find the parameter name from the mapping and trigger the animation
        if (animationMapping.TryGetValue(animationType, out string parameterName))
        {
            animator.SetTrigger(parameterName);
        }
        else
        {
            Debug.LogWarning($"AnimationType '{animationType}' not found in mapping.");
        }
    }

    /// <summary>
    /// Resets all triggers for the given Animator.
    /// </summary>
    /// <param name="animator">The Animator component.</param>
    public static void ResetAllAnimations(Animator animator)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null. Cannot reset animations.");
            return;
        }

        foreach (var parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(parameter.name);
            }
        }
    }
}
public enum AnimationType
{
    Idle,
    Catch,
    Throw,
    Dead,
    BodyHit,
    HeadHit

}