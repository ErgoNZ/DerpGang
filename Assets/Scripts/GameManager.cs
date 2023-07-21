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
    public GameObject Menu;
    public List<Item> ItemList = new List<Item>();
    public GameState State;
    int ID = 0;
    bool MenuOpen = false;
    PlayerControler PlayerControler;
    public struct Item
    {
        public int ID;
        public string Name;
        public Catagory Type;
        public int Amount;
        public string Description;
        public Element Element;
        public Stats Stats;
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
        public EffectNames Name;
        public Element Element;
        public int Duration;
        public int Damage;
        public int MpDamage;
        public string Source;
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
        DontDestroyOnLoad(Menu);
        State = GameState.LoadingItems;
        PlayerControler = Player.GetComponent<PlayerControler>();
        LoadItems("Assets/ItemData/Consumables.txt");
        LoadItems("Assets/ItemData/Weapons.txt");
        LoadItems("Assets/ItemData/Armour.txt");
        LoadItems("Assets/ItemData/Charms.txt");
        for (int i = 0; i < ItemList.Count; i++)
        {
            Debug.LogWarning("ID:" + ItemList[i].ID.ToString().PadRight(10) + "Name: " + ItemList[i].Name.ToString());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuOpen = !MenuOpen;
            State = GameState.Overworld;
            Menu.SetActive(MenuOpen);
            if(MenuOpen == true)
            {
                State = GameState.Menu;
            }
        }
        PlayerControler.enabled = true;

        if (State == GameState.Menu)
        {
            PlayerControler.enabled = false;
        }
    }

    /// <summary>
    /// Loads all items into memory from the requested path
    /// </summary>
    /// <param name="path"></param>
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
        item.Resistance = new List<Element>();
        item.Vulnerable = new List<Element>();
        EffectData effectData;
        CharmData charmData;
        List<Character> characterList = new List<Character>();
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

            if(itemData[11] != "NULL")
            {
                effData = itemData[11].Split('|');
                for (int i = 0; i < effData.Length; i++)
                {
                    effectData = new EffectData();
                    processingArray = effData[i].Split('~');
                    effectData.Name = ParseEnum<EffectNames>(processingArray[0]);
                    effectData.Element = ParseEnum<Element>(processingArray[1]);
                    effectData.Duration = int.Parse(processingArray[2]);
                    effectData.Damage = int.Parse(processingArray[3]);
                    effectData.MpDamage = int.Parse(processingArray[4]);
                    effectData.Source = processingArray[5];
                    item.Effects.Add(effectData);
                }
            }

            if (itemData[12] != "NULL")
            {
                chrmData = itemData[12].Split('|');
                for (int i = 0; i < chrmData.Length; i++)
                {
                    charmData = new CharmData();
                    processingArray = chrmData[i].Split('~');
                    charmData.Name = processingArray[0];
                    charmData.CheckingFor = processingArray[1];
                    charmData.Trigger = ParseEnum<Trigger>(processingArray[2]);
                    item.CharmEffects.Add(charmData);
                }
            }

            resistance = itemData[13].Split('|');
            for (int i = 0; i < resistance.Length; i++)
            {
                item.Resistance.Add(ParseEnum<Element>(resistance[i]));
            }

            vunlerabilities = itemData[14].Split('|');
            for (int i = 0; i < vunlerabilities.Length; i++)
            {
                item.Vulnerable.Add(ParseEnum<Element>(vunlerabilities[i]));
            }

            characters = itemData[15].Split('|');
            for (int i = 0; i < characters.Length; i++)
            {
                characterList.Add(ParseEnum<Character>(characters[i]));
            }
            item.characters = characterList;
            ItemList.Add(item);
            Debug.LogWarning("Item has been stored!");
        }
        reader.Close();
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
        LoadingItems,
        Menu
    }
}
