using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Diagnostics;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject PlayerPrefab;
    static GameObject Player;
    public List<Item> ItemList = new List<Item>();
    public GameState State;
    public struct Item
    {
        public int ID;
        public string Name;
        public Catagory Type;
        public int Amount;
        public string Description;
        public Element Element;
        public Stats Stats;
        public List<EffectData> ?Effects;
        public List<CharmData> ?CharmEffects;
        public List<Element> ?Resistance;
        public List<Element> ?Vulnerable;
        public List<Character> characters;
    }
    public enum Character
    {
        Seth,
        Susie,
        Shiana,
        Brody
    }
    public enum Catagory
    {
        Consumable,
        Weapon,
        Armour,
        Charm
    }
    public struct EffectData
    {
        EffectNames Name;
        Element Element;
        int Duration;
        int Damage;
        int MpDamage;
        string Source;
    }

    public enum EffectNames
    {
        Burn,
        Flamable,
        Soaked
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
        string Name;
        string CheckingFor;
        Trigger Trigger;
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

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Player = Instantiate(PlayerPrefab);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Player);
        State = GameState.LoadingItems;   
        LoadItems("Assets/ItemData/Consumables.txt");
        LoadItems("Assets/ItemData/Weapons.txt");
        LoadItems("Assets/ItemData/Armour.txt");
        LoadItems("Assets/ItemData/Charms.txt");
    }

    private void LoadItems(string path)
    {
        StreamReader reader = new StreamReader(path);
        string line;
        string[] itemData;
        string[] effData;
        string[] chrmData;
        string[] resistance;
        string[] vunlerabilities;
        string[] characters;
        string[] processingArray;
        Item item = new Item();
        EffectData effectData;
        CharmData charmData;
        List<Character> characterList = new List<Character>();
        int ID = 0;
        reader.ReadLine();
        while (!reader.EndOfStream)
        {
            line = reader.ReadLine();
            itemData = line.Split(',');
            item.ID = ID++;
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
            /*effData = itemData[11].Split('|');
            chrmData = itemData[12].Split('|');
            resistance = itemData[13].Split('|');
            vunlerabilities = itemData[14].Split('|');*/
            characters = itemData[15].Split('|');
            for (int i = 0; i < characters.Length; i++)
            {
                characterList.Add(ParseEnum<Character>(characters[i]));
            }
            item.characters = characterList;
            ItemList.Add(item);
            Debug.LogWarning("Item has been stored!");
        }
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public enum GameState{
        Overworld,
        CombatStart,
        CharacterTurn1,
        CharacterTurn2,
        CharacterTurn3,
        CharacterTurn4,
        EnemyTurn,
        Defeat,
        Victory,
        Cutscene,
        LoadingItems
    }
}
