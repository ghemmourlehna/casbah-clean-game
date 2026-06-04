using UnityEngine;

public class ThirdPersonPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;
    public float crouchSpeed = 1.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;
    public float turnSpeed = 10f;

    [Header("References")]
    public CharacterController controller;
    public Animator animator;
    public Transform modelTransform;

    [Header("Crouch")]
    public float crouchHeight = 1.2f;
    public Vector3 crouchCenter = new Vector3(0, 0.6f, 0);

    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching = false;
    private bool isSliding = false;
    private bool canMove = true;

    private float normalHeight;
    private Vector3 normalCenter;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();

        normalHeight = controller.height;
        normalCenter = controller.center;

        if (animator != null)
        {
            animator.SetBool("CanMove", true);
            animator.SetBool("IsCrouching", false);
            animator.SetFloat("Speed", 0f);
        }
    }

    void Update()
    {
        if (!canMove)
        {
            ApplyGravityOnly();
            return;
        }

        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleSlide();
        ApplyGravity();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) vertical = -1f;

        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;

        float currentSpeed = 0f;

        if (!isSliding && move.magnitude > 0.1f)
        {
            if (isCrouching)
                currentSpeed = crouchSpeed;
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                currentSpeed = runSpeed;
            else
                currentSpeed = walkSpeed;

            controller.Move(move * currentSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(move);
            if (modelTransform != null)
                modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        float speedParam = 0f;
        if (currentSpeed == walkSpeed) speedParam = 0.5f;
        if (currentSpeed == runSpeed) speedParam = 1f;
        if (currentSpeed == crouchSpeed) speedParam = 0.2f;

        if (animator != null)
            animator.SetFloat("Speed", speedParam);
    }

    void HandleJump()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching && !isSliding)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
                animator.SetTrigger("Jump");
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                controller.height = crouchHeight;
                controller.center = crouchCenter;
            }
            else
            {
                controller.height = normalHeight;
                controller.center = normalCenter;
            }

            if (animator != null)
                animator.SetBool("IsCrouching", isCrouching);
        }
    }

    void HandleSlide()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isSliding && !isCrouching)
        {
            StartCoroutine(SlideRoutine());
        }
    }

    System.Collections.IEnumerator SlideRoutine()
    {
        isSliding = true;

        if (animator != null)
            animator.SetTrigger("Slide");

        float timer = 0.6f;
        Vector3 slideDirection = modelTransform != null ? modelTransform.forward : transform.forward;

        while (timer > 0f)
        {
            controller.Move(slideDirection * runSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        isSliding = false;
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void ApplyGravityOnly()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void PlayPickUp()
    {
        if (animator != null)
            animator.SetTrigger("PickUp");
    }

    public void PlayWin()
    {
        canMove = false;
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Win");
        }
    }

    public void PlayDeath()
    {
        canMove = false;
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Die");
        }
    }
}