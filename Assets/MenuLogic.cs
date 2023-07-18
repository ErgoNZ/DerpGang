using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        GameObject Party, Inventory, Map, Insights, Journal, Canvas;
        Party = GameObject.Find("PartyBtn");
        Inventory = GameObject.Find("InventoryBtn");
        Map = GameObject.Find("MapBtn");
        Insights = GameObject.Find("InsightsBtn");
        Journal = GameObject.Find("JournalBtn");
        Canvas = GameObject.Find("Canvas");
    }
    public void MenuSwitch(string Menu)
    {
        switch (Menu)
        {
            case "Party":
                Debug.LogWarning("Party");
                break;
            case "Inventory":
                Debug.LogWarning("Inventory");
                break;
            case "Map":
                Debug.LogWarning("Map");
                break;
            case "Insights":
                Debug.LogWarning("Insights");
                break;
            case "Journal":
                Debug.LogWarning("Journal");
                break;
            default:
                Debug.LogWarning("Default");
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            
        }
    }
}
