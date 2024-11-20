using UnityEngine;
using TMPro; // For TextMeshPro
public class FiringMechanismWithReload : MonoBehaviour
{
    public Transform firingPoint; // Assign this in the Inspector
    public GameObject projectilePrefab; // Assign this in the Inspector
    public float projectileSpeed = 20f;

    [Header("Ammo Settings")]
    public int magazineSize = 10;
    public int totalBullets = 30;
    public float reloadTime = 2f;
    public float fireRate = 0.5f; // Time (in seconds) between consecutive shots

    [Header("Audio Settings")]
    public AudioClip fireSound; // Assign the firing sound in the Inspector
    public AudioClip reloadSound; // Assign the reloading sound in the Inspector
    private AudioSource audioSource; // To play the audio clips

    [Header("UI Element Names")]
    public string chamberBulletsTextName = "ChamberBulletsText"; // Name of the TMP text object in the scene
    public string totalBulletsTextName = "TotalBulletsText"; // Name of the TMP text object in the scene

    private TMP_Text chamberBulletsText; // TMP Text for bullets in the chamber
    private TMP_Text totalBulletsText; // TMP Text for total bullets

    private int bulletsInChamber; // Current bullets in the magazine
    private bool isReloading = false;
    private bool canFire = true; // To manage fire rate

    public Animator gunAnimator; // Assign this in the Inspector

    void Start()
    {
        bulletsInChamber = magazineSize;

        // Initialize TMP text fields dynamically by name
        chamberBulletsText = GameObject.Find(chamberBulletsTextName)?.GetComponent<TMP_Text>();
        totalBulletsText = GameObject.Find(totalBulletsTextName)?.GetComponent<TMP_Text>();

        if (chamberBulletsText == null || totalBulletsText == null)
        {
            Debug.LogError("Failed to find TMP text objects. Ensure their names match the specified names.");
        }

        // Initialize the audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateAmmoUI();
    }

    void Update()
    {
        // Fire on left mouse button click
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            Fire();
        }

        // Reload on pressing 'R'
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReloading();
        }
    }

    public void Fire()
    {
        if (isReloading)
        {
            Debug.Log("Reloading, cannot fire.");
            return;
        }

        if (bulletsInChamber > 0)
        {
            // Prevent firing again until the fire rate interval has passed
            canFire = false;
            Invoke(nameof(ResetFire), fireRate);

            // Trigger the shooting animation
            gunAnimator.SetTrigger("Fire");

            // Fire the projectile
            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firingPoint.forward * projectileSpeed;
            }

            bulletsInChamber--;
            UpdateAmmoUI();

            // Play the firing sound
            PlaySound(fireSound);
        }
        else
        {
            Debug.Log("No bullets in the chamber. Reload required.");
        }
    }

    private void ResetFire()
    {
        // Reset the fire trigger and allow firing again
        gunAnimator.ResetTrigger("Fire");
        canFire = true;
    }

    public void StartReloading()
    {
        if (isReloading || bulletsInChamber == magazineSize || totalBullets <= 0)
        {
            return;
        }

        isReloading = true;
        Debug.Log("Reloading...");
        PlaySound(reloadSound);
        Invoke(nameof(Reload), reloadTime);
    }

    private void Reload()
    {
        int bulletsToReload = magazineSize - bulletsInChamber;

        if (totalBullets >= bulletsToReload)
        {
            bulletsInChamber += bulletsToReload;
            totalBullets -= bulletsToReload;
        }
        else
        {
            bulletsInChamber += totalBullets;
            totalBullets = 0;
        }

        isReloading = false;
        UpdateAmmoUI();
        Debug.Log("Reload complete.");
    }

    private void UpdateAmmoUI()
    {
        if (chamberBulletsText != null)
        {
            chamberBulletsText.text = bulletsInChamber.ToString();
        }

        if (totalBulletsText != null)
        {
            totalBulletsText.text = totalBullets.ToString();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
