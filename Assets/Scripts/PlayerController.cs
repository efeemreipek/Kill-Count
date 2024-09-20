using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float upDownRange = 80f;
    [SerializeField] private Transform _cameraHolderTransform;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private AudioClip[] jumpStartSounds;
    [SerializeField] private AudioClip[] jumpLandSounds;

    [Header("Footstep")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float velocityThreshold = 0.2f;

    // INPUT
    private PlayerInputControls _playerInputControls;
    private InputAction _movementAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private Vector2 inputMovement;
    private Vector2 inputLook;

    private CharacterController _characterController;
    private Camera _camera;

    private float verticalRotation;
    private Vector3 currentMovement = Vector3.zero;
    private float nextStepTime;
    private bool isMoving;
    private int lastPlayedIndex = -1;
    private bool wasGrounded = true;

    private void Awake()
    {
        Instance = this;

        _playerInputControls = new PlayerInputControls();
        _characterController = GetComponent<CharacterController>();
        _camera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        mouseSensitivity = SettingsManager.Instance.mouseSensitivity;
    }

    private void OnEnable()
    {
        _movementAction = _playerInputControls.Player.Movement;
        _lookAction = _playerInputControls.Player.Look;
        _sprintAction = _playerInputControls.Player.Sprint;
        _jumpAction = _playerInputControls.Player.Jump;

        _movementAction.performed += MovementAction_Performed;
        _movementAction.canceled += MovementAction_Canceled;
        _lookAction.performed += LookAction_Performed;
        _lookAction.canceled += LookAction_Canceled;

        _playerInputControls.Enable();
    }

    private void OnDisable()
    {
        _movementAction.performed -= MovementAction_Performed;
        _lookAction.performed -= LookAction_Performed;

        _playerInputControls.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleFootsteps();
    }

    private void LookAction_Performed(InputAction.CallbackContext obj) => inputLook = obj.ReadValue<Vector2>();
    private void MovementAction_Performed(InputAction.CallbackContext obj) => inputMovement = obj.ReadValue<Vector2>();
    private void LookAction_Canceled(InputAction.CallbackContext obj) => inputLook = Vector2.zero;
    private void MovementAction_Canceled(InputAction.CallbackContext obj) => inputMovement = Vector2.zero;
    private void HandleMovement()
    {
        float speedMultiplier = _sprintAction.ReadValue<float>() > 0f ? sprintMultiplier : 1f;

        float horizontalSpeed = inputMovement.x * walkSpeed * speedMultiplier;
        float verticalSpeed = inputMovement.y * walkSpeed * speedMultiplier;

        Vector3 horizontalMovement = new Vector3(horizontalSpeed, transform.position.y, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        HandleGravityAndJump();

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        _characterController.Move(currentMovement * Time.deltaTime);

        isMoving = inputMovement.x != 0f || inputMovement.y != 0f;
    }
    private void HandleRotation()
    {
        float mouseXRotation = inputLook.x * mouseSensitivity;
        float mouseYRotation = inputLook.y * mouseSensitivity;

        transform.Rotate(0f, mouseXRotation, 0f);

        verticalRotation -= mouseYRotation;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);

        _cameraHolderTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

    }
    private void HandleGravityAndJump()
    {

        if (_characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (!wasGrounded)
            {
                PlaySounds(jumpLandSounds);
            }

            if (_jumpAction.triggered)
            {
                currentMovement.y = jumpForce;
                PlaySounds(jumpStartSounds);
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }

        wasGrounded = _characterController.isGrounded;
    }
    private void HandleFootsteps()
    {
        float currentStepInterval = _sprintAction.ReadValue<float>() > 0f ? sprintStepInterval : walkStepInterval;

        if (_characterController.isGrounded && isMoving && Time.time > nextStepTime && _characterController.velocity.magnitude > velocityThreshold)
        {
            PlaySounds(footstepSounds);
            nextStepTime = Time.time + currentStepInterval;
        }
    }
    private void PlaySounds(AudioClip[] sounds)
    {
        int randomIndex;
        if (sounds.Length == 1)
        {
            randomIndex = 0;
        }
        else
        {
            randomIndex = Random.Range(0, sounds.Length - 1);
            if (randomIndex > lastPlayedIndex)
            {
                randomIndex++;
            }
        }

        lastPlayedIndex = randomIndex;
        audioSource.clip = sounds[randomIndex];
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();
    }
    public Vector2 GetMovementInput() => inputMovement;
    public Vector2 GetLookInput() => inputLook;
    public bool GetIsGrounded() => _characterController.isGrounded;
    public float GetWalkSpeed() => walkSpeed;

}

