using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    GameObject Party, Inventory, Map, Insights, Journal, Canvas;
    Vector3 PartyPos, InvPos, MapPos, InsightsPos;
    // Start is called before the first frame update
    private void Start()
    {
        Party = GameObject.Find("PartyBtn");
        Inventory = GameObject.Find("InvBtn");
        Map = GameObject.Find("MapBtn");
        Insights = GameObject.Find("InsightBtn");
        Journal = GameObject.Find("JournalBtn");
        Canvas = GameObject.Find("Canvas");
        PartyPos = Party.transform.localPosition;
        InvPos = Inventory.transform.localPosition;
        MapPos = Map.transform.localPosition;
        InsightsPos = Insights.transform.localPosition;
        Canvas.SetActive(false);
    }
    public void MenuSwitch(string Menu)
    {
        switch (Menu)
        {
            case "Party":
                Debug.LogWarning("Party");
                Party.transform.localPosition = PartyPos;
                Inventory.transform.localPosition = InvPos;
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                break;
            case "Inventory":
                Debug.LogWarning("Inventory");
                Party.transform.localPosition = new Vector3(PartyPos.x - (float)172.80, PartyPos.y);
                Inventory.transform.localPosition = InvPos;
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                break;
            case "Map":
                Debug.LogWarning("Map");
                Party.transform.localPosition = new Vector3(PartyPos.x - (float)172.80, PartyPos.y);
                Inventory.transform.localPosition = new Vector3(InvPos.x - (float)172.80 * 3, InvPos.y);
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                break;
            case "Insights":
                Debug.LogWarning("Insights");
                Party.transform.localPosition = new Vector3(PartyPos.x - (float)172.80, PartyPos.y);
                Inventory.transform.localPosition = new Vector3(InvPos.x - (float)172.80 * 3, InvPos.y);
                Map.transform.localPosition = new Vector3(MapPos.x - (float)172.80 * 5, MapPos.y);
                Insights.transform.localPosition = InsightsPos;
                break;
            case "Journal":
                Debug.LogWarning("Journal");
                Party.transform.localPosition = new Vector3(PartyPos.x - (float)172.80, PartyPos.y);
                Inventory.transform.localPosition = new Vector3(InvPos.x - (float)172.80 * 3, InvPos.y);
                Map.transform.localPosition = new Vector3(MapPos.x - (float)172.80 * 5, MapPos.y);
                Insights.transform.localPosition = new Vector3(InsightsPos.x - (float)172.80 * 7, InsightsPos.y);
                break;
            default:
                Debug.LogWarning("Default");
                break;
        }
    }
}
