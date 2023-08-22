using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    public GameObject Party, Inv, Map, Insights, Journal, Canvas, InvContent, QuickCharInfo, Member1, Member2, Member3, Member4;
    RectTransform InvRect;
    Vector3 PartyPos, InvPos, MapPos, InsightsPos;
    PlayerData PData;
    ItemData.Catagory InvFilter = ItemData.Catagory.Consumable;
    List<ItemData.Item> SortedInv = new();
    public GameObject InventoryItemPrefab;
    List<GameObject> ItemPrefabList = new();
    public ItemData.Item ?SelectedItem = null;
    // Start is called before the first frame update
    private void Start()
    {
        InvRect = InvContent.GetComponent<RectTransform>();
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
        InvRect.sizeDelta = new(InvRect.sizeDelta.x, 50 * GetSortedSize());
        DrawSortInv();
    }

    void DrawSortInv()
    {
        QuickCharInfo.SetActive(false);
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
                QuickCharInfo.SetActive(false);
                break;
            case "Inventory":
                Debug.LogWarning("Inventory");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = InvPos;
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                SortInv(InvFilter);
                InvRect.sizeDelta = new(InvRect.sizeDelta.x, 50 * GetSortedSize());
                QuickCharInfo.SetActive(false);
                break;
            case "Map":
                Debug.LogWarning("Map");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = new Vector3(InvPos.x - 172.80f * 3, InvPos.y);
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                QuickCharInfo.SetActive(false);
                break;
            case "Insights":
                Debug.LogWarning("Insights");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = new Vector3(InvPos.x - 172.80f * 3, InvPos.y);
                Map.transform.localPosition = new Vector3(MapPos.x - 172.80f * 5, MapPos.y);
                Insights.transform.localPosition = InsightsPos;
                QuickCharInfo.SetActive(false);
                break;
            case "Journal":
                Debug.LogWarning("Journal");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = new Vector3(InvPos.x - 172.80f * 3, InvPos.y);
                Map.transform.localPosition = new Vector3(MapPos.x - 172.80f * 5, MapPos.y);
                Insights.transform.localPosition = new Vector3(InsightsPos.x - 172.80f * 7, InsightsPos.y);
                QuickCharInfo.SetActive(false);
                break;
            case "Item":
                InvFilter = ItemData.Catagory.Consumable;
                SortInv(InvFilter);
                break;
            case "Gear":
                InvFilter = ItemData.Catagory.Gear;
                SortInv(InvFilter);
                break;
            case "Charm":
                InvFilter = ItemData.Catagory.Charm;
                SortInv(InvFilter);
                break;
            case "Key":
                InvFilter = ItemData.Catagory.Key;
                SortInv(InvFilter);
                break;
            case "EquipItem":
                QuickCharInfo.SetActive(true);
                break;
            default:
                Debug.LogWarning("Default");
                break;
        }
    }

    public void QuickInfoShow(int CharPos)
    {
        TMPro.TextMeshProUGUI CharIcon, HpTxt, MpTxt, Pouch1, Pouch2, Pouch3, Pouch4, Pouch5, WeaponTxt, ChestTxt, LegsTxt, BootsTxt, Charm1Txt, Charm2Txt;
        GameObject HpBar, MpBar;
        CharIcon = GameObject.Find("CharIcon").GetComponent<TMPro.TextMeshProUGUI>();
        Pouch1 = GameObject.Find("PItem1").GetComponent<TMPro.TextMeshProUGUI>();
        Pouch2 = GameObject.Find("PItem2").GetComponent<TMPro.TextMeshProUGUI>();
        Pouch3 = GameObject.Find("PItem3").GetComponent<TMPro.TextMeshProUGUI>();
        Pouch4 = GameObject.Find("PItem4").GetComponent<TMPro.TextMeshProUGUI>();
        Pouch5 = GameObject.Find("PItem5").GetComponent<TMPro.TextMeshProUGUI>();
        WeaponTxt = GameObject.Find("EquippedWeapon").GetComponent<TMPro.TextMeshProUGUI>();
        ChestTxt = GameObject.Find("EquippedChest").GetComponent<TMPro.TextMeshProUGUI>();
        LegsTxt = GameObject.Find("EquippedLegs").GetComponent<TMPro.TextMeshProUGUI>();
        BootsTxt = GameObject.Find("EquippedBoots").GetComponent<TMPro.TextMeshProUGUI>();
        Charm1Txt = GameObject.Find("EquippedCharm1").GetComponent<TMPro.TextMeshProUGUI>();
        Charm2Txt = GameObject.Find("EquippedCharm2").GetComponent<TMPro.TextMeshProUGUI>();
        HpTxt = GameObject.Find("HpNum").GetComponent<TMPro.TextMeshProUGUI>();
        MpTxt = GameObject.Find("MpNum").GetComponent<TMPro.TextMeshProUGUI>();
        HpBar = GameObject.Find("HpBarCasing");
        MpBar = GameObject.Find("MpBarCasing");

        for (int i = 0; i < 4; i++)
        {
            if(PData.characters[i].position == CharPos)
            {
                Pouch1.SetText(PData.characters[i].Pouch[0].Name);
                Pouch2.SetText(PData.characters[i].Pouch[1].Name);
                Pouch3.SetText(PData.characters[i].Pouch[2].Name);
                Pouch4.SetText(PData.characters[i].Pouch[3].Name);
                Pouch5.SetText(PData.characters[i].Pouch[4].Name);
                WeaponTxt.SetText(PData.characters[i].Weapon.Name);
                ChestTxt.SetText(PData.characters[i].Chest.Name);
                LegsTxt.SetText(PData.characters[i].Legs.Name);
                BootsTxt.SetText(PData.characters[i].Boots.Name);
                Charm1Txt.SetText(PData.characters[i].Charm1.Name);
                Charm2Txt.SetText(PData.characters[i].Charm2.Name);
                HpTxt.SetText(PData.characters[i].CurrentHp + "/" + PData.characters[i].Stats.Hp);
                MpTxt.SetText(PData.characters[i].CurrentMp + "/" + PData.characters[i].Stats.Mp);
                float percentageFilled;
                percentageFilled = (float)PData.characters[i].CurrentHp / (float)PData.characters[i].Stats.Hp;
                HpBar.GetComponent<Slider>().value = percentageFilled;
                percentageFilled = (float)PData.characters[i].CurrentMp / (float)PData.characters[i].Stats.Mp;
                MpBar.GetComponent<Slider>().value = percentageFilled;
            }
        }
    }
}
