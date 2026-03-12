using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    private Animator anim;

    [Header("Inventory Reference")]
    public PlayerInventory inventory;

    [Header("Punch / Unarmed Settings")]
    public List<Collider> unarmedAttackColliders = new List<Collider>();
    public float unarmedDamage = 15f;
    public float unarmedEnableDelay = 0.18f;
    public float unarmedActiveDuration = 0.22f;

    [Header("Axe Settings")]
    public WeaponDamage axe;
    public float axeEnableDelay = 0.18f;
    public float axeActiveDuration = 0.22f;

    [Header("Combo Settings")]
    public float comboWindow = 0.65f;

    private int comboStep = 0;
    private float lastClickTime = 0f;
    private bool inCombat = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (axe != null) axe.DisableDamage();

        foreach (var col in unarmedAttackColliders)
            if (col != null) col.enabled = false;
    }

    void Update()
    {
        HandleCombatToggle();
        HandleCombo();
        UpdateHasAxeParameter();
    }

    void HandleCombatToggle()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            inCombat = !inCombat;
            anim.SetBool("isCombat", inCombat);
            if (!inCombat) comboStep = 0;
        }
    }

    void UpdateHasAxeParameter()
    {
        bool hasAxe = inventory != null && inventory.equippedItem != null;
        anim.SetBool("hasAxe", hasAxe);
    }

    void HandleCombo()
    {
        if (!inCombat) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime > comboWindow)
                comboStep = 1;
            else
                comboStep = Mathf.Min(comboStep + 1, 3);

            lastClickTime = Time.time;

            bool hasAxe = anim.GetBool("hasAxe");

            if (!hasAxe)
            {
                anim.SetTrigger("Punch" + comboStep);
                StartCoroutine(EnableUnarmedColliders());
            }
            else if (axe != null)
            {
                anim.SetTrigger("axe_hit" + comboStep);
                StartCoroutine(EnableAxeDamage());
            }
        }
    }

    IEnumerator EnableUnarmedColliders()
    {
        yield return new WaitForSeconds(unarmedEnableDelay);

        if (!inCombat) yield break;

        // Enable
        foreach (var col in unarmedAttackColliders)
            if (col != null) col.enabled = true;

        // Small flicker to force overlap detection if already overlapping
        yield return new WaitForSeconds(0.01f);
        foreach (var col in unarmedAttackColliders)
            if (col != null)
            {
                col.enabled = false;
                col.enabled = true;
            }

        yield return new WaitForSeconds(unarmedActiveDuration - 0.01f);

        // Disable
        foreach (var col in unarmedAttackColliders)
            if (col != null) col.enabled = false;
    }

    IEnumerator EnableAxeDamage()
    {
        yield return new WaitForSeconds(axeEnableDelay);

        if (axe != null) axe.EnableDamage();

        yield return new WaitForSeconds(axeActiveDuration);

        if (axe != null) axe.DisableDamage();
    }
}