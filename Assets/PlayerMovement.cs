using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float mouseSensitivity = 2f;

    private Vector3 moveDirection;
    private float rotationY;

    [SerializeField] float jumpCooldown = 2f;
    private bool canJump = true;


    [SerializeField]
    float jumpForce = 5f;

    Rigidbody rb;
    private bool jumpRequested;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        if (Keyboard.current.spaceKey.wasPressedThisFrame && canJump)
        {
            jumpRequested = true;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.Self);
        if (Keyboard.current.leftShiftKey.isPressed)
            speed = 10f;
        else
            speed = 5f;

    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        rotationY += mouseX * mouseSensitivity;

        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    private void FixedUpdate()
    {
        if (jumpRequested)
        {
            if (transform.position.y < 3.4f)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(JumpCooldown());
            }

            jumpRequested = false;
        }
    }

    private System.Collections.IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }


}
