using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    private Animator anim;
    public float cooldownTime = 1f; // time between attacks
    private float nextFireTime = 0f;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time > nextFireTime && Input.GetMouseButtonDown(0))
        {
            Attack();
            nextFireTime = Time.time + cooldownTime;
        }
    }

    void Attack()
    {
        anim.SetBool("hit1", true); // Trigger attack
        // Start coroutine to reset the Bool
        StartCoroutine(ResetHit1());
    }

    IEnumerator ResetHit1()
    {
        // Wait until the hit1 animation is playing
        yield return null; // wait one frame
        anim.SetBool("hit1", false);
    }
}
