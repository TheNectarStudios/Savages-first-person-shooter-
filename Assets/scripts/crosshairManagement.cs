using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField] private GameObject gun;       // Reference to the gun GameObject
    [SerializeField] private GameObject crosshair; // Reference to the crosshair GameObject

    private void Update()
    {
        if (gun != null && crosshair != null)
        {
            // Check if the gun is active
            crosshair.SetActive(gun.activeSelf);
        }
    }
}
