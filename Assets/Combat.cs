using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    private Animator anim;
    public float cooldownTime = 2f;
    private float nextFireTime = 0f;
    public static int noOfClicks = 0;
    float lastClickedTime = 0;
    float maxComboDelay = 1f;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Press F to enter combat
        if (Input.GetKeyDown(KeyCode.F))
        {
            EnterCombat();
        }

        // Reset combo if too much time passed
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClicks = 0;
        }

        // Check for mouse input
        if (Time.time > nextFireTime && Input.GetMouseButtonDown(0))
        {
            RegisterClick();
        }

        // Automatically advance combos if conditions are met
        HandleComboProgression();
    }

    void RegisterClick()
    {
        lastClickedTime = Time.time;
        noOfClicks = Mathf.Clamp(noOfClicks + 1, 0, 3);
    }

    void HandleComboProgression()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        // First punch
        if (noOfClicks >= 1 && state.IsName("hit1") == false && state.IsName("hit2") == false && state.IsName("hit3") == false)
        {
            anim.SetBool("hit1", true);
        }

        // Second punch
        if (noOfClicks >= 2 && state.IsName("hit1") && state.normalizedTime > 0.7f)
        {
            anim.SetBool("hit1", false);
            anim.SetBool("hit2", true);
        }

        // Third punch
        if (noOfClicks >= 3 && state.IsName("hit2") && state.normalizedTime > 0.7f)
        {
            anim.SetBool("hit2", false);
            anim.SetBool("hit3", true);
        }

        // Reset after final punch
        if (state.IsName("hit3") && state.normalizedTime > 0.9f)
        {
            anim.SetBool("hit3", false);
            noOfClicks = 0;
        }
    }

    void EnterCombat()
    {
        // Reset all combo bools
        anim.SetBool("hit1", false);
        anim.SetBool("hit2", false);
        anim.SetBool("hit3", false);

        // Reset combo counter
        noOfClicks = 0;

        // Enter fight idle via isCombat parameter
        anim.SetBool("isCombat", true);
    }
}