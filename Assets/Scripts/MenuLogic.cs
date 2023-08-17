using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    GameObject Party, Inv, Map, Insights, Journal, Canvas, InvContent;
    RectTransform InvRect;
    Vector3 PartyPos, InvPos, MapPos, InsightsPos;
    PlayerData PData;
    ItemData.Catagory InvFilter = ItemData.Catagory.Consumable;
    List<ItemData.Item> SortedInv = new();
    public GameObject InventoryItemPrefab;
    List<GameObject> ItemPrefabList = new();
    List<GameObject> CharQuickInfoList = new();
    // Start is called before the first frame update
    private void Start()
    {
        Party = GameObject.Find("PartyBtn");
        Inv = GameObject.Find("InvBtn");
        Map = GameObject.Find("MapBtn");
        Insights = GameObject.Find("InsightBtn");
        Journal = GameObject.Find("JournalBtn");
        Canvas = GameObject.Find("Canvas");
        InvContent = GameObject.Find("Content");
        InvRect = InvContent.GetComponent<RectTransform>();
        PData = GetComponent<PlayerData>();
        PartyPos = Party.transform.localPosition;
        InvPos = Inv.transform.localPosition;
        MapPos = Map.transform.localPosition;
        InsightsPos = Insights.transform.localPosition;
        Canvas.SetActive(false);
        CharQuickInfoList.Add(GameObject.Find("CharQuickInfo1"));
        CharQuickInfoList.Add(GameObject.Find("CharQuickInfo2"));
        CharQuickInfoList.Add(GameObject.Find("CharQuickInfo3"));
        CharQuickInfoList.Add(GameObject.Find("CharQuickInfo4"));
    }

    private void Awake()
    {
        PData = GetComponent<PlayerData>();
    }

    int GetSortedSize()
    {
        return SortedInv.Count;
    }

    void SortInv(ItemData.Catagory Filter)
    {
        Filter = InvFilter;
        SortedInv.Clear();
        for (int i = 0; i < PData.Inventory.Count; i++)
        {
            if(PData.Inventory[i].Type == Filter)
            {
                SortedInv.Add(PData.Inventory[i]);
            }

            if(Filter == ItemData.Catagory.Gear)
            {
                if (PData.Inventory[i].Type == ItemData.Catagory.Weapon)
                {
                    SortedInv.Add(PData.Inventory[i]);
                }
                if (PData.Inventory[i].Type == ItemData.Catagory.Chest)
                {
                    SortedInv.Add(PData.Inventory[i]);
                }
                if (PData.Inventory[i].Type == ItemData.Catagory.Legs)
                {
                    SortedInv.Add(PData.Inventory[i]);
                }
                if (PData.Inventory[i].Type == ItemData.Catagory.Boots)
                {
                    SortedInv.Add(PData.Inventory[i]);
                }
            }
        }
    }

    void DrawSortInv()
    {
        for (int i = 0; i < ItemPrefabList.Count; i++)
        {
            Destroy(ItemPrefabList[i]);
        }
        ItemPrefabList.Clear();
        for (int i = 0; i < GetSortedSize(); i++)
        {
            ItemPrefabList.Add(Instantiate(InventoryItemPrefab));
            ItemPrefabList[i].transform.SetParent(InvContent.transform);
            ItemPrefabList[i].transform.localPosition = new(0, InvRect.rect.yMax - (50 * i) -25);
            ItemPrefabList[i].transform.localScale = new(1, 1, 1);
            ItemBtnLogic itemBtnLogic = ItemPrefabList[i].GetComponent<ItemBtnLogic>();
            itemBtnLogic.FillInfo(SortedInv[i].Amount.ToString(), SortedInv[i].Name, SortedInv[i]);
        }
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
                SortInv(InvFilter);
                InvRect.sizeDelta = new(InvRect.sizeDelta.x, 50 * GetSortedSize());
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
                InvFilter = ItemData.Catagory.Consumable;
                SortInv(InvFilter);
                InvRect.sizeDelta = new(InvRect.sizeDelta.x, 50 * GetSortedSize());
                DrawSortInv();
                break;
            case "Gear":
                InvFilter = ItemData.Catagory.Gear;
                SortInv(InvFilter);
                InvRect.sizeDelta = new(InvRect.sizeDelta.x, 50 * GetSortedSize());
                DrawSortInv();
                break;
            case "Charm":
                InvFilter = ItemData.Catagory.Charm;
                SortInv(InvFilter);
                InvRect.sizeDelta = new(InvRect.sizeDelta.x, 50 * GetSortedSize());
                DrawSortInv();
                break;
            case "Key":
                InvFilter = ItemData.Catagory.Key;
                SortInv(InvFilter);
                InvRect.sizeDelta = new(InvRect.sizeDelta.x, 50 * GetSortedSize());
                DrawSortInv();
                break;
            default:
                Debug.LogWarning("Default");
                break;
        }
    }

    void QuickInfoShow()
    {

    }
}
