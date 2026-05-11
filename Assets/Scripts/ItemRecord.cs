using System;
using System.Collections.Generic;

[System.Serializable]
public class ItemRecord
{
    public string id;
    public string category;
    public string itemName;
    public string modelNo;

    public int sold_quantity;
    public int available;
    public float revenue;
    public float cost;

    public float profitGenerated;
    public DateTime lastSoldDate;

    public bool IsValid(out string error)
    {
        // Check for empty strings
        if (string.IsNullOrWhiteSpace(id)) { error = "ID is required"; return false; }
        if (string.IsNullOrWhiteSpace(category)) { error = "Category is required"; return false; }
        if (string.IsNullOrWhiteSpace(itemName)) { error = "Item Name is required"; return false; }
        if (string.IsNullOrWhiteSpace(modelNo)) { error = "Model No is required"; return false; }

        // Logic checks
        if (sold_quantity < 0) { error = "Sold count cannot be negative"; return false; }
        if (available < 0) { error = "Available stock cannot be negative"; return false; }
        if (revenue < 0 || cost < 0) { error = "Pricing cannot be negative"; return false; }

        // Business logic: You might sell at a loss occasionally, 
        // but if your validation requires profit, keep this:
        if (revenue < cost)
        {
            error = "Sold amount is less than cost (Negative Profit)";
            return false;
        }

        // Date validation: Check if date was actually set (not default)
        if (lastSoldDate == DateTime.MinValue)
        {
            error = "A valid Last Sold Date is required";
            return false;
        }

        error = null;
        return true;
    }
    [System.Serializable]
    public class ItemDatabase
    {
        public List<ItemRecord> items = new List<ItemRecord>();
    }
}