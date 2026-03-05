using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float jumpHeight = 1.5f;
    private Vector3 lastMoveDirection = Vector3.forward;
    private CharacterController controller;
    private Animator animator;
    private Vector3 playerVelocity;
    private Vector2 moveInput;
    private Camera mainCamera;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime;

    [Header("Aiming")]
    [SerializeField] private GameObject laserSightObject;
    [SerializeField] private LayerMask aimLayerMask;
    private bool isAiming = false;
    private LineRenderer laserLineRenderer;
    private Vector3 currentAimPoint;


    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;
    private InputAction moveAction, shootAction, aimAction, jumpAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        var playerMap = playerControls.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        shootAction = playerMap.FindAction("Shoot");
        aimAction = playerMap.FindAction("Aim");
        jumpAction = playerMap.FindAction("Jump");

        if (laserSightObject != null)
            laserLineRenderer = laserSightObject.GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        shootAction.Enable();
        aimAction.Enable();
        jumpAction.Enable();

        shootAction.performed += OnShoot;
        aimAction.performed += OnAimPerformed;
        aimAction.canceled += OnAimCanceled;
        jumpAction.performed += OnJump;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        shootAction.Disable();
        aimAction.Disable();
        jumpAction.Disable();

        shootAction.performed -= OnShoot;
        aimAction.performed -= OnAimPerformed;
        aimAction.canceled -= OnAimCanceled;
        jumpAction.performed -= OnJump;
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        HandleMovement();
        HandleAnimations();

        if (isAiming)
        {
            HandleAimingRotation();
            UpdateLaserSight();
        }

    }

    // -------------------- MOVEMENT --------------------
    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 inputDir = (camRight * input.x + camForward * input.y);
        float inputMag = inputDir.magnitude;
        Vector3 moveDirection = inputMag > 0.001f ? inputDir.normalized : Vector3.zero;

        Vector3 horizontalMove = Vector3.zero;
        if (!isAiming)
        {
            horizontalMove = moveDirection * moveSpeed;
            if (moveDirection != Vector3.zero)
                lastMoveDirection = moveDirection;
        }

        if (controller.isGrounded && playerVelocity.y < 0f)
            playerVelocity.y = -2f; // snap al suelo
        else
            playerVelocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = horizontalMove;
        finalMove.y = playerVelocity.y;

        controller.Move(finalMove * Time.deltaTime);

        if (!isAiming)
        {
            if (moveDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            else
            {
                Quaternion targetRot = Quaternion.LookRotation(lastMoveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * 0.5f * Time.deltaTime);
            }
        }
        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


    // -------------------- ANIMATIONS --------------------
    private void HandleAnimations()
    {
        bool grounded = controller.isGrounded;

        float realSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;

        Vector2 input = moveAction.ReadValue<Vector2>();
        bool hasInput = input.sqrMagnitude > 0.01f;

        if (!isAiming && hasInput && realSpeed > 0.1f)
            animator.SetFloat("Speed", realSpeed);
        else
            animator.SetFloat("Speed", 0f);

        animator.SetBool("IsGrounded", grounded);
        animator.SetBool("IsAiming", isAiming);
    }

    // -------------------- JUMP --------------------
    private void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded && !isAiming)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("JumpTrigger");
        }
    }

    // -------------------- SHOOT --------------------
    private void OnShoot(InputAction.CallbackContext context)
    {
        if (isAiming && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ShootProjectile();
            animator.SetTrigger("ShootTrigger");
        }
    }

    private void ShootProjectile()
    {
        if (!projectilePrefab || !firePoint) return;

        Vector3 shootDirection = (currentAimPoint - firePoint.position).normalized;
    
        Quaternion shootRotation = Quaternion.LookRotation(shootDirection);
        Instantiate(projectilePrefab, firePoint.position, shootRotation);
    }

    // -------------------- AIMING --------------------
    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        isAiming = true;
        moveInput = Vector2.zero;
        if (laserSightObject) laserSightObject.SetActive(true);
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        isAiming = false;
        if (laserSightObject) laserSightObject.SetActive(false);
    }

    private void HandleAimingRotation()
    {
        if (!mainCamera) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimLayerMask))
        {
            Vector3 lookAtPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Vector3 dir = (lookAtPoint - transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateLaserSight()
    {
        if (!laserLineRenderer || !firePoint || !mainCamera) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimLayerMask))
        {
            laserLineRenderer.SetPosition(0, firePoint.position);
            laserLineRenderer.SetPosition(1, hit.point);

            // Guardamos el punto exacto del impacto
            currentAimPoint = hit.point;
        }
        else
        {
            // Si no pega en nada, apuntamos a un punto lejano en la dirección del rayo
            currentAimPoint = ray.origin + ray.direction * 100f;
            laserLineRenderer.SetPosition(0, firePoint.position);
            laserLineRenderer.SetPosition(1, currentAimPoint);
        }
    }
}
