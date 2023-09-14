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
        //Grabs gameObjects and any components that are used
        InfoArea = GameObject.Find("InfoArea");
        CombatLogic = GameObject.Find("GameManager").GetComponent<CombatLogic>();
    }
    /// <summary>
    /// Sets up the text for the skill btn prefab for the skill this script has been passed.
    /// </summary>
    public void SetUpText()
    {
        string cost;
        cost = Skill.MpCost + "Mp";
        //Show hp cost if there is one
        if(Skill.HpCost > 0)
        {
            cost = Skill.MpCost + "/" + Skill.HpCost; 
        }
        //If the cost is calculated by percentage show that it is a percentage
        if(Skill.MpCost > 0 && Skill.CostMode == SkillData.CostMode.Percentage)
        {
            cost = Skill.MpCost + "%/";
            if(Skill.HpCost > 0)
            {
                cost += Skill.HpCost + "%";
            }
        }
        //Sets text to the cost value and the name of the skill
        MpHp.GetComponent<TMPro.TextMeshProUGUI>().SetText(cost);
        Name.GetComponent<TMPro.TextMeshProUGUI>().SetText(Skill.Name);
    }
    /// <summary>
    /// When the object that has this script is hovered over shows the data from the skill
    /// In the info area so the player can get a better idea of what the skill does.
    /// </summary>
    public void OnHover()
    {
        string mp, hp, mainInfo = "";
        hp = "Hp Cost: " + Skill.HpCost + "\n";
        mp = "Mp Cost: " + Skill.MpCost + "\n";
        //Switches how mp/hp cost is shown if the skill is in percentage mode
        if (Skill.CostMode == SkillData.CostMode.Percentage)
        {
            hp = "Hp Cost: " + Skill.HpCost + "%"; 
            mp = "Mp Cost: " + Skill.MpCost + "%";
        }
        //Only shows mp or hp info to the player if the values are at least 1
        if(Skill.MpCost > 0)
        {
            mainInfo += mp;
        }
        if(Skill.HpCost > 0)
        {
            mainInfo += hp;
        }
        //Adds the power values to the description shown to the player
        if(Skill.PhysicalPower > 0)
        {
            mainInfo += "Physical Power: " + Skill.PhysicalPower + "\n";
        }
        if(Skill.MagicalPower > 0)
        {
            mainInfo += "Magical Power: " + Skill.MagicalPower + "\n";
        }
        //Adds the element, range and description to the info shown to the player.
        mainInfo += "Element: " + Skill.Element + "\n";
        mainInfo += "Range: " + Skill.range + "\n";
        mainInfo += Skill.description;
        //Set the text for the info box to the resulting string.
        InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(mainInfo);
    }
    /// <summary>
    /// Tells the main combat logic script what skill was picked by whether or not this skill button was clicked on
    /// </summary>
    public void Clicked()
    {
        //Calls SkillPicked method from the combat logic script and pass the skill this script holds.
        CombatLogic.SkillPicked(Skill);
    }
    /// <summary>
    /// If the player stop hovering this skill the info box is set to go blank.
    /// </summary>
    public void LeaveHover()
    {
        //Sets info box to be blank
        InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("");
    }
}
