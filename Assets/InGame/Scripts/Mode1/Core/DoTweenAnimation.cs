using DG.Tweening;
using UnityEngine;

public class DoTweenAnimation : MonoBehaviour
{
    /// <summary>
    /// Spawns a text prefab in the center of the screen and animates it.
    /// </summary>
    /// <param name="textPrefab">The text prefab to spawn.</param>
    /// <param name="textContent">The content of the text.</param>
    /// <param name="duration">Duration of the animation.</param>
    /// <param name="endScale">The final scale of the text after the animation.</param>
    public static void AnimatePopUpText(GameObject textPrefab, string textContent, float duration = 1f, float endScale = 1.5f)
    {
        if (textPrefab == null)
        {
            Debug.LogError("Text prefab is not assigned.");
            return;
        }

        // Spawn the text prefab in the center of the screen
        GameObject textInstance = Object.Instantiate(textPrefab, Vector3.zero, Quaternion.identity);

        // Set the parent of the text instance to the Canvas (ensure the prefab uses RectTransform)
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            textInstance.transform.SetParent(canvas.transform, false);
        }
        else
        {
            Debug.LogError("No Canvas found in the scene. Ensure a Canvas exists.");
            return;
        }

        // Set initial properties of the text
        RectTransform textTransform = textInstance.GetComponent<RectTransform>();
        textTransform.anchoredPosition = Vector2.zero; // Center the text in the Canvas

        // Update the text content
        TMPro.TextMeshProUGUI textComponent = textInstance.GetComponent<TMPro.TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = textContent;
        }
        else
        {
            Debug.LogWarning("Text prefab is missing a TextMeshProUGUI component.");
        }

        // Play DoTween animation (scale up and fade out)
        Sequence sequence = DOTween.Sequence();
        sequence.Append(textTransform.DOScale(endScale, duration / 2).SetEase(Ease.OutBack))
                .Append(textTransform.DOScale(0f, duration / 2).SetEase(Ease.InBack))
                .Join(textInstance.GetComponent<CanvasGroup>().DOFade(0f, duration / 2))
                .OnComplete(() =>
                {
                    Object.Destroy(textInstance); // Destroy after animation
                });

        sequence.Play();
    }

    /// <summary>
    /// Displays floating damage text above a target object.
    /// </summary>
    /// <param name="textPrefab">The text prefab to spawn.</param>
    /// <param name="damageAmount">The damage amount to display.</param>
    /// <param name="target">The target object to display the damage above.</param>
    /// <param name="offset">The vertical offset from the target's position.</param>
    /// <param name="duration">Duration of the floating animation.</param>
    public static void AnimateFloatingText(GameObject textPrefab, Vector3 position, string textContent, float floatDistance = 1f, float duration = 1f)
    {
        if (textPrefab == null)
        {
            Debug.LogError("Text prefab is not assigned.");
            return;
        }

        // Instantiate the text prefab at the given position
        GameObject textInstance = Object.Instantiate(textPrefab, position, Quaternion.identity);

        // Update the text content
        TMPro.TextMeshPro textComponent = textInstance.GetComponent<TMPro.TextMeshPro>();
        if (textComponent != null)
        {
            textComponent.text = textContent;
        }
        else
        {
            Debug.LogWarning("Text prefab is missing a TextMeshPro component.");
        }

        // Animate the text to float upwards and fade out
        Transform textTransform = textInstance.transform;
        Vector3 targetPosition = position + Vector3.up * floatDistance; // Move upward

        Sequence sequence = DOTween.Sequence();
        sequence.Append(textTransform.DOMove(targetPosition, duration).SetEase(Ease.OutQuad)) // Float upward
                .Join(textComponent.DOFade(0, duration)) // Fade out
                .OnComplete(() =>
                {
                    Object.Destroy(textInstance); // Destroy after animation
                });

        sequence.Play();
    }
}
