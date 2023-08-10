using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBtnLogic : MonoBehaviour
{
    public ItemData.Item item;
    public GameObject Name, Amount, Icon;

    public void clicked()
    {

    }
    public void FillInfo(string ItemAmount, string ItemName, ItemData.Item itemInfo)
    {
        Name.GetComponent<TMPro.TextMeshProUGUI>().SetText(ItemName);
        Amount.GetComponent<TMPro.TextMeshProUGUI>().SetText(ItemAmount);
        item = itemInfo;
    }
}
