using UnityEngine;

public class QuestTest : MonoBehaviour
{
    public GameObject objectiveMarker;

    public void ActivateQuest()
    {
        objectiveMarker.SetActive(true);
        Debug.Log("Quest Activated: Go To Marker");
    }
}