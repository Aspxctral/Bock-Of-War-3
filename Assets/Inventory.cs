using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerInventory : MonoBehaviour
{
    public Transform rightHand;
    public float pickupRange = 2f;
    public LayerMask pickupLayer;

    [Header("UI")]
    public GameObject interactionUI;     // "G Equip | K Store"
    public TMP_Text interactionText;
    public GameObject popupText;         // Big cinematic text
    public TMP_Text popupLabel;

    private GameObject nearbyItem;
    private GameObject equippedItem;

    public List<GameObject> inventory = new List<GameObject>();

    void Update()
    {
        CheckForItem();

        // Equip from ground
        if (nearbyItem != null && Input.GetKeyDown(KeyCode.G))
        {
            EquipItem(nearbyItem);
            ShowPopup("LEVIATHAN AXE ACQUIRED");
        }

        // Store / Toggle inventory
        if (Input.GetKeyDown(KeyCode.K))
        {
            HandleInventoryToggle();
        }
    }

    void CheckForItem()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);

        if (hits.Length > 0)
        {
            nearbyItem = hits[0].transform.root.gameObject;

            interactionUI.SetActive(true);
            interactionText.text = "Press G to Equip\nPress K to Store";
        }
        else
        {
            nearbyItem = null;
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
        {
            col.enabled = false;
        }

        item.transform.SetParent(rightHand);
        item.transform.localPosition = new Vector3(-0.12f, 0.2f, 0f);
        item.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);

        interactionUI.SetActive(false);
    }

    void HandleInventoryToggle()
    {
        // Store currently equipped
        if (equippedItem != null)
        {
            inventory.Add(equippedItem);
            equippedItem.SetActive(false);
            equippedItem = null;

            ShowPopup("AXE STORED");
        }
        // Pull from inventory
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
        popupLabel.text = message;
        popupText.SetActive(true);

        CanvasGroup canvasGroup = popupText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = popupText.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0;

        // Fade In
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * 3;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        // Fade Out
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 2;
            yield return null;
        }

        popupText.SetActive(false);
    }
}
