using UnityEngine;

public class Fighter : MonoBehaviour
{
    private Animator anim;

    [Header("Inventory Reference")]
    public PlayerInventory inventory; // assign PlayerInventory in inspector

    [Header("Animator Weapon Bool")]
    public string hasAxeParam = "hasAxe"; // Animator bool to track axe equipped

    private bool isCombat = false;
    private int comboStep = 0;
    private float lastClickedTime = 0f;
    public float maxComboDelay = 1f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

void Update()
{
    HandleCombatToggle();
    UpdateHasAxe();
    HandleComboInput();
    ResetComboIfTooSlow();
}

    // Only set hasAxe for attack branch, not idle
 void UpdateHasAxe()
{
    bool hasAxe = false;

    if (inventory != null && inventory.rightHand != null)
    {
        Transform hand = inventory.rightHand;

        // Check direct children only
        for (int i = 0; i < hand.childCount; i++)
        {
            Transform child = hand.GetChild(i);

            if (child.CompareTag("Pickup") && child.gameObject.activeSelf)
            {
                hasAxe = true;
                break;
            }
        }
    }

    anim.SetBool("hasAxe", hasAxe);
}

    void HandleCombatToggle()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isCombat = !isCombat;
            anim.SetBool("isCombat", isCombat);
            comboStep = 0;
        }
    }

    void HandleComboInput()
    {
        if (!isCombat) return;

        if (Input.GetMouseButtonDown(0))
        {
            lastClickedTime = Time.time;
            comboStep++;
            comboStep = Mathf.Clamp(comboStep, 1, 3);

            bool hasAxe = anim.GetBool(hasAxeParam);

            if (!hasAxe)
            {
                if (comboStep == 1) anim.SetTrigger("Punch1");
                else if (comboStep == 2) anim.SetTrigger("Punch2");
                else if (comboStep == 3)
                {
                    anim.SetTrigger("Punch3");
                    comboStep = 0;
                }
            }
            else
            {
                if (comboStep == 1) anim.SetTrigger("axe_hit1");
                else if (comboStep == 2) anim.SetTrigger("axe_hit2");
                else if (comboStep == 3)
                {
                    anim.SetTrigger("axe_hit3");
                    comboStep = 0;
                }
            }
        }
    }

    void ResetComboIfTooSlow()
    {
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            comboStep = 0;
        }
    }
}