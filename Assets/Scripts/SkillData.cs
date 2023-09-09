using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkillData : MonoBehaviour
{
    public List<Skill> skillList = new();
    public TextAsset Data;
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
    public enum CostMode
    {
        Interger,
        Percentage
    }
    public void ReadSkillData()
    {
        string[] linesFromFile = Data.text.Split(Environment.NewLine);
        string[] dataArray;
        string[] effectArray;
        int lineNum = 1;
        while (lineNum < linesFromFile.Length)
        {
            Skill skill;
            skill.effects = new();
            dataArray = linesFromFile[lineNum].Split(',');
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
                effectArray = dataArray[6].Split('|');
                for (int i = 0; i < effectArray.Length; i++)
                {
                    ItemData.EffectData effect;
                    string[] effectInfo = effectArray[i].Split('~');
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
            skillList.Add(skill);
            lineNum++;
        }
    }

    public Skill GetSkill(int ID)
    {
        return skillList[ID];
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}
