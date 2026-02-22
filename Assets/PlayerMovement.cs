using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Third-person movement + axe inventory for the player.
/// </summary>
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed ​​at which the character moves. It is not affected by gravity or jumping.")]
    public float velocity = 5f;
    [Tooltip("This value is added to the speed value while the character is sprinting.")]
    public float sprintAdittion = 3.5f;
    [Tooltip("The higher the value, the higher the character will jump.")]
    public float jumpForce = 18f;
    [Tooltip("Stay in the air. The higher the value, the longer the character floats before falling.")]
    public float jumpTime = 0.85f;
    [Tooltip("Force that pulls the player down. Changing this value causes all movement, jumping and falling to be changed as well.")]
    public float gravity = 9.8f;

    [Header("Axe Inventory")]
    public Transform rightHand;
    public float pickupRange = 2f;
    public LayerMask pickupLayer;
    public GameObject interactionUI; // "G Equip | K Store"
    public Text interactionText;
    public GameObject popupText;     // Big cinematic text
    public Text popupLabel;

    float jumpElapsedTime = 0f;

    // Player states
    bool isJumping = false;
    bool isSprinting = false;

    // Inputs
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputSprint;

    // Axe system
    private GameObject nearbyItem;
    private GameObject equippedItem;
    public List<GameObject> inventory = new List<GameObject>();

    Animator animator;
    CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogWarning("No Animator component found. Animations won't work.");
    }

    void Update()
    {
        // Movement inputs
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;
        inputSprint = Input.GetAxis("Fire3") == 1f;

        HandleMovementAnimations();
        HandleJump();

        // Axe pickup/equip system
        CheckForItem();

        if (nearbyItem != null && Input.GetKeyDown(KeyCode.G))
        {
            EquipItem(nearbyItem);
            ShowPopup("LEVIATHAN AXE ACQUIRED");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            HandleInventoryToggle();
        }

        HeadHittingDetect();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    #region Movement

    void HandleMovementAnimations()
    {
        if (cc.isGrounded && animator != null)
        {
            float minSpeed = 0.9f;
            animator.SetBool("run", cc.velocity.magnitude > minSpeed);

            isSprinting = cc.velocity.magnitude > minSpeed && inputSprint;
            animator.SetBool("sprint", isSprinting);
        }

        if (animator != null)
            animator.SetBool("air", !cc.isGrounded);
    }

    void HandleJump()
    {
        if (inputJump && cc.isGrounded)
            isJumping = true;
    }

    void ApplyMovement()
    {
        float velocityAddition = isSprinting ? sprintAdittion : 0f;

        float dirX = inputHorizontal * (velocity + velocityAddition) * Time.deltaTime;
        float dirZ = inputVertical * (velocity + velocityAddition) * Time.deltaTime;
        float dirY = 0f;

        if (isJumping)
        {
            dirY = Mathf.SmoothStep(jumpForce, jumpForce * 0.3f, jumpElapsedTime / jumpTime) * Time.deltaTime;
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0f;
            }
        }

        dirY -= gravity * Time.deltaTime;

        // Rotation
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();
        forward *= dirZ;
        right *= dirX;

        if (dirX != 0 || dirZ != 0)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        Vector3 verticalDir = Vector3.up * dirY;
        Vector3 horizontalDir = forward + right;
        cc.Move(verticalDir + horizontalDir);
    }

    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;

        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }

    #endregion

    #region Axe Inventory

    void CheckForItem()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);
        if (hits.Length > 0)
        {
            nearbyItem = hits[0].transform.root.gameObject;
            if (interactionUI != null && interactionText != null)
            {
                interactionUI.SetActive(true);
                interactionText.text = "Press G to Equip\nPress K to Store";
            }
        }
        else
        {
            nearbyItem = null;
            if (interactionUI != null)
                interactionUI.SetActive(false);
        }
    }

    void EquipItem(GameObject item)
    {
        equippedItem = item;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = item.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        item.transform.SetParent(rightHand);
        item.transform.localPosition = new Vector3(-0.12f, 0.2f, 0f);
        item.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);

        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    void HandleInventoryToggle()
    {
        if (equippedItem != null)
        {
            inventory.Add(equippedItem);
            equippedItem.SetActive(false);
            equippedItem = null;
            ShowPopup("AXE STORED");
        }
        else if (inventory.Count > 0)
        {
            GameObject item = inventory[0];
            inventory.RemoveAt(0);
            item.SetActive(true);
            EquipItem(item);
            ShowPopup("AXE EQUIPPED");
        }
    }

    void ShowPopup(string message)
    {
        StopAllCoroutines();
        StartCoroutine(PopupRoutine(message));
    }

    IEnumerator PopupRoutine(string message)
    {
        if (popupLabel == null || popupText == null)
            yield break;

        popupLabel.text = message;
        popupText.SetActive(true);

        CanvasGroup canvasGroup = popupText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = popupText.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        // Fade in
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * 3f;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        // Fade out
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * 2f;
            yield return null;
        }

        popupText.SetActive(false);
    }

    #endregion
}
