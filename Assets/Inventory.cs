using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    public Transform rightHand;
    public float pickupRange = 2f;

    [Header("UI")]
    public GameObject interactionUI;
    public TMP_Text interactionText;
    public GameObject popupText;
    public TMP_Text popupLabel;

    [Header("Weapon Hand Positioning")]
    public Vector3 equipLocalPosition = new Vector3(-0.12f, 0.20f, 0.00f);
    public Vector3 equipLocalRotation = new Vector3(0f, 0f, 45f);

    [Header("Equipped References")]
    public GameObject equippedItem => _equippedItem;
    public WeaponDamage equippedWeapon { get; private set; }

    private GameObject _equippedItem;
    private GameObject nearbyItem;
    public List<GameObject> inventory = new List<GameObject>();

    void Update()
    {
        CheckForPickup();

        if (nearbyItem != null)
        {
            if (Input.GetKeyDown(KeyCode.G))
                EquipItem(nearbyItem);
            else if (Input.GetKeyDown(KeyCode.K))
                StoreItem(nearbyItem);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleInventory();
        }
    }

    void CheckForPickup()
    {
        nearbyItem = null;
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
        List<(GameObject item, float dist)> candidates = new List<(GameObject, float)>();

        foreach (Collider hit in hits)
        {
            PickupItem pu = hit.GetComponentInParent<PickupItem>()
                       ?? hit.GetComponentInChildren<PickupItem>()
                       ?? hit.GetComponent<PickupItem>();

            if (pu != null && !pu.isPickedUp)
            {
                GameObject item = pu.gameObject;
                if (item.transform.IsChildOf(rightHand) || inventory.Contains(item))
                    continue;

                float dist = Vector3.Distance(transform.position, item.transform.position);
                candidates.Add((item, dist));
            }
        }

        if (candidates.Count > 0)
        {
            candidates = candidates.OrderBy(c => c.dist).ToList();
            nearbyItem = candidates[0].item;
            interactionUI.SetActive(true);
            interactionText.text = "Press G to Equip\nPress K to Store";
        }
        else
        {
            interactionUI.SetActive(false);
        }
    }

    void EquipItem(GameObject item)
    {
        _equippedItem = item;

        if (item.TryGetComponent<PickupItem>(out var pickup))
            pickup.isPickedUp = true;

        equippedWeapon = item.GetComponent<WeaponDamage>();
        if (equippedWeapon != null)
            equippedWeapon.DisableDamage();

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = item.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        item.transform.SetParent(rightHand, worldPositionStays: false);
        item.transform.localPosition = equipLocalPosition;
        item.transform.localRotation = Quaternion.Euler(equipLocalRotation);
        item.transform.localScale = Vector3.one;

        interactionUI.SetActive(false);
        ShowPopup("AXE ACQUIRED");
    }

    void StoreItem(GameObject item)
    {
        if (item.TryGetComponent<PickupItem>(out var pickup))
            pickup.isPickedUp = true;

        inventory.Add(item);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        item.transform.SetParent(null);
        item.SetActive(false);

        interactionUI.SetActive(false);
        ShowPopup("AXE STORED");
    }

    void ToggleInventory()
    {
        if (_equippedItem != null)
        {
            inventory.Add(_equippedItem);

            Rigidbody rb = _equippedItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            _equippedItem.transform.SetParent(null);
            _equippedItem.SetActive(false);
            equippedWeapon = null;
            _equippedItem = null;

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
        popupLabel.text = message;
        popupText.SetActive(true);

        CanvasGroup cg = popupText.GetComponent<CanvasGroup>();
        if (cg == null) cg = popupText.AddComponent<CanvasGroup>();

        cg.alpha = 0;
        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime * 3;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime * 2;
            yield return null;
        }

        popupText.SetActive(false);
    }
}