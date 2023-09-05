using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkillData : MonoBehaviour
{
    List<Skill> skillData;
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
    }
    public enum CostMode
    {
        Interger,
        Percentage
    }
    private void Start()
    {
        
    }

    public void ReadSkillData()
    {

    }

    public Skill GetSkill(int ID)
    {
        return skillData[ID];
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}
