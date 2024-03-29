using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    //Set up of all objects and variables
    [Header("General GUI")]
    public GameObject Party;
    public GameObject Inv;
    public GameObject Map;
    public GameObject Insights;
    public GameObject Journal;
    public GameObject Canvas;
    public GameObject UnderConstruction;
    [Header("InventoryGUI")]
    public GameObject InvContent;
    public GameObject QuickCharInfo;
    public GameObject Member1;
    public GameObject Member2;
    public GameObject Member3;
    public GameObject Member4;
    public GameObject InvPanel;
    public GameObject ItemPanel;
    public GameObject UseItem;
    [Header("PartyGUI")]
    public GameObject CharacterInfoPanel;
    public GameObject PartyPanel;
    public GameObject QuickMemberinfo1;
    public GameObject QuickMemberinfo2;
    public GameObject QuickMemberinfo3;
    public GameObject QuickMemberinfo4;
    [Header("QuickInfoMenu")]
    public GameObject Charm1;
    public GameObject Charm2;
    public GameObject Pouch1;
    public GameObject Pouch2;
    public GameObject Pouch3;
    public GameObject Pouch4;
    public GameObject Pouch5;
    [Header("MainMenu/Combat")]
    public GameObject MainMenu;
    public GameObject HowToPlayScreen;
    public GameObject Menu;
    public GameObject GameOver;
    public GameObject CombatMenu;
    public GameObject Title;
    RectTransform InvRect;
    Vector3 PartyPos, InvPos, MapPos, InsightsPos;
    PlayerData PData;
    ItemData.Catagory InvFilter = ItemData.Catagory.Consumable;
    List<ItemData.Item> SortedInv = new();
    [Header("Prefab stuff")]
    public GameObject InventoryItemPrefab;
    List<GameObject> ItemPrefabList = new();
    [Header("Item Stuff")]
    public ItemData.Item SelectedItem;
    public int SelectedChar = 1;
    ItemData.Item Placeholder = new();
    StateManager StateManager;
    // Start is called before the first frame update
    private void Start()
    {
        InvRect = InvContent.GetComponent<RectTransform>();
        PData = GetComponent<PlayerData>();
        PartyPos = Party.transform.localPosition;
        InvPos = Inv.transform.localPosition;
        MapPos = Map.transform.localPosition;
        InsightsPos = Insights.transform.localPosition;
        MenuSwitch("Party");
        //Canvas.SetActive(false);
    }
    private void Awake()
    {
        PData = GetComponent<PlayerData>();
        StateManager = GetComponent<StateManager>();
    }
    /// <summary>
    /// This returns with the length of the sorted list.
    /// </summary>
    /// <returns></returns>
    int GetSortedSize()
    {
        return SortedInv.Count;
    }
    /// <summary>
    /// Sorts the items from the inventory and adds the items that matches the filter to a new list
    /// </summary>
    /// <param name="Filter"></param>
    void SortInv(ItemData.Catagory Filter)
    {
        Filter = InvFilter;
        //Empties the old sorted list
        SortedInv.Clear();
        //Tells the button that you click to do stuff with items to not be useable
        UseItem.SetActive(true);
        UseItem.GetComponent<Button>().interactable = false;
        //Reads through every item in the inventory and if it matches the filter and is not of id 0 it is sorted into the new list
        for (int i = 0; i < PData.Inventory.Count; i++)
        {
            if(PData.Inventory[i].Type == Filter && PData.Inventory[i].ID != 0)
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
        //Resizes the size of the content for the scroll menu to fit the items
        InvRect.sizeDelta = new(InvRect.sizeDelta.x, 50 * GetSortedSize());
        DrawSortInv();
    }
    /// <summary>
    /// Resets the item button prefabs and creates new ones based on the sorted inventory and pass them the data they require to hold
    /// </summary>
    void DrawSortInv()
    {
        QuickCharInfo.SetActive(false);
        //Deletes all previous prefab instances of the item buttons
        for (int i = 0; i < ItemPrefabList.Count; i++)
        {
            Destroy(ItemPrefabList[i]);
        }
        ItemPrefabList.Clear();
        //Sets up each item button prefab instance and gives them the item data they need
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
    /// <summary>
    /// Switches between the menus based on the button clicked
    /// </summary>
    /// <param name="Menu"></param>
    public void MenuSwitch(string Menu)
    {
        UseItem.SetActive(false);
        QuickCharInfo.SetActive(false);
        InvPanel.SetActive(false);
        ItemPanel.SetActive(false);
        PartyPanel.SetActive(false);
        CharacterInfoPanel.SetActive(false);
        switch (Menu)
        {
            case "Party":
                Debug.LogWarning("Party");
                Party.transform.localPosition = PartyPos;
                Inv.transform.localPosition = InvPos;
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                UnderConstruction.SetActive(false);
                PartyPanel.SetActive(true);
                CharacterInfoPanel.SetActive(true);
                SetupPartyInfo();
                SetupCharInfo(1);
                break;
            case "Inventory":
                Debug.LogWarning("Inventory");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = InvPos;
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                UnderConstruction.SetActive(false);
                SortInv(InvFilter);
                InvRect.sizeDelta = new(InvRect.sizeDelta.x, 50 * GetSortedSize());
                InvPanel.SetActive(true);
                ItemPanel.SetActive(true);
                UseItem.SetActive(true);
                UseItem.GetComponent<Button>().interactable = false;
                break;
            case "Map":
                Debug.LogWarning("Map");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = new Vector3(InvPos.x - 172.80f * 3, InvPos.y);
                Map.transform.localPosition = MapPos;
                Insights.transform.localPosition = InsightsPos;
                UnderConstruction.SetActive(true);
                break;
            case "Insights":
                Debug.LogWarning("Insights");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = new Vector3(InvPos.x - 172.80f * 3, InvPos.y);
                Map.transform.localPosition = new Vector3(MapPos.x - 172.80f * 5, MapPos.y);
                Insights.transform.localPosition = InsightsPos;
                UnderConstruction.SetActive(true);
                break;
            case "Journal":
                Debug.LogWarning("Journal");
                Party.transform.localPosition = new Vector3(PartyPos.x - 172.80f, PartyPos.y);
                Inv.transform.localPosition = new Vector3(InvPos.x - 172.80f * 3, InvPos.y);
                Map.transform.localPosition = new Vector3(MapPos.x - 172.80f * 5, MapPos.y);
                Insights.transform.localPosition = new Vector3(InsightsPos.x - 172.80f * 7, InsightsPos.y);
                UnderConstruction.SetActive(true);
                break;
            case "Item":
                InvFilter = ItemData.Catagory.Consumable;
                SortInv(InvFilter);
                InvPanel.SetActive(true);
                ItemPanel.SetActive(true);
                UnderConstruction.SetActive(false);
                break;
            case "Gear":
                InvFilter = ItemData.Catagory.Gear;
                SortInv(InvFilter);
                InvPanel.SetActive(true);
                ItemPanel.SetActive(true);
                UnderConstruction.SetActive(false);
                break;
            case "Charm":
                InvFilter = ItemData.Catagory.Charm;
                SortInv(InvFilter);
                InvPanel.SetActive(true);
                ItemPanel.SetActive(true);
                UnderConstruction.SetActive(false);
                break;
            case "Key":
                InvFilter = ItemData.Catagory.Key;
                SortInv(InvFilter);
                InvPanel.SetActive(true);
                ItemPanel.SetActive(true);
                UnderConstruction.SetActive(false);
                break;
            case "EquipItem":
                QuickCharInfo.SetActive(true);
                InvPanel.SetActive(true);
                ItemPanel.SetActive(true);
                UnderConstruction.SetActive(false);
                QuickInfoShow(0);
                break;
            default:
                Debug.LogWarning("Default");
                break;
        }
    }
    /// <summary>
    /// Displays and updates all of the info on the quick info object
    /// </summary>
    /// <param name="CharPos"></param>
    public void QuickInfoShow(int CharPos)
    {
        SelectedChar = CharPos;
        TMPro.TextMeshProUGUI CharIcon, HpTxt, MpTxt, Pouch1, Pouch2, Pouch3, Pouch4, Pouch5, WeaponTxt, ChestTxt, LegsTxt, BootsTxt, Charm1Txt, Charm2Txt;
        TMPro.TextMeshProUGUI[] CharBtnTxt = new TMPro.TextMeshProUGUI[4];
        GameObject HpBar, MpBar, UseItemBtn, EquipItemBtn;
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
        CharBtnTxt[0] = GameObject.Find("MemberTxt1").GetComponent<TMPro.TextMeshProUGUI>();
        CharBtnTxt[1] = GameObject.Find("MemberTxt2").GetComponent<TMPro.TextMeshProUGUI>();
        CharBtnTxt[2] = GameObject.Find("MemberTxt3").GetComponent<TMPro.TextMeshProUGUI>();
        CharBtnTxt[3] = GameObject.Find("MemberTxt4").GetComponent<TMPro.TextMeshProUGUI>();
        UseItemBtn = GameObject.Find("UseItemOnChar");
        EquipItemBtn = GameObject.Find("EquipItemOnChar");

        if (SelectedItem.Element == ItemData.Element.Restore)
        {
            UseItemBtn.GetComponent<Button>().interactable = true;
        }
        else
        {
            UseItemBtn.GetComponent<Button>().interactable = false;
        }

        for (int i = 0; i < 4; i++)
        {
            int pos = PData.characters[i].position;
            CharBtnTxt[(pos - 1)].SetText(PData.characters[i].Name + "");
        }

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

                EquipItemBtn.GetComponent<Button>().interactable = true;
                if (!SelectedItem.characters.Contains(PData.characters[i].Name))
                {
                    EquipItemBtn.GetComponent<Button>().interactable = false;
                }
            }
        }
    }
    /// <summary>
    /// Enables equip buttons based on type of item
    /// </summary>
    public void EnableEquipBtns()
    {
        ItemData.Catagory Type = SelectedItem.Type;
        switch (Type)
        {
            case ItemData.Catagory.Charm:
                GameObject.Find("EquipItemOnChar").transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("Which charm slot?");
                Charm1.GetComponent<Button>().interactable = true;
                Charm2.GetComponent<Button>().interactable = true;
                break;
            case ItemData.Catagory.Consumable:
                GameObject.Find("EquipItemOnChar").transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("Which pouch slot?");
                Pouch1.GetComponent<Button>().interactable = true;
                Pouch2.GetComponent<Button>().interactable = true;
                Pouch3.GetComponent<Button>().interactable = true;
                Pouch4.GetComponent<Button>().interactable = true;
                Pouch5.GetComponent<Button>().interactable = true;
                break;
            default:
                EquipItem(0);
                break;
        }
    }
    /// <summary>
    /// Calls gear swap method based on the type of weapon and selected character and resets everything on the quick info object
    /// </summary>
    /// <param name="Slot"></param>
    public void EquipItem(int Slot)
    {
        if(SelectedItem.Type == ItemData.Catagory.Charm)
        {
            PData.CharmSwap(Slot, SelectedItem, SelectedChar);
        }
        if(SelectedItem.Type == ItemData.Catagory.Consumable)
        {
            PData.PouchSwapItems(Slot, SelectedItem, SelectedChar);
        }
        if (SelectedItem.Type == ItemData.Catagory.Boots)
        {
            PData.GearSwap(SelectedItem, SelectedChar);
        }
        if (SelectedItem.Type == ItemData.Catagory.Legs)
        {
            PData.GearSwap(SelectedItem, SelectedChar);
        }
        if (SelectedItem.Type == ItemData.Catagory.Chest)
        {
            PData.GearSwap(SelectedItem, SelectedChar);
        }
        if (SelectedItem.Type == ItemData.Catagory.Weapon)
        {
            PData.GearSwap(SelectedItem, SelectedChar);
        }
        QuickInfoShow(SelectedChar);
        SelectedItem = PData.SearchInventory(SelectedItem);
        GameObject ItemAmount;
        ItemAmount = GameObject.Find("ItemAmount");
        if (SelectedItem.Amount > 0)
        {
            ItemAmount.GetComponent<TMPro.TextMeshProUGUI>().SetText("Quantity: " + SelectedItem.Amount);
        }
        else
        {
            SelectedItem = Placeholder;
            GameObject.Find("Description").GetComponent<TMPro.TextMeshProUGUI>().SetText("");
            GameObject.Find("HpStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("Hp: ");
            GameObject.Find("MpStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("Mp: ");
            GameObject.Find("AtkStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("Atk: ");
            GameObject.Find("M.AtkStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("M.Atk: ");
            GameObject.Find("DefStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("Def: " );
            GameObject.Find("M.DefStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("M.Def: ");
            GameObject.Find("SpdStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("Spd: ");
            ItemAmount.GetComponent<TMPro.TextMeshProUGUI>().SetText("Quantity: ");
            GameObject.Find("ItemCharacters").GetComponent<TMPro.TextMeshProUGUI>().SetText("Used By: ");
            GameObject.Find("ItemType").GetComponent<TMPro.TextMeshProUGUI>().SetText("Type: ");
            GameObject.Find("ItemName").GetComponent<TMPro.TextMeshProUGUI>().SetText("");
        }
        Pouch1.GetComponent<Button>().interactable = false;
        Pouch2.GetComponent<Button>().interactable = false;
        Pouch3.GetComponent<Button>().interactable = false;
        Pouch4.GetComponent<Button>().interactable = false;
        Pouch5.GetComponent<Button>().interactable = false;
        Charm1.GetComponent<Button>().interactable = false;
        Charm2.GetComponent<Button>().interactable = false;
        GameObject.Find("EquipItemOnChar").transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("Equip Item on character");
        SortInv(InvFilter);
    }
    /// <summary>
    /// Calls use healing item method from Pdata and if that was the last one of an item and resets all of the GUI elements related to the item info
    /// </summary>
    public void UseHealItem()
    {
        PData.UseHealingItem(SelectedItem,SelectedChar);
        if (SelectedItem.Amount > 0)
        {
            GameObject.Find("ItemAmount").GetComponent<TMPro.TextMeshProUGUI>().SetText("Quantity: " + SelectedItem.Amount);
        }
        else
        {
            SelectedItem = Placeholder;
            GameObject.Find("Description").GetComponent<TMPro.TextMeshProUGUI>().SetText("");
            GameObject.Find("HpStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("Hp: ");
            GameObject.Find("MpStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("Mp: ");
            GameObject.Find("AtkStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("Atk: ");
            GameObject.Find("M.AtkStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("M.Atk: ");
            GameObject.Find("DefStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("Def: ");
            GameObject.Find("M.DefStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("M.Def: ");
            GameObject.Find("SpdStat").GetComponent<TMPro.TextMeshProUGUI>().SetText("Spd: ");
            GameObject.Find("ItemAmount").GetComponent<TMPro.TextMeshProUGUI>().SetText("Quantity: ");
            GameObject.Find("ItemCharacters").GetComponent<TMPro.TextMeshProUGUI>().SetText("Used By: ");
            GameObject.Find("ItemType").GetComponent<TMPro.TextMeshProUGUI>().SetText("Type: ");
            GameObject.Find("ItemName").GetComponent<TMPro.TextMeshProUGUI>().SetText("");
        }
        SortInv(InvFilter);
    }
    /// <summary>
    /// Shows character info in the party tab for each character
    /// </summary>
    public void SetupPartyInfo()
    {
        for (int i = 0; i < PData.characters.Count; i++)
        {
            int itemsInPouch = 0;
            if(PData.characters[i].position == 1)
            {
                QuickMemberinfo1.transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Name + "");
                QuickMemberinfo1.transform.GetChild(8).GetComponent<TMPro.TextMeshProUGUI>().SetText("LVL: " + PData.characters[i].Level);
                for (int p = 0; p < PData.characters[i].Pouch.Count; p++)
                {
                    if(PData.characters[i].Pouch[p].ID != 0)
                    {
                        itemsInPouch++;
                    }
                }
                QuickMemberinfo1.transform.GetChild(10).GetComponent<TMPro.TextMeshProUGUI>().SetText(itemsInPouch + " / 5");
                QuickMemberinfo1.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].CurrentMp + " / " + PData.characters[i].Stats.Mp);
                QuickMemberinfo1.transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].CurrentHp + " / " + PData.characters[i].Stats.Hp);
                QuickMemberinfo1.transform.GetChild(1).GetComponent<Slider>().value = (float)PData.characters[i].CurrentHp / (float)PData.characters[i].Stats.Hp;
                QuickMemberinfo1.transform.GetChild(2).GetComponent<Slider>().value = (float)PData.characters[i].CurrentMp / (float)PData.characters[i].Stats.Mp;
                QuickMemberinfo1.GetComponent<Image>().color = GetPartyMemberColor(PData.characters[i].Name);
            }

            if (PData.characters[i].position == 2)
            {
                QuickMemberinfo2.transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Name + "");
                QuickMemberinfo2.transform.GetChild(8).GetComponent<TMPro.TextMeshProUGUI>().SetText("LVL: " + PData.characters[i].Level);
                for (int p = 0; p < PData.characters[i].Pouch.Count; p++)
                {
                    if (PData.characters[i].Pouch[p].ID != 0)
                    {
                        itemsInPouch++;
                    }
                }
                QuickMemberinfo2.transform.GetChild(10).GetComponent<TMPro.TextMeshProUGUI>().SetText(itemsInPouch + " / 5");
                QuickMemberinfo2.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].CurrentMp + " / " + PData.characters[i].Stats.Mp);
                QuickMemberinfo2.transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].CurrentHp + " / " + PData.characters[i].Stats.Hp);
                QuickMemberinfo2.transform.GetChild(1).GetComponent<Slider>().value = (float)PData.characters[i].CurrentHp / (float)PData.characters[i].Stats.Hp;
                QuickMemberinfo2.transform.GetChild(2).GetComponent<Slider>().value = (float)PData.characters[i].CurrentMp / (float)PData.characters[i].Stats.Mp;
                QuickMemberinfo2.GetComponent<Image>().color = GetPartyMemberColor(PData.characters[i].Name);
            }

            if (PData.characters[i].position == 3)
            {
                QuickMemberinfo3.transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Name + "");
                QuickMemberinfo3.transform.GetChild(8).GetComponent<TMPro.TextMeshProUGUI>().SetText("LVL: " + PData.characters[i].Level);
                for (int p = 0; p < PData.characters[i].Pouch.Count; p++)
                {
                    if (PData.characters[i].Pouch[p].ID != 0)
                    {
                        itemsInPouch++;
                    }
                }
                QuickMemberinfo3.transform.GetChild(10).GetComponent<TMPro.TextMeshProUGUI>().SetText(itemsInPouch + " / 5");
                QuickMemberinfo3.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].CurrentMp + " / " + PData.characters[i].Stats.Mp);
                QuickMemberinfo3.transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].CurrentHp + " / " + PData.characters[i].Stats.Hp);
                QuickMemberinfo3.transform.GetChild(1).GetComponent<Slider>().value = (float)PData.characters[i].CurrentHp / (float)PData.characters[i].Stats.Hp;
                QuickMemberinfo3.transform.GetChild(2).GetComponent<Slider>().value = (float)PData.characters[i].CurrentMp / (float)PData.characters[i].Stats.Mp;
                QuickMemberinfo3.GetComponent<Image>().color = GetPartyMemberColor(PData.characters[i].Name);
            }

            if (PData.characters[i].position == 4)
            {
                QuickMemberinfo4.transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Name + "");
                QuickMemberinfo4.transform.GetChild(8).GetComponent<TMPro.TextMeshProUGUI>().SetText("LVL: " + PData.characters[i].Level);
                for (int p = 0; p < PData.characters[i].Pouch.Count; p++)
                {
                    if (PData.characters[i].Pouch[p].ID != 0)
                    {
                        itemsInPouch++;
                    }
                }
                QuickMemberinfo4.transform.GetChild(10).GetComponent<TMPro.TextMeshProUGUI>().SetText(itemsInPouch + " / 5");
                QuickMemberinfo4.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].CurrentMp + " / " + PData.characters[i].Stats.Mp);
                QuickMemberinfo4.transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].CurrentHp + " / " + PData.characters[i].Stats.Hp);
                QuickMemberinfo4.transform.GetChild(1).GetComponent<Slider>().value = (float)PData.characters[i].CurrentHp / (float)PData.characters[i].Stats.Hp;
                QuickMemberinfo4.transform.GetChild(2).GetComponent<Slider>().value = (float)PData.characters[i].CurrentMp / (float)PData.characters[i].Stats.Mp;
                QuickMemberinfo4.GetComponent<Image>().color = GetPartyMemberColor(PData.characters[i].Name);
            }
        }
    }
    /// <summary>
    /// Returns a colour based on the character that has been passed to the method
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public Color32 GetPartyMemberColor(ItemData.Character character)
    {
        switch (character)
        {
            case ItemData.Character.Seth:
                return new Color32(0, 133, 79, 255);
            case ItemData.Character.Susie:
                return new Color32(0, 70, 191, 255);
            case ItemData.Character.Shiana:
                return new Color32(191, 19, 0, 255);
            case ItemData.Character.Brody:
                return new Color32(131, 0, 191, 255);
            default:
                return new Color32(145, 145, 145, 255);
        }
    }
    /// <summary>
    /// Sets up all of the character info based on what character info has been clicked and updates the GUI based on that character's data
    /// </summary>
    /// <param name="position"></param>
    public void SetupCharInfo(int position)
    {
        for (int i = 0; i < PData.characters.Count; i++)
        {
            if(PData.characters[i].position == position)
            {
                CharacterInfoPanel.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().SetText("Level: " + PData.characters[i].Level);
                CharacterInfoPanel.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Name + "");
                CharacterInfoPanel.transform.GetChild(3).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Weapon.Name + "");
                CharacterInfoPanel.transform.GetChild(3).transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Charm1.Name + "");
                CharacterInfoPanel.transform.GetChild(3).transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Charm2.Name + "");
                CharacterInfoPanel.transform.GetChild(3).transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Chest.Name + "");
                CharacterInfoPanel.transform.GetChild(3).transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Legs.Name + "");
                CharacterInfoPanel.transform.GetChild(3).transform.GetChild(5).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Boots.Name + "");
                CharacterInfoPanel.transform.GetChild(4).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("Hp: " + PData.characters[i].Stats.Hp);
                CharacterInfoPanel.transform.GetChild(4).transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().SetText("Mp: " + PData.characters[i].Stats.Mp);
                CharacterInfoPanel.transform.GetChild(4).transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().SetText("Atk: " + PData.characters[i].Stats.Atk);
                CharacterInfoPanel.transform.GetChild(4).transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText("M.Atk: " + PData.characters[i].Stats.MAtk);
                CharacterInfoPanel.transform.GetChild(4).transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().SetText("Def: " + PData.characters[i].Stats.Def);
                CharacterInfoPanel.transform.GetChild(4).transform.GetChild(5).GetComponent<TMPro.TextMeshProUGUI>().SetText("M.Def: " + PData.characters[i].Stats.MDef);
                CharacterInfoPanel.transform.GetChild(4).transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>().SetText("Spd: " + PData.characters[i].Stats.Spd);
                CharacterInfoPanel.transform.GetChild(5).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Pouch[0].Name);
                CharacterInfoPanel.transform.GetChild(5).transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Pouch[1].Name);
                CharacterInfoPanel.transform.GetChild(5).transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Pouch[2].Name);
                CharacterInfoPanel.transform.GetChild(5).transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Pouch[3].Name);
                CharacterInfoPanel.transform.GetChild(5).transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Pouch[4].Name);
                CharacterInfoPanel.transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>().SetText("Hp: " + PData.characters[i].CurrentHp + " / " + PData.characters[i].Stats.Hp);
                CharacterInfoPanel.transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().SetText("Mp: " + PData.characters[i].CurrentMp + " / " + PData.characters[i].Stats.Mp);
            }
        }
    }
    /// <summary>
    /// Code for the main menu based on what button is clicked
    /// </summary>
    /// <param name="button"></param>
    public void MainMenuInput(string button)
    {
        switch (button)
        {
            //Starts the game
            case "Play":
                MainMenu.SetActive(false);
                Canvas.SetActive(false);
                Menu.SetActive(true);
                GameOver.SetActive(false);
                CombatMenu.SetActive(false);
                PData.LoadPlayerData();
                StateManager.State = StateManager.GameState.Overworld;
                SceneManager.LoadScene("Default");
                StateManager.ResetPlayerPos();
                break;
                //Shows the how to play screen
            case "HowToPlay":
                HowToPlayScreen.SetActive(true);
                break;
                //Quits the gaem
            case "Quit":
                Application.Quit();
                break;
                //Goes baack to main menu
            case "Back":
                HowToPlayScreen.SetActive(false);
                break;
                //If the player reached a game over it sends them back to the main menu screen
            case "GameOver":
                MainMenu.SetActive(true);
                Canvas.SetActive(true);
                Menu.SetActive(false);
                StateManager.State = StateManager.GameState.MainMenu;
                break;
            default:
                break;
        }
    }
}