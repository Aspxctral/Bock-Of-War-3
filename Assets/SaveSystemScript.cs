using UnityEngine;
using TMPro;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public TMP_InputField saveNameInput;
    public Transform contentParent;
    public GameObject saveButtonPrefab;

    string savePath;

    void Start()
    {
        savePath = Application.persistentDataPath + "/saves/";

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        RefreshSaveList();
    }

    public void CreateSave()
    {
        string saveName = saveNameInput.text;
        if (string.IsNullOrEmpty(saveName)) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        SaveData data = new SaveData
        {
            saveName = saveName,
            x = player.transform.position.x,
            y = player.transform.position.y,
            z = player.transform.position.z
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath + saveName + ".json", json);

        RefreshSaveList();
    }

    public void LoadSave(string filePath)
    {
        string json = File.ReadAllText(filePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(data.x, data.y, data.z);
    }

    void RefreshSaveList()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        string[] files = Directory.GetFiles(savePath, "*.json");

        foreach (string file in files)
        {
            GameObject btn = Instantiate(saveButtonPrefab, contentParent);

            string fileName = Path.GetFileNameWithoutExtension(file);

            btn.GetComponentInChildren<TMPro.TMP_Text>().text = fileName;

            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LoadSave(file));
        }
    }
}