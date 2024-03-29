using UnityEngine;
using UnityEngine.UI;

public class ItemBtnLogic : MonoBehaviour
{
    public ItemData.Item item;
    public GameObject Name, Amount, Icon;
    GameObject ItemDesc, ItemHp, ItemAtk, ItemMp, ItemSpd, ItemMAtk, ItemMDef, ItemUseable, ItemAmount, ItemAtkEffect, ItemDefEffect, ItemDef, ItemType, ItemName, UseItem;
    string content;
    MenuLogic MenuLogic;
    /// <summary>
    /// When the button is clicked changes all of the data in the item info section to the item data stored in the button
    /// </summary>
    public void clicked()
    {
        MenuLogic = GameObject.Find("GameManager").GetComponent<MenuLogic>();
        //The description can hold ~400 characters worth of text.
        ItemDesc = GameObject.Find("Description");
        //Changes all of the text areas to the info stored in the item data given so the player can easily see all of the data laid out nicely
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
        //This just adds all of the characters to the list for who can use them
        if(item.characters.Count != 4)
        {
            for (int i = 0; i < item.characters.Count; i++)
            {
                if (i == item.characters.Count - 1)
                {
                    content += item.characters[i].ToString();
                }
                else
                {
                    content += item.characters[i] + ", ";
                }
            }
        }
        else
        {
            content = "Used by: Everyone";
        }
        ItemUseable.GetComponent<TMPro.TextMeshProUGUI>().SetText(content);
        ItemType.GetComponent<TMPro.TextMeshProUGUI>().SetText("Type: " + item.Type);
        ItemName.GetComponent<TMPro.TextMeshProUGUI>().SetText(item.Name);
        MenuLogic.SelectedItem = item;
        if(item.Type != ItemData.Catagory.Key)
        {
            UseItem = GameObject.Find("UseItemBtn");
            UseItem.GetComponent<Button>().interactable = true;
        }
    }
    /// <summary>
    /// This method is called to fill in the text for the button so the player can see what item the button is associated with and how much of it they have
    /// </summary>
    /// <param name="ItemAmount"></param>
    /// <param name="ItemName"></param>
    /// <param name="itemInfo"></param>
    public void FillInfo(string ItemAmount, string ItemName, ItemData.Item itemInfo)
    {
        Name.GetComponent<TMPro.TextMeshProUGUI>().SetText(ItemName);
        Amount.GetComponent<TMPro.TextMeshProUGUI>().SetText(ItemAmount);
        item = itemInfo;
    }
}
