using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ItemData : MonoBehaviour
{
    public List<Item> ItemList = new();
    public TextAsset Consumables, Weapons, Armour, Charms, Key;
    int id = 0;
    /// <summary>
    /// All structs and enums go towards making up all of the data used for 1 item.
    /// </summary>
    public class Item
    {
        public int ID;
        public string Name;
        public Catagory Type;
        public int Amount;
        public string Description;
        public Element Element;
        public Stats Stats;
        public Range Range;
        public List<EffectData> Effects;
        public List<CharmData> CharmEffects;
        public List<Element> Resistance;
        public List<Element> Vulnerable;
        public List<Character> characters;
    }
    public enum Character
    {
        Seth,
        Susie,
        Shiana,
        Brody,
        None
    }
    public enum Catagory
    {
        Consumable,
        Weapon,
        Chest,
        Legs,
        Boots,
        Charm,
        Gear,
        Key
    }
    public enum Range
    {
        Single,
        Wide,
        All,
        None
    }
    public struct EffectData
    {
        public string Name;
        public Element Element;
        public int Duration;
        public int Damage;
        public int MpDamage;
        //In percentages eg 25% being shown as 25
        public int Chance;
        //public Stats StatChanges;
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
    /// <summary>
    /// When this is awoken it loads all items into memory and list the items added.
    /// </summary>
    private void Awake()
    {
        //Loads all the items from each file type for the data for items.
        LoadItems(Consumables);
        LoadItems(Weapons);
        LoadItems(Armour);
        LoadItems(Charms);
        LoadItems(Key);
        //print the id and name of all items loaded into memory.
        for (int i = 0; i < ItemList.Count; i++)
        {
            Debug.Log("ID:" + ItemList[i].ID.ToString().PadRight(10) + "Name: " + ItemList[i].Name.ToString());
        }
    }

    /// <summary>
    /// Loads all items into memory from the requested path
    /// </summary>
    /// <param name="path"></param>
    private void LoadItems(TextAsset data)
    {
        try
        {
            string[] linesFromFile = data.text.Split(Environment.NewLine);
            string line;
            string[] itemData;
            string[] effData;
            string[] chrmData;
            string[] resistance;
            string[] vunlerabilities;
            string[] characters;
            string[] processingArray;
            int lineNum = 1;
            //Read lines until the end of the file
            while (lineNum < linesFromFile.Length)
            {
                Item item = new();
                item.Resistance = new List<Element>();
                item.Vulnerable = new List<Element>();
                EffectData effectData;
                CharmData charmData;
                List<Character> characterList = new();
                line = linesFromFile[lineNum];
                //Splits data into smaller pieces so they are easier to use
                itemData = line.Split(',');
                item.ID = id++;
                item.Name = itemData[0];
                item.Type = ParseEnum<Catagory>(itemData[1]);
                item.Description = itemData[2];
                item.Element = ParseEnum<Element>(itemData[3]);
                item.Stats.Hp = int.Parse(itemData[4]);
                item.Stats.Mp = int.Parse(itemData[5]);
                item.Stats.Atk = int.Parse(itemData[6]);
                item.Stats.Def = int.Parse(itemData[7]);
                item.Stats.MAtk = int.Parse(itemData[8]);
                item.Stats.MDef = int.Parse(itemData[9]);
                item.Stats.Spd = int.Parse(itemData[10]);

                if (itemData[11] != "NULL")
                {
                    //This is confusing to visualize so an example is given below
                    //Name~Element~Duration~Damage~MpDamage~Chance|Name~Element~Duration~Damage etc
                    //When a '|' character is used it signifies a new effect and the '~' character is what splits the data for that effect
                    //This could probably be implemented better but I'm not really going to do much about it unless it annoys me
                    effData = itemData[11].Split('|');
                    for (int i = 0; i < effData.Length; i++)
                    {
                        effectData = new EffectData();
                        //Split the data to make it easier to use
                        processingArray = effData[i].Split('~');
                        effectData.Name = processingArray[0];
                        effectData.Element = ParseEnum<Element>(processingArray[1]);
                        effectData.Duration = int.Parse(processingArray[2]);
                        effectData.Damage = int.Parse(processingArray[3]);
                        effectData.MpDamage = int.Parse(processingArray[4]);
                        effectData.Chance = int.Parse(processingArray[5]);
                        item.Effects.Add(effectData);
                    }
                }

                if (itemData[12] != "NULL")
                {
                    //Splits again if there is multiple pieces of charm data the item contains
                    chrmData = itemData[12].Split('|');
                    for (int i = 0; i < chrmData.Length; i++)
                    {
                        charmData = new CharmData();
                        //Split the data to make it easier to use
                        processingArray = chrmData[i].Split('~');
                        charmData.Name = processingArray[0];
                        charmData.CheckingFor = processingArray[1];
                        charmData.Trigger = ParseEnum<Trigger>(processingArray[2]);
                        item.CharmEffects.Add(charmData);
                    }
                }

                //Splits the data again if there are multiple of them
                resistance = itemData[13].Split('|');
                //Adds all of the data to the item
                for (int i = 0; i < resistance.Length; i++)
                {
                    item.Resistance.Add(ParseEnum<Element>(resistance[i]));
                }

                //Splits the data again if there are multiple of them
                vunlerabilities = itemData[14].Split('|');
                //Adds all of the data to the item
                for (int i = 0; i < vunlerabilities.Length; i++)
                {
                    item.Vulnerable.Add(ParseEnum<Element>(vunlerabilities[i]));
                }
                //Splits the data again if there are multiple of them
                characters = itemData[15].Split('|');
                //Adds all of the data to the item
                for (int i = 0; i < characters.Length; i++)
                {
                    characterList.Add(ParseEnum<Character>(characters[i]));
                }
                item.characters = characterList;
                item.Range = ParseEnum<Range>(itemData[16]);
                //Add the item to the list and go to the next line of the text assest
                ItemList.Add(item);
                lineNum++;
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }     
    }
    /// <summary>
    /// Custom parse method that converts strings into enums.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
    /// <summary>
    /// Returns with the data for an item to the script that called on the method.
    /// </summary>
    /// <param name="idString"></param>
    /// <returns></returns>
    public Item GetItem(string idString)
    {
        int id = int.Parse(idString);
        return ItemList[id];
    }
}
