using UnityEngine;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{
    [Header("Compass UI")]
    public RectTransform compassBarTransform;
    public RectTransform objectiveMarkerTransform;   // The moving arrow/marker

    [Header("Cardinal Directions")]
    public RectTransform northMarkerTransform;
    public RectTransform southMarkerTransform;
    public RectTransform eastMarkerTransform;
    public RectTransform westMarkerTransform;

    [Header("Camera")]
    public Transform cameraObjectTransform;

    [Header("Objectives - Connect from QuestUIController")]
    public Transform objective1;   // Drag worldObjective1 here
    public Transform objective2;   // Drag worldObjective2 here
    public Transform objective3;   // Drag worldObjective3 here

    private Transform currentObjective;   // The one currently active

    void Update()
    {
        // Update cardinal directions
        UpdateCardinalMarkers();

        // Update the main objective marker
        if (currentObjective != null)
        {
            SetMarkerPosition(objectiveMarkerTransform, currentObjective.position);
        }
        else
        {
            // Hide marker if no active objective
            objectiveMarkerTransform.gameObject.SetActive(false);
        }
    }

    private void UpdateCardinalMarkers()
    {
        if (cameraObjectTransform == null) return;

        SetMarkerPosition(northMarkerTransform, cameraObjectTransform.position + Vector3.forward * 1000);
        SetMarkerPosition(southMarkerTransform, cameraObjectTransform.position + Vector3.back * 1000);
        SetMarkerPosition(eastMarkerTransform, cameraObjectTransform.position + Vector3.right * 1000);
        SetMarkerPosition(westMarkerTransform, cameraObjectTransform.position + Vector3.left * 1000);
    }

    private void SetMarkerPosition(RectTransform markerTransform, Vector3 worldPosition)
    {
        if (markerTransform == null || cameraObjectTransform == null) return;

        Vector3 directionToTarget = worldPosition - cameraObjectTransform.position;

        float signedAngle = Vector3.SignedAngle(
            new Vector3(cameraObjectTransform.forward.x, 0, cameraObjectTransform.forward.z),
            new Vector3(directionToTarget.x, 0, directionToTarget.z),
            Vector3.up
        );

        float compassPosition = Mathf.Clamp(signedAngle / Camera.main.fieldOfView, -0.5f, 0.5f);

        markerTransform.anchoredPosition = new Vector2(
            compassBarTransform.rect.width * compassPosition,
            0
        );

        markerTransform.gameObject.SetActive(true);
    }

    // ====================== PUBLIC METHODS CALLED BY QUESTUICONTROLLER ======================

    public void SetActiveObjective(int questNumber)
    {
        switch (questNumber)
        {
            case 1:
                currentObjective = objective1;
                break;
            case 2:
                currentObjective = objective2;
                break;
            case 3:
                currentObjective = objective3;
                break;
            default:
                currentObjective = null;
                break;
        }

        // Show/hide the marker
        if (objectiveMarkerTransform != null)
            objectiveMarkerTransform.gameObject.SetActive(currentObjective != null);
    }

    public void ClearObjective()
    {
        currentObjective = null;
        if (objectiveMarkerTransform != null)
            objectiveMarkerTransform.gameObject.SetActive(false);
    }
}