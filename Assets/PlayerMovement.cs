using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float jumpForce = 7f;

    [Header("Sprint Scaling")]
    public float baseSprintSpeed = 7f;
    public float sprintIncreasePerLevel = 10f;
    public float maxSprintCap = 100f;

    [Header("Stamina Scaling")]
    public float baseStamina = 10f;
    public float staminaIncreasePerLevel = 5f;

    [Header("UI")]
    public Slider sprintSlider;
    public TextMeshProUGUI sprintText;
    public TextMeshProUGUI staminaText;
    public Slider staminaSlider;
    public Slider staminaSliderGame;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Movement Control")]
    private bool canMove = true;

    private Rigidbody rb;
    private Animator anim;
    private float moveX;
    private float moveZ;
    private bool isGrounded;
    private bool jumpRequested;

    [Header("Stamina")]
    public float maxStamina;
    public float currentStamina;
    public float staminaDrainRate = 1f;
    public float staminaRegenRate = 1f;
    private bool isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        anim = GetComponent<Animator>();

        sprintSpeed = baseSprintSpeed;

        maxStamina = baseStamina;
        currentStamina = maxStamina;

        UpdateSprintUI();
    }

    void Update()
    {
        if (!canMove) return;

        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            jumpRequested = true;

        HandleAnimations();
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        if (cameraTransform == null)
        {
            Debug.LogError("CameraTransform not assigned!");
            return;
        }

        bool sprintInput = Input.GetKey(KeyCode.LeftShift) && moveZ > 0;

        if (sprintInput && currentStamina > 0f)
        {
            isSprinting = true;
            currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
        }
        else
        {
            isSprinting = false;
            currentStamina += staminaRegenRate * Time.fixedDeltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        float speed = isSprinting ? sprintSpeed : walkSpeed;

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

        UpdateSprintUI();
    }

    // 🔥 LEVEL UP HANDLER
    public void OnLevelUp(int level)
    {
        // Speed scaling
        sprintSpeed = Mathf.Min(baseSprintSpeed + (level - 1) * sprintIncreasePerLevel, maxSprintCap);

        // Stamina scaling (+5 per level)
        maxStamina = baseStamina + (level - 1) * staminaIncreasePerLevel;

        // Refill stamina
        currentStamina = maxStamina;

        UpdateSprintUI();

        Debug.Log($"Level {level} → Speed: {sprintSpeed} | Stamina: {maxStamina}");
    }

    void UpdateSprintUI()
    {
        if (sprintSlider != null)
            sprintSlider.value = sprintSpeed;

        if (sprintText != null)
            sprintText.text = $"Speed: {Mathf.RoundToInt(sprintSpeed)} / {Mathf.RoundToInt(maxSprintCap)}";

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        if (staminaSliderGame != null)
        {
            staminaSliderGame.maxValue = maxStamina;
            staminaSliderGame.value = currentStamina;
        }

        if (staminaText != null)
            staminaText.text = $"Stamina: {Mathf.CeilToInt(currentStamina)} / {Mathf.CeilToInt(maxStamina)}";
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
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (anim.GetBool("jump")) anim.SetBool("jump", false);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    public void SetMovementActive(bool active)
    {
        canMove = active;
    }
}