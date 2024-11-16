// Copyright 2021, Infima Games. All Rights Reserved.

using System.Linq;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Movement : MovementBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Audio Clips")]
        [Tooltip("The audio clip that is played while walking.")]
        [SerializeField]
        private AudioClip audioClipWalking;

        [Tooltip("The audio clip that is played while running.")]
        [SerializeField]
        private AudioClip audioClipRunning;

        [Header("Speeds")]
        [SerializeField]
        private float speedWalking = 5.0f;

        [Tooltip("How fast the player moves while running."), SerializeField]
        private float speedRunning = 9.0f;

        [Tooltip("How fast the player moves while crouching."), SerializeField]
        private float speedCrouching = 2.5f;

        [Header("Crouching")]
        [Tooltip("Height of the capsule when crouching.")]
        [SerializeField]
        private float crouchHeight = 1.0f;

        [Tooltip("Normal height of the capsule.")]
        [SerializeField]
        private float normalHeight = 2.0f;

        [Tooltip("Crouch key.")]
        [SerializeField]
        private KeyCode crouchKey = KeyCode.LeftControl;

        [Header("Gravity and Jumping")]
        [Tooltip("Gravity force applied to the player.")]
        [SerializeField]
        private float gravity = -20f;

        [Tooltip("Force applied when the player jumps.")]
        [SerializeField]
        private float jumpForce = 10.0f;

        [Header("Stair Climb")]
        [Tooltip("Maximum height of the step the player can climb.")]
        [SerializeField]
        private float stepHeight = 0.5f;

        [Tooltip("Force applied to move the player up the step.")]
        [SerializeField]
        private float stepSmooth = 0.1f;

        #endregion

        #region PROPERTIES

        private Vector3 Velocity
        {
            get => rigidBody.velocity;
            set => rigidBody.velocity = value;
        }

        #endregion

        #region FIELDS

        private Rigidbody rigidBody;
        private CapsuleCollider capsule;
        private AudioSource audioSource;
        private bool grounded;
        private CharacterBehaviour playerCharacter;
        private WeaponBehaviour equippedWeapon;
        private readonly RaycastHit[] groundHits = new RaycastHit[8];

        private bool isCrouching = false;
        private Vector3 playerVelocity;

        #endregion

        #region UNITY FUNCTIONS

        protected override void Awake()
        {
            playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
        }

        protected override void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            capsule = GetComponent<CapsuleCollider>();
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClipWalking;
            audioSource.loop = true;
        }

        private void OnCollisionStay()
        {
            Bounds bounds = capsule.bounds;
            Vector3 extents = bounds.extents;
            float radius = extents.x - 0.01f;

            Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
                groundHits, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);

            grounded = groundHits.Any(hit => hit.collider != null && hit.collider != capsule);
            for (var i = 0; i < groundHits.Length; i++)
                groundHits[i] = new RaycastHit();
        }

        protected override void FixedUpdate()
        {
            MoveCharacter();
            ApplyGravity();
            HandleStepClimb();
            grounded = false;
        }

        protected override void Update()
        {
            equippedWeapon = playerCharacter.GetInventory().GetEquipped();
            HandleCrouch();
            HandleJump();
            PlayFootstepSounds();
        }

        #endregion

        #region METHODS

        private void MoveCharacter()
        {
            Vector2 frameInput = playerCharacter.GetInputMovement();
            var movement = new Vector3(frameInput.x, 0.0f, frameInput.y);

            if (playerCharacter.IsRunning() && !isCrouching)
                movement *= speedRunning;
            else if (isCrouching)
                movement *= speedCrouching;
            else
                movement *= speedWalking;

            movement = transform.TransformDirection(movement);
            movement.y = rigidBody.velocity.y; // Retain vertical velocity for slopes and gravity.

            Velocity = new Vector3(movement.x, rigidBody.velocity.y, movement.z);
        }

        private void ApplyGravity()
        {
            if (!grounded)
                playerVelocity.y += gravity * Time.fixedDeltaTime;
            else
                playerVelocity.y = Mathf.Max(0, playerVelocity.y);

            rigidBody.velocity = new Vector3(rigidBody.velocity.x, playerVelocity.y, rigidBody.velocity.z);
        }

        private void HandleCrouch()
        {
            if (Input.GetKey(crouchKey))
            {
                isCrouching = true;
                capsule.height = crouchHeight;
            }
            else
            {
                isCrouching = false;
                capsule.height = normalHeight;
            }
        }

        private void HandleJump()
        {
            // Apply jump force directly when grounded and spacebar is pressed
            if (Input.GetKeyDown(KeyCode.Space) && grounded && !isCrouching)
            {
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce, rigidBody.velocity.z);
            }
        }

        private void HandleStepClimb()
        {
            Vector3 lowerRayOrigin = transform.position + new Vector3(0, 0.1f, 0);
            Vector3 upperRayOrigin = transform.position + new Vector3(0, stepHeight, 0);

            if (Physics.Raycast(lowerRayOrigin, transform.forward, out RaycastHit lowerHit, 0.5f))
            {
                if (!Physics.Raycast(upperRayOrigin, transform.forward, 0.5f))
                {
                    float stepHeightDifference = lowerHit.point.y - transform.position.y;
                    if (stepHeightDifference > 0 && stepHeightDifference <= stepHeight)
                    {
                        rigidBody.position += new Vector3(0, stepHeightDifference + stepSmooth, 0);
                    }
                }
            }
        }

        private void PlayFootstepSounds()
        {
            if (grounded && rigidBody.velocity.sqrMagnitude > 0.1f)
            {
                audioSource.clip = playerCharacter.IsRunning() ? audioClipRunning : audioClipWalking;
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
            else if (audioSource.isPlaying)
                audioSource.Pause();
        }

        #endregion
    }
}
