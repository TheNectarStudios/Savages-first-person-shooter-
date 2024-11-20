using UnityEngine;

public class GunAiming : MonoBehaviour
{
    public Animator gunAnimator; // Reference to the Animator component
    private bool isAiming = false;

    void Update()
    {
        // Check if the right mouse button is pressed or released
        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            StartAiming();
        }
        else if (Input.GetMouseButtonUp(1)) // Right mouse button released
        {
            StopAiming();
        }
    }

    void StartAiming()
    {
        isAiming = true;
        gunAnimator.SetBool("isAiming", true); // Trigger the aiming animation
    }

    void StopAiming()
    {
        isAiming = false;
        gunAnimator.SetBool("isAiming", false); // Reverse the aiming animation
    }
}
