using UnityEngine;

public class WeaponAnimatorController : MonoBehaviour
{
    private Animator animator;

    // Animation parameter names
    private static readonly int FireTrigger = Animator.StringToHash("Fire");
    private static readonly int ReloadEmptyTrigger = Animator.StringToHash("Reload Empty");

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        // Fire animation on left mouse click
        if (Input.GetMouseButtonDown(0)) // 0 = Left Mouse Button
        {
            PlayFireAnimation();
        }

        // Reload empty animation on pressing R
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayReloadEmptyAnimation();
        }
    }

    private void PlayFireAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(FireTrigger);
        }
    }

    private void PlayReloadEmptyAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(ReloadEmptyTrigger);
        }
    }
}
