using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float jumpForce = 7f;

    [Header("Camera")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private Animator anim;

    private float moveX;
    private float moveZ;
    private bool isGrounded;
    private bool jumpRequested;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        anim = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Input
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        // Request jump if grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
        }

        HandleAnimations();
    }

    void FixedUpdate()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // Camera-relative movement
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * moveZ + camRight * moveX;
        Vector3 velocity = moveDir.normalized * speed;

        // Apply horizontal movement
        rb.linearVelocity = new Vector3(
            velocity.x,
            rb.linearVelocity.y,
            velocity.z
        );

        // Apply jump if requested
        if (jumpRequested)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            TriggerJumpAnimation();
            jumpRequested = false;
            isGrounded = false; // prevent immediate reset
        }

        // Rotate player toward movement
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(
                Quaternion.Slerp(rb.rotation, targetRotation, 15f * Time.fixedDeltaTime)
            );
        }
    }

    void HandleAnimations()
    {
        if (anim == null) return;

        // Don't override jump animation while mid-air
        if (anim.GetBool("jump")) return;

        bool moving = moveX != 0 || moveZ != 0;
        bool sprinting = moving && Input.GetKey(KeyCode.LeftShift);

        if (!moving)
        {
            anim.SetBool("idle", true);
            anim.SetBool("run", false);
            anim.SetBool("sprint", false);
        }
        else if (sprinting)
        {
            anim.SetBool("idle", false);
            anim.SetBool("run", false);
            anim.SetBool("sprint", true);
        }
        else
        {
            anim.SetBool("idle", false);
            anim.SetBool("run", true);
            anim.SetBool("sprint", false);
        }
    }
void TriggerJumpAnimation()
{
    // Smoothly blend from current state to Jump over 0.1s
    anim.CrossFade("Jump", 0.1f);
    anim.SetBool("jump", true);
}

    // Ground detection using collisions
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            // Reset jump animation on landing
            if (anim.GetBool("jump"))
                anim.SetBool("jump", false);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}