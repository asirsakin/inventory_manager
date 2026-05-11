using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.IO;
[System.Serializable]
public class ItemDatabase
{
    public List<ItemRecord> items = new List<ItemRecord>();
}

public class process : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private string savePath;
    public Text to_kam;
    [Header("Input Fields")]
    public InputField[] Input;
    [Header("Date Field")]
    public TMP_InputField dateInputField;
    [Header("Data Table")]
    public List<ItemRecord> table = new List<ItemRecord>();
    public Text label;
    public GameObject submit_but, update_but;
    private void Start()
    {
        // Path handling for different platforms
#if UNITY_WEBGL && !UNITY_EDITOR
        // In WebGL, we rely on PlayerPrefs or IndexedDB mapping. 
        // We'll use a string-based approach for maximum compatibility.
        savePath = "WEBGL_STORAGE";
#elif UNITY_ANDROID && !UNITY_EDITOR
        savePath = Path.Combine(Application.persistentDataPath, "shop8.json");
#else
        savePath = Path.Combine(Application.persistentDataPath, "shop8.json");
#endif

        LoadData();
        ClearFields();
    }

    public void SaveData()
    {
        ItemDatabase db = new ItemDatabase { items = table };
        string json = JsonUtility.ToJson(db, true);

#if UNITY_WEBGL && !UNITY_EDITOR
        PlayerPrefs.SetString(WebGL_Key, json);
        PlayerPrefs.Save();
        Debug.Log("Data Saved to PlayerPrefs (IndexedDB)");
#else
        File.WriteAllText(savePath, json);
        Debug.Log("Data Saved To: " + savePath);
#endif
    }

    public void LoadData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (PlayerPrefs.HasKey(WebGL_Key))
        {
            string json = PlayerPrefs.GetString(WebGL_Key);
            ItemDatabase db = JsonUtility.FromJson<ItemDatabase>(json);
            table = db.items;
            ShowTableData();
        }
#else
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            ItemDatabase db = JsonUtility.FromJson<ItemDatabase>(json);
            table = db.items;
            ShowTableData();
        }
#endif
       /// totalxkamai();

    }
    public void SetDate(string date)
    {
        dateInputField.text = date;
    }
    public void update_item()
    {
        label.text = "update";
        Input[0].gameObject.SetActive (false);
        submit_but.SetActive(false);
        update_but.SetActive(true);

        temp = EventSystem.current.currentSelectedGameObject.name;
        Transform txo = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(0);
        for (int i = 0; i < txo.childCount-2; i++)
        {
            Input[i].text= txo.GetChild(i).GetComponent<Text>().text;
        }
    }
    public void delete_item()
    {
        temp = EventSystem.current.currentSelectedGameObject.name;
        DeleteItem();
    }
    public void add_item()
    {
        label.text = "Insert data";
        Input[0].gameObject.SetActive(true);
        submit_but.SetActive(true);

        update_but.SetActive(false);
        temp = "";

    }
    public void AddItem()
    {
        ItemRecord item = new ItemRecord();

        // Mapping Strings
        item.id = Input[0].text;
        item.category = Input[1].text;
        item.itemName = Input[2].text;
        item.modelNo = Input[3].text;

        // Mapping Numbers
        item.sold_quantity = ParseInt(Input[4].text);
        item.available = ParseInt(Input[5].text);
        item.revenue = ParseFloat(Input[6].text);
        item.cost = ParseFloat(Input[7].text);

        // Auto profit
        item.profitGenerated = item.revenue - item.cost;

        string dateInput = dateInputField.text.Trim();

        DateTime parsedDate;

        string[] formats =
        {
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "dd-MM-yyyy"
        };

        if (DateTime.TryParseExact(
            dateInput,
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out parsedDate))
        {
            item.lastSoldDate = parsedDate;
        }
        else
        {
            Debug.LogError("Invalid date format!");
            return;
        }

        // --- THE FIX: Call the validation method ---
        string errorMessage;
        if (item.IsValid(out errorMessage))
        {
            table.Add(item);
            Debug.Log("<color=green>Success:</color> Item Added: " + item.itemName);
            ClearFields();
            SaveData();
        }
        else
        {
            // Display why it failed
            Debug.LogError("Validation Failed: " + errorMessage);
        }

    }
    int ParseInt(string value)
    {
        int result;
        int.TryParse(value, out result);
        return result;
    }

    float ParseFloat(string value)
    {
        float result;
        float.TryParse(value, out result);
        return result;
    }

    void ClearFields()
    {
       foreach(InputField inp in Input)
        {
            inp.text = "";
        }
      
    
    }
   
    public GameObject prefab;
    public Transform parent;
    public float total_kamai;

    public void ShowTableData()
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
        // 1. Safety check: Ensure there is data to process
        if (table == null || table.Count == 0)
        {
            Debug.LogWarning("Table is empty. Nothing to populate.");
            return;
        }

        // 2. Clear tab_dat so we don't duplicate data on every button click

      
        // 3. Use a for loop to iterate through the table
        for (int i = 0; i < table.Count; i++)
        {
            // Reference the current item in the list
            ItemRecord item = table[i];

            string row = $"{item.id} | {item.category} | {item.itemName} | {item.modelNo} | " +
                        $"{item.sold_quantity} | {item.available} | {item.revenue} | " +
                        $"{item.cost} | {item.profitGenerated} | {item.lastSoldDate.ToShortDateString()}";

            
            GameObject dat_ob = Instantiate(prefab,parent.position,Quaternion.identity) as GameObject;
            dat_ob.transform.SetParent(parent);
            dat_ob.transform.localScale = prefab.transform.localScale;
            Transform par = dat_ob.transform.GetChild(0);

            par.GetChild(0).GetComponent<Text>().text = item.id.ToString();
            par.GetChild(1).GetComponent<Text>().text = item.category.ToString();
            par.GetChild(2).GetComponent<Text>().text = item.itemName.ToString();
            par.GetChild(3).GetComponent<Text>().text = item.modelNo.ToString();

            par.GetChild(4).GetComponent<Text>().text = item.sold_quantity.ToString();
            par.GetChild(5).GetComponent<Text>().text = item.available.ToString();
            par.GetChild(6).GetComponent<Text>().text = item.revenue.ToString();
            par.GetChild(7).GetComponent<Text>().text = item.cost.ToString();

            par.GetChild(8).GetComponent<Text>().text = item.profitGenerated.ToString();
            par.GetChild(9).GetComponent<Text>().text = item.lastSoldDate.ToShortDateString();

            dat_ob.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(update_item);
            dat_ob.transform.GetChild(1).name = item.id;

            dat_ob.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delete_item);
            dat_ob.transform.GetChild(2).name = item.id;

        }
        totalxkamai();

        // 6. Output the final count to console
    }
    [SerializeField]
    private string temp;
    public void UpdateItem()
    {
        string targetId = temp;
        print(targetId);
        // Find item in list
        ItemRecord item = table.Find(x => x.id == targetId);

        if (item == null)
        {
            Debug.LogError("Item not found with ID: " + targetId);
            return;
        }

        // Now use your MAIN Input[] fields (not hierarchy guessing)
        item.category = Input[1].text;
        item.itemName = Input[2].text;
        item.modelNo = Input[3].text;

        item.sold_quantity = ParseInt(Input[4].text);
        item.available = ParseInt(Input[5].text);
        item.revenue = ParseFloat(Input[6].text);
        item.cost = ParseFloat(Input[7].text);

        item.profitGenerated = item.revenue - item.cost;

        // Date
        string dateInput = dateInputField.text.Trim();
        DateTime parsedDate;

        string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy" };

        if (DateTime.TryParseExact(dateInput, formats,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out parsedDate))
        {
            item.lastSoldDate = parsedDate;
        }
        else
        {
            Debug.LogError("Invalid date format");
            return;
        }

        string errorMessage;
        if (!item.IsValid(out errorMessage))
        {
            Debug.LogError("Validation Failed: " + errorMessage);
            return;
        }

        Debug.Log("<color=yellow>Updated:</color> " + item.itemName);
        temp = "";

        ShowTableData();
        SaveData();
      //  totalxkamai();


    }
  public  void totalxkamai()
    {
        total_kamai = 0;
        for (int i = 0; i < table.Count; i++)
        {
            ItemRecord item1 = table[i];

            total_kamai += item1.profitGenerated;
        }

        to_kam.text= total_kamai.ToString();
    }
    public void DeleteItem()
    {
        string targetId = temp;

        if (string.IsNullOrEmpty(targetId))
        {
            Debug.LogError("No item selected to delete.");
            return;
        }

        ItemRecord item = table.Find(x => x.id == targetId);

        if (item == null)
        {
            Debug.LogError("Item not found with ID: " + targetId);
            return;
        }

        // Build same row string format
        string row = $"{item.id} | {item.category} | {item.itemName} | {item.modelNo} | " +
                     $"{item.sold_quantity} | {item.available} | {item.revenue} | " +
                     $"{item.cost} | {item.profitGenerated} | {item.lastSoldDate.ToShortDateString()}";

        // Remove from both
        table.Remove(item);

        Debug.Log("<color=red>Deleted:</color> " + item.itemName);

        temp = "";

        // Refresh UI
        ShowTableData();
        SaveData();

    }
    [Header("Search")]
    public InputField searchInputField; // The UI field where you type search terms
    private List<ItemRecord> filteredTable = new List<ItemRecord>();
    public void SearchItems()
    {
        string term = searchInputField.text.ToLower().Trim();

        if (string.IsNullOrEmpty(term))
        {
            // If search is empty, show the full table
            ShowSearch_table(table);
        }
        else
        {
            // Filter the table based on multiple criteria
            filteredTable = table.FindAll(x =>
                x.id.ToLower().Contains(term) ||
                x.itemName.ToLower().Contains(term) ||
                x.category.ToLower().Contains(term) ||
                x.modelNo.ToLower().Contains(term)
            );

            ShowSearch_table(filteredTable);
        }
    }
    public void ShowSearch_table(List<ItemRecord> listToDisplay = null)
    {
        // Default to the main table if no list is provided
        if (listToDisplay == null) listToDisplay = table;

        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }

        if (listToDisplay.Count == 0)
        {
            Debug.LogWarning("No data to populate.");
            return;
        }

        // Use listToDisplay instead of table
        for (int i = 0; i < listToDisplay.Count; i++)
        {
            ItemRecord item = listToDisplay[i];

            GameObject dat_ob = Instantiate(prefab, parent.position, Quaternion.identity) as GameObject;
            dat_ob.transform.SetParent(parent);
            dat_ob.transform.localScale = prefab.transform.localScale;

            Transform par = dat_ob.transform.GetChild(0);

            // ... [Keep your existing GetChild(x).GetComponent<Text>() assignments here] ...
            par.GetChild(0).GetComponent<Text>().text = item.id.ToString();
            par.GetChild(1).GetComponent<Text>().text = item.category.ToString();
            par.GetChild(2).GetComponent<Text>().text = item.itemName.ToString();
            // etc...

            dat_ob.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(update_item);
            dat_ob.transform.GetChild(1).name = item.id;

            dat_ob.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delete_item);
            dat_ob.transform.GetChild(2).name = item.id;
        }
    }
}

