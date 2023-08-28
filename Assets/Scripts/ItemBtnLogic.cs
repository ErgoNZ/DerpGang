using UnityEngine;
using UnityEngine.UI;

public class ItemBtnLogic : MonoBehaviour
{
    public ItemData.Item item;
    public GameObject Name, Amount, Icon;
    GameObject ItemDesc, ItemHp, ItemAtk, ItemMp, ItemSpd, ItemMAtk, ItemMDef, ItemUseable, ItemAmount, ItemAtkEffect, ItemDefEffect, ItemDef, ItemType, ItemName, UseItem;
    string content;
    MenuLogic MenuLogic;

    public void clicked()
    {
        MenuLogic = GameObject.Find("GameManager").GetComponent<MenuLogic>();
        ItemDesc = GameObject.Find("Description");
        ItemHp = GameObject.Find("HpStat");
        ItemMp = GameObject.Find("MpStat");
        ItemAtk = GameObject.Find("AtkStat");
        ItemMAtk = GameObject.Find("M.AtkStat");
        ItemDef = GameObject.Find("DefStat");
        ItemMDef = GameObject.Find("M.DefStat");
        ItemSpd = GameObject.Find("SpdStat");
        ItemUseable = GameObject.Find("ItemCharacters");
        ItemType = GameObject.Find("ItemType");
        ItemAmount = GameObject.Find("ItemAmount");
        ItemName = GameObject.Find("ItemName");
        ItemDesc.GetComponent<TMPro.TextMeshProUGUI>().SetText(item.Description);
        ItemHp.GetComponent<TMPro.TextMeshProUGUI>().SetText("Hp: " + item.Stats.Hp);
        ItemMp.GetComponent<TMPro.TextMeshProUGUI>().SetText("Mp: " + item.Stats.Mp);
        ItemAtk.GetComponent<TMPro.TextMeshProUGUI>().SetText("Atk: " + item.Stats.Atk);
        ItemMAtk.GetComponent<TMPro.TextMeshProUGUI>().SetText("M.Atk: " + item.Stats.MAtk);
        ItemDef.GetComponent<TMPro.TextMeshProUGUI>().SetText("Def: " + item.Stats.Def);
        ItemMDef.GetComponent<TMPro.TextMeshProUGUI>().SetText("M.Def: " + item.Stats.MDef);
        ItemSpd.GetComponent<TMPro.TextMeshProUGUI>().SetText("Spd: " + item.Stats.Spd);
        ItemAmount.GetComponent<TMPro.TextMeshProUGUI>().SetText("Quantity: " + item.Amount);
        content = "Used By: ";
        for (int i = 0; i < item.characters.Count; i++)
        {
            if(i == item.characters.Count - 1)
            {
                content += item.characters[i].ToString();
            }
            else
            {
                content += item.characters[i] + ", ";
            }
        }
        ItemUseable.GetComponent<TMPro.TextMeshProUGUI>().SetText(content);
        ItemType.GetComponent<TMPro.TextMeshProUGUI>().SetText("Type: " + item.Type);
        ItemName.GetComponent<TMPro.TextMeshProUGUI>().SetText(item.Name);
        MenuLogic.SelectedItem = item;
        UseItem = GameObject.Find("UseItemBtn");
        UseItem.GetComponent<Button>().interactable = true;
    }
    public void FillInfo(string ItemAmount, string ItemName, ItemData.Item itemInfo)
    {
        Name.GetComponent<TMPro.TextMeshProUGUI>().SetText(ItemName);
        Amount.GetComponent<TMPro.TextMeshProUGUI>().SetText(ItemAmount);
        item = itemInfo;
    }
}
