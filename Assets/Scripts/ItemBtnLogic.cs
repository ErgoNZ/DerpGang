using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBtnLogic : MonoBehaviour
{
    public string txtAmount, txtName;
    public ItemData.Item item;
    public GameObject Name, Amount, Icon;
    private void Awake()
    {
        Name.GetComponent<TMPro.TextMeshProUGUI>().text = txtName;
        Amount.GetComponent<TMPro.TextMeshProUGUI>().text = txtAmount;
    }
    public void clicked()
    {

    }
}
