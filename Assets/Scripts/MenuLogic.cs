using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    GameObject Party, Inv, Map, Insights, Journal, Canvas;
    Vector3 PartyPos, InvPos, MapPos, InsightsPos;
    PlayerData PData;
    string InvFilter = "Consumables";
    // Start is called before the first frame update
    private void Start()
    {
        Party = GameObject.Find("PartyBtn");
        Inv = GameObject.Find("InvBtn");
        Map = GameObject.Find("MapBtn");
        Insights = GameObject.Find("InsightBtn");
        Journal = GameObject.Find("JournalBtn");
        Canvas = GameObject.Find("Canvas");
        PData = GetComponent<PlayerData>();
        PartyPos = Party.transform.localPosition;
        InvPos = Inv.transform.localPosition;
        MapPos = Map.transform.localPosition;
        InsightsPos = Insights.transform.localPosition;
        Canvas.SetActive(false);
    }

    private void Awake()
    {
        PData = GetComponent<PlayerData>();
    }
    public void MenuSwitch(string Menu)
    {
        switch (Menu)
        {
            case "Party":
                Debug.LogWarning("Party");
                Party.transform.localPosition = PartyPos;
                Inv.transform.localPosition = InvPos;
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                break;
            case "Inventory":
                Debug.LogWarning("Inventory");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = InvPos;
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                break;
            case "Map":
                Debug.LogWarning("Map");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = new Vector3(InvPos.x - 172.80f * 3, InvPos.y);
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                break;
            case "Insights":
                Debug.LogWarning("Insights");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = new Vector3(InvPos.x - 172.80f * 3, InvPos.y);
                Map.transform.localPosition = new Vector3(MapPos.x - 172.80f * 5, MapPos.y);
                Insights.transform.localPosition = InsightsPos;
                break;
            case "Journal":
                Debug.LogWarning("Journal");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = new Vector3(InvPos.x - 172.80f * 3, InvPos.y);
                Map.transform.localPosition = new Vector3(MapPos.x - 172.80f * 5, MapPos.y);
                Insights.transform.localPosition = new Vector3(InsightsPos.x - 172.80f * 7, InsightsPos.y);
                break;
            case "Item":
                InvFilter = "Consumables";
                break;
            case "Gear":
                InvFilter = "Gear";
                break;
            case "Charms":
                InvFilter = "Charms";
                break;
            case "Key":
                InvFilter = "Key";
                break;
            default:
                Debug.LogWarning("Default");
                break;
        }
    }
}
