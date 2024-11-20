using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Target Image Background")]
    public RectTransform targetImage; // Assign the RectTransform of the button's background image.

    [Header("Transition Settings")]
    public float hoverWidth = 300f; // The width to expand to on hover.
    public float transitionDuration = 0.5f; // Time taken to transition.

    [Header("Audio Settings")]
    public AudioSource audioSource; // AudioSource component for playing sounds.
    public AudioClip hoverSound; // Sound to play on hover.
    public AudioClip clickSound; // Sound to play on click.

    private float originalWidth; // The original width of the image.
    private Coroutine currentCoroutine;

    private void Start()
    {
        // Store the original width of the background image.
        if (targetImage != null)
        {
            originalWidth = targetImage.sizeDelta.x;
        }
        else
        {
            Debug.LogError("No target image assigned for hover effect.");
        }

        // Check if an AudioSource is assigned.
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource assigned for button sounds.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        // Play hover sound.
        PlaySound(hoverSound);

        // Start expanding the width smoothly.
        currentCoroutine = StartCoroutine(SmoothTransition(targetImage.sizeDelta.x, hoverWidth));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        // Revert to the original width smoothly.
        currentCoroutine = StartCoroutine(SmoothTransition(targetImage.sizeDelta.x, originalWidth));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Play click sound.
        PlaySound(clickSound);
    }

    private IEnumerator SmoothTransition(float startWidth, float targetWidth)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float newWidth = Mathf.Lerp(startWidth, targetWidth, elapsedTime / transitionDuration);
            targetImage.sizeDelta = new Vector2(newWidth, targetImage.sizeDelta.y);
            yield return null;
        }

        // Ensure the final size is exactly the target size.
        targetImage.sizeDelta = new Vector2(targetWidth, targetImage.sizeDelta.y);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
