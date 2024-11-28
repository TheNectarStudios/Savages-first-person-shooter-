using UnityEngine;

public class DoorControllerWithSound : MonoBehaviour
{
    [Header("Door Movement Settings")]
    public Transform door; // Reference to the door object.

    [Header("Rotation Settings")]
    public Quaternion closedRotation; // The door's initial (closed) rotation.
    public Quaternion openRotation; // The door's final (open) rotation.

    [Header("Position Settings")]
    public bool changePosition = false; // Toggle whether to change the position.
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
        // Ensure the door starts at the closed position and rotation.
        if (door != null)
        {
            if (changePosition)
            {
                door.position = closedPosition;
            }
            door.rotation = closedRotation;
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
            ToggleDoor(); // Toggle the door state when triggered.
        }
    }

    public void ToggleDoor()
    {
        if (!isMoving)
        {
            // Toggle the state.
            isOpen = !isOpen;

            // Play the appropriate sound.
            PlaySound(isOpen ? openSound : closeSound);

            // Move the door to the target rotation and optionally the position.
            StartCoroutine(MoveDoor(
                isOpen ? openRotation : closedRotation,
                isOpen ? openPosition : closedPosition
            ));
        }
    }

    private System.Collections.IEnumerator MoveDoor(Quaternion targetRotation, Vector3 targetPosition)
    {
        isMoving = true; // Lock movement during animation.
        float elapsedTime = 0f;
        Quaternion startingRotation = door.rotation;
        Vector3 startingPosition = door.position;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            // Smoothly interpolate the rotation.
            door.rotation = Quaternion.Lerp(startingRotation, targetRotation, t);

            // If changePosition is enabled, interpolate the position.
            if (changePosition)
            {
                door.position = Vector3.Lerp(startingPosition, targetPosition, t);
            }

            yield return null;
        }

        // Snap to the final rotation and position.
        door.rotation = targetRotation;
        if (changePosition)
        {
            door.position = targetPosition;
        }

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
