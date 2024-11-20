using UnityEngine;

public class DoorControllerWithSound : MonoBehaviour
{
    [Header("Door Movement Settings")]
    public Transform door; // Reference to the door object.
    public Vector3 closedPosition; // The door's initial (closed) position.
    public Vector3 openPosition; // The door's final (open) position.
    public float moveDuration = 1f; // Time taken to open/close the door.

    [Header("Sound Settings")]
    public AudioSource audioSource; // The audio source to play sounds.
    public AudioClip openSound; // Sound to play when the door opens.
    public AudioClip closeSound; // Sound to play when the door closes.

    private bool isOpen = false; // Tracks whether the door is open.
    private bool isMoving = false; // Prevents overlapping animations.

    private void Start()
    {
        // Ensure the door starts at the closed position.
        if (door != null)
        {
            door.position = closedPosition;
        }
        else
        {
            Debug.LogError("Door Transform is not assigned.");
        }

        // Ensure an AudioSource is assigned.
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player triggers the zone and the door is not moving.
        if (other.CompareTag("Player") && !isMoving)
        {
            // Toggle the door's state.
            isOpen = !isOpen;

            // Play the appropriate sound.
            PlaySound(isOpen ? openSound : closeSound);

            // Start moving the door.
            StartCoroutine(MoveDoor(isOpen ? openPosition : closedPosition));
        }
    }

    private System.Collections.IEnumerator MoveDoor(Vector3 targetPosition)
    {
        isMoving = true; // Lock movement during animation.
        float elapsedTime = 0f;
        Vector3 startingPosition = door.position;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            door.position = Vector3.Lerp(startingPosition, targetPosition, t);
            yield return null;
        }

        door.position = targetPosition; // Snap to the final position.
        isMoving = false; // Unlock movement.
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // Play the sound.
        }
    }
}
