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

    [Header("Movement Control")]
    private bool canMove = true; // freeze movement when UI open

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
    }

    void Update()
    {
        if (!canMove) return; // freeze movement when disabled

        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            jumpRequested = true;

        HandleAnimations();
    }

    void FixedUpdate()
    {
        if (!canMove) return; // freeze physics when disabled

        if (cameraTransform == null)
        {
            Debug.LogError("CameraTransform not assigned!");
            return;
        }

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * moveZ + camRight * moveX;
        Vector3 velocity = moveDir.normalized * speed;

        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            TriggerJumpAnimation();
            jumpRequested = false;
            isGrounded = false;
        }

        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 15f * Time.fixedDeltaTime));
        }
    }

    void HandleAnimations()
    {
        if (anim == null) return;
        if (anim.GetBool("jump")) return;

        bool moving = moveX != 0 || moveZ != 0;
        bool sprinting = moving && Input.GetKey(KeyCode.LeftShift);

        anim.SetBool("idle", !moving);
        anim.SetBool("run", moving && !sprinting);
        anim.SetBool("sprint", sprinting);
    }

    void TriggerJumpAnimation()
    {
        anim.CrossFade("Jump", 0.1f);
        anim.SetBool("jump", true);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision == null || collision.gameObject == null) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (anim.GetBool("jump")) anim.SetBool("jump", false);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision == null || collision.gameObject == null) return;

        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    // ✅ Public method to freeze/unfreeze movement
    public void SetMovementActive(bool active)
    {
        canMove = active;
    }
}