using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerData : MonoBehaviour
{
    struct CharacterData
    {
        int position;
        Character Name;
        Stats Stats;
        public List<Element> Resistance;
        public List<Element> Vulnerable;
        public List<EffectData> Effects;
        public Item Chest;
        public Item Legs;
        public Item Boots;
        public Item Weapon;
        public Item Charm1;
        public Item Charm2;
    }

    public struct Item
    {
        public int ID;
        public string Name;
        public GearType Type;
        public int Amount;
        public string Description;
        public Element Element;
        public Stats Stats;
        public List<CharmData> CharmEffects;
        public List<Element> Resistance;
        public List<Element> Vulnerable;
        public List<Character> characters;
    }
    public struct Stats
    {
        public int Hp;
        public int Mp;
        public int Atk;
        public int Def;
        public int MAtk;
        public int MDef;
        public int Spd;
    }
    public enum Element
    {
        Water,
        Fire,
        Ice,
        Poison,
        True,
        Earth,
        Restore,
        Electric,
        None
    }
    public enum Character
    {
        Seth,
        Susie,
        Shiana,
        Brody
    }

    public enum GearType
    {
        Weapon,
        Chest,
        Legs,
        Boots,
        Charm
    }

    public struct CharmData
    {
        public string Name;
        public string CheckingFor;
        public Trigger Trigger;
    }

    public enum Trigger
    {
        OnHit,
        RoundStart,
        HpCheck,
        EnemyCheck,
        StatusCheck,
        CombatStart
    }

    public struct EffectData
    {
        public string Name;
        public Element Element;
        public int Duration;
        public int Damage;
        public int MpDamage;
        public string Source;
    }

    List<int> Flags;
    List<CharacterData> characters;
    void LoadPlayerData(string Path)
    {
        StreamReader reader = new StreamReader(Path);

    }

    private void Start()
    {
        LoadPlayerData("Assets/Data/SaveData.txt");
    }
}
