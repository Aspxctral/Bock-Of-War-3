using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float velocity = 5f;
    public float sprintAdittion = 3.5f;
    public float jumpForce = 18f;
    public float jumpTime = 0.85f;
    public float gravity = 9.8f;

    [Header("Axe Inventory")]
    public Transform rightHand;
    public float pickupRange = 2f;
    public LayerMask pickupLayer;
    public GameObject interactionUI;
    public TMP_Text interactionText;
    public GameObject popupText;
    public TMP_Text popupLabel;

    float jumpElapsedTime = 0f;

    // Player states
    bool isJumping = false;
    bool isSprinting = false;
    bool isCombat = false; // fight mode

    // Inputs
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputSprint;
    bool inputCombatToggle;

    // Axe system
    private GameObject nearbyItem;
    private GameObject equippedItem;
    public List<GameObject> inventory = new List<GameObject>();

    // Combo system
    private int comboStep = 0;
    private float comboTimer = 0f;
    public float comboResetTime = 2.5f;
    private bool canPunch = true;

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

        // Combat toggle
        inputCombatToggle = Input.GetKeyDown(KeyCode.F);
        if (inputCombatToggle)
        {
            isCombat = !isCombat;
            animator.SetBool("isCombat", isCombat);
        }

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

        // Punch combo
        HandlePunchCombo();

        HeadHittingDetect();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    #region Movement

    void HandleMovementAnimations()
    {
        if (animator != null)
        {
            // Prevent jumping while in combat
            if (isCombat)
                inputJump = false;

            float minSpeed = 0.1f; // very sensitive so walk triggers easily
            bool isMoving = cc.velocity.magnitude > minSpeed;

            animator.SetBool("run", isMoving && !isCombat);
            isSprinting = isMoving && inputSprint && !isCombat;
            animator.SetBool("sprint", isSprinting);
        }
    }

    void HandleJump()
    {
        if (!isCombat && inputJump && cc.isGrounded)
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

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * 3f;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * 2f;
            yield return null;
        }

        popupText.SetActive(false);
    }

    #endregion

    #region Punch Combo

    void HandlePunchCombo()
    {
        // Countdown combo window
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
                comboStep = 0;
        }

        // Left click combo input
        if (Input.GetMouseButtonDown(0) && canPunch && isCombat && animator != null)
        {
            if (comboTimer > 0f)
            {
                comboStep = Mathf.Clamp(comboStep + 1, 1, 3);
            }
            else
            {
                comboStep = 1;
            }

            animator.SetInteger("ComboStep", comboStep);
            animator.SetTrigger("Punch");

            comboTimer = comboResetTime;
        }
    }

    #endregion
}