using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    private Animator anim;

    [Header("Inventory Reference")]
    public PlayerInventory inventory;

    [Header("Weapon (Axe)")]
    public WeaponDamage axe;

    [Header("Punch / Kick Settings")]
    [Tooltip("All colliders that should activate during unarmed attacks")]
    public List<Collider> unarmedAttackColliders = new List<Collider>();

    [Tooltip("Damage dealt by unarmed hits")]
    public float unarmedDamage = 15f;

    [Tooltip("Delay before enabling colliders (sync with animation wind-up)")]
    public float unarmedEnableDelay = 0.35f;

    [Tooltip("How long colliders stay active (active hit window)")]
    public float unarmedActiveDuration = 0.25f;

    [Tooltip("Min time between attacks to prevent spam")]
    public float attackCooldown = 0.6f;

    [Header("Axe Combo Settings")]
    public float axeEnableDelay = 0.3f;
    public float axeActiveDuration = 0.3f;

    [Header("Combo Timing")]
    public float maxComboDelay = 1.2f;          // time window to continue combo

    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private bool inCombat = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Safety: disable everything at start
        if (axe != null) axe.DisableDamage();

        foreach (var col in unarmedAttackColliders)
        {
            if (col != null) col.enabled = false;
        }

        // Debug tip
        if (unarmedAttackColliders.Count == 0)
        {
            Debug.LogWarning("No unarmed colliders assigned in Fighter → punches won't hit anything!");
        }
    }

    void Update()
    {
        HandleCombatToggle();
        HandleComboInput();
        ResetComboIfTooSlow();
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

    void HandleComboInput()
    {
        if (!inCombat) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        if (Input.GetMouseButtonDown(0))
        {
            comboStep = Mathf.Clamp(comboStep + 1, 1, 3);
            lastAttackTime = Time.time;

            bool hasAxe = anim.GetBool("hasAxe");

            if (!hasAxe)
            {
                // Unarmed (punch/kick) combo
                anim.SetTrigger("Punch" + comboStep);
                StartCoroutine(EnableUnarmedColliders(unarmedEnableDelay, unarmedActiveDuration));
            }
            else
            {
                // Axe combo
                anim.SetTrigger("axe_hit" + comboStep);
                if (axe != null)
                {
                    StartCoroutine(EnableAxeDamage(axeEnableDelay, axeActiveDuration));
                }
            }
        }
    }

    IEnumerator EnableUnarmedColliders(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        if (!inCombat) yield break;

        // Enable all assigned colliders (fist + elbow + leg etc.)
        foreach (var col in unarmedAttackColliders)
        {
            if (col != null && !col.enabled)
            {
                col.enabled = true;
                // Optional debug
                // Debug.Log($"Enabled unarmed collider: {col.name}");
            }
        }

        yield return new WaitForSeconds(duration);

        // Disable them again
        foreach (var col in unarmedAttackColliders)
        {
            if (col != null) col.enabled = false;
        }
    }

    IEnumerator EnableAxeDamage(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        if (axe != null) axe.EnableDamage();
        yield return new WaitForSeconds(duration);
        if (axe != null) axe.DisableDamage();
    }

    void ResetComboIfTooSlow()
    {
        if (comboStep > 0 && Time.time > lastAttackTime + maxComboDelay)
        {
            comboStep = 0;
        }
    }

    // Optional: call from animation events at end of each attack clip if needed
    public void EndAttack()
    {
        // You can force-disable colliders here as safety net
        foreach (var col in unarmedAttackColliders)
        {
            if (col != null) col.enabled = false;
        }
        if (axe != null) axe.DisableDamage();
    }
}