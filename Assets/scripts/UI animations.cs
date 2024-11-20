using UnityEngine;

public class SmoothUISlideInEffect : MonoBehaviour
{
    [Header("Animation Settings")]
    public Vector2 startOffset = new Vector2(-500f, 0f); // Starting offset from the original position.
    public float slideDuration = 1f; // Time taken for the animation to complete.
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Default curve for smooth transitions.

    private RectTransform rectTransform; // The RectTransform of the UI element.
    private Vector2 originalPosition; // Original position of the UI element.

    private void Awake()
    {
        // Get the RectTransform component.
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("No RectTransform component found on the UI element.");
            return;
        }

        // Store the original position.
        originalPosition = rectTransform.anchoredPosition;

        // Set the starting position with the offset.
        rectTransform.anchoredPosition = originalPosition + startOffset;
    }

    private void Start()
    {
        // Start sliding animation.
        StartCoroutine(SlideToOriginalPosition());
    }

    private System.Collections.IEnumerator SlideToOriginalPosition()
    {
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / slideDuration;

            // Use the animation curve to calculate the eased progress.
            float curveValue = slideCurve.Evaluate(progress);

            // Interpolate the position using the curve value.
            rectTransform.anchoredPosition = Vector2.Lerp(originalPosition + startOffset, originalPosition, curveValue);

            yield return null; // Wait for the next frame.
        }

        // Ensure the final position is exactly the original position.
        rectTransform.anchoredPosition = originalPosition;
    }
}
