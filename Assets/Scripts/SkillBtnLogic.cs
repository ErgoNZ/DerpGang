using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBtnLogic : MonoBehaviour
{
    public SkillData.Skill Skill;
    public CombatLogic CombatLogic;
    GameObject InfoArea;
    public GameObject MpHp, Name;
    private void Start()
    {
        InfoArea = GameObject.Find("InfoArea");
        CombatLogic = GameObject.Find("GameManager").GetComponent<CombatLogic>();
    }
    public void SetUpText()
    {
        string Cost;
        Cost = Skill.MpCost + "Mp";
        if(Skill.HpCost > 0)
        {
            Cost = Skill.MpCost + "/" + Skill.HpCost; 
        }
        if(Skill.MpCost > 0 && Skill.CostMode == SkillData.CostMode.Percentage)
        {
            Cost = Skill.MpCost + "%/";
            if(Skill.HpCost > 0)
            {
                Cost += Skill.HpCost + "%";
            }
        }
        MpHp.GetComponent<TMPro.TextMeshProUGUI>().SetText(Cost);
        Name.GetComponent<TMPro.TextMeshProUGUI>().SetText(Skill.Name);
    }
    public void OnHover()
    {
        string Mp, Hp, MainInfo = "";
        Hp = "Hp Cost: " + Skill.HpCost + "\n";
        Mp = "Mp Cost: " + Skill.MpCost + "\n";
        if (Skill.CostMode == SkillData.CostMode.Percentage)
        {
            Hp = "Hp Cost: " + Skill.HpCost + "%"; 
            Mp = "Mp Cost: " + Skill.MpCost + "%";
        }
        if(Skill.MpCost > 0)
        {
            MainInfo += Mp;
        }
        if(Skill.HpCost > 0)
        {
            MainInfo += Hp;
        }
        if(Skill.PhysicalPower > 0)
        {
            MainInfo += "Physical Power: " + Skill.PhysicalPower + "\n";
        }
        if(Skill.MagicalPower > 0)
        {
            MainInfo += "Magical Power: " + Skill.MagicalPower + "\n";
        }
        MainInfo += "Element: " + Skill.Element + "\n";
        MainInfo += "Range: " + Skill.range + "\n";
        MainInfo += Skill.description;
        InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(MainInfo);
    }
    public void Clicked()
    {
        CombatLogic.SkillPicked(Skill);
    }
    public void LeaveHover()
    {
        InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("");
    }
}
