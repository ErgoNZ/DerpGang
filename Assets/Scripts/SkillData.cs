using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkillData : MonoBehaviour
{
    public List<Skill> skillList = new();
    public TextAsset Data;
    //This struct contatins the structure for the data needed for a skill to work.
    public struct Skill
    {
        public string Name;
        public int ID;
        public int MpCost;
        public int HpCost;
        public CostMode CostMode;
        public double PhysicalPower;
        public double MagicalPower;
        public List<ItemData.EffectData> effects;
        public ItemData.Range range;
        public int multihit;
        public string description;
        public ItemData.Element Element;
    }
    //Just an enum for the 2 cost modes of a skill
    public enum CostMode
    {
        Interger,
        Percentage
    }
    /// <summary>
    /// Reads all of the data from the skill textAsset and converts them into skills and adds them to the list.
    /// </summary>
    public void ReadSkillData()
    {
        try
        {
            //Takes the data from the skill text assest and splits them all into lines based on when a new line is started in the file.
            string[] linesFromFile = Data.text.Split(Environment.NewLine);
            string[] dataArray;
            string[] effectArray;
            //lineNum is set to 1 to skip the line that contains the template/header guide for making a skill.
            int lineNum = 1;
            //While there are still lines left in the text Assest file for all of the skills.
            while (lineNum < linesFromFile.Length)
            {
                Skill skill;
                skill.effects = new();
                //Splits the line into more useful chunks of information.
                dataArray = linesFromFile[lineNum].Split(',');
                //Pulls all of the data from the dataArray to create the skill's data.
                skill.Name = dataArray[0];
                skill.ID = lineNum;
                skill.MpCost = int.Parse(dataArray[1]);
                skill.HpCost = int.Parse(dataArray[2]);
                skill.CostMode = ParseEnum<CostMode>(dataArray[3]);
                skill.PhysicalPower = int.Parse(dataArray[4]);
                skill.MagicalPower = int.Parse(dataArray[5]);
                //Refer to ItemData for an explanation on how effects are added but the example is still below
                //Name~Element~Duration~Damage~MpDamage~Chance| Next effect...
                if (dataArray[6] != "NULL")
                {
                    //Splits again if there is more than one effect associated with a skill
                    effectArray = dataArray[6].Split('|');
                    for (int i = 0; i < effectArray.Length; i++)
                    {
                        ItemData.EffectData effect;
                        //Splits the effect data where the '~' character is present. Makes the data split into more useful pieces.
                        string[] effectInfo = effectArray[i].Split('~');
                        //Assign all of the useful data from the line to make up the effect and adds it to the effect list for the skill
                        effect.Name = effectInfo[0];
                        effect.Element = ParseEnum<ItemData.Element>(effectInfo[1]);
                        effect.Duration = int.Parse(effectInfo[2]);
                        effect.Damage = int.Parse(effectInfo[3]);
                        effect.MpDamage = int.Parse(effectInfo[4]);
                        effect.Chance = int.Parse(effectInfo[5]);
                        skill.effects.Add(effect);
                    }
                }
                skill.range = ParseEnum<ItemData.Range>(dataArray[7]);
                skill.multihit = int.Parse(dataArray[8]);
                skill.description = dataArray[9];
                skill.Element = ParseEnum<ItemData.Element>(dataArray[10]);
                //Adds the skill to the list and goes to the next line of data in the text assest file for skills.
                skillList.Add(skill);
                lineNum++;
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
        
    }
    /// <summary>
    /// This returns a skill's data based on the id requested.
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public Skill GetSkill(int id)
    {
        //Returns the requested skill's data.
        return skillList[id];
    }
    /// <summary>
    /// Custom parsing method that converts strings into enums.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}
