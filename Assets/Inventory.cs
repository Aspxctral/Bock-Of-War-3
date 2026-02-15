using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> inventory = new List<GameObject>();

    public Transform rightHand;          // Assign in Inspector
    public float pickupRange = 2f;       // How close player must be
    public LayerMask pickupLayer;        // Set to "Pickup" layer in Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Pickup"))
            {
                PickUpItem(hit.gameObject);
                break;
            }
        }
    }

    void PickUpItem(GameObject item)
    {
        inventory.Add(item);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = item.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        item.transform.SetParent(rightHand);
        item.transform.localPosition = new Vector3(-0.13f, 0.05f, 0f);
        item.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);


        Debug.Log("Picked up: " + item.name);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
