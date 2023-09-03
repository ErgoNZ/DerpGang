using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PlayerData : MonoBehaviour
{
    StateManager StateManager;
    ItemData itemData;
    int Money = 0;
    public class CharacterData
    {
        public int position;
        public ItemData.Character Name;
        public int Level;
        public ItemData.Stats Stats;
        public int CurrentHp, CurrentMp;
        public List<ItemData.Element> Resistance;
        public List<ItemData.Element> Vulnerable;
        public List<ItemData.EffectData> Effects;
        public ItemData.Item Chest;
        public ItemData.Item Legs;
        public ItemData.Item Boots;
        public ItemData.Item Weapon;
        public ItemData.Item Charm1;
        public ItemData.Item Charm2;
        public List<ItemData.Item> Pouch;
    }

    public List<int> Flags = new List<int>();
    public List<CharacterData> characters = new();
    public List<ItemData.Item> Inventory = new();

    /// <summary>
    /// THIS IS A METHOD USED BY THE UI!
    /// NEVER USE IT IN ANY OTHER CASE FOR RIGHT NOW!
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public ItemData.Item SearchInventory(ItemData.Item item)
    {
        for (int i = 0; i < Inventory.Count; i++)
        {
            if (Inventory[i].ID == item.ID)
            {
                return Inventory[i];
            }
        }
        item.Amount = 0;
        return item;
    }

    public void AddItem(int ID, int Amount)
    {
        ItemData.Item Item;
        for (int i = 0; i < Inventory.Count; i++)
        {
            if (Inventory[i].ID == ID)
            {
                Item = Inventory[i];
                Item.Amount = Inventory[i].Amount + Amount;
                Inventory[i] = Item;
                return;
            }
        }
        Item = itemData.GetItem(ID.ToString());
        Item.Amount = Amount;
        Inventory.Add(Item);
    }

    public void DelItem(int ID, int Amount)
    {
        for (int i = 0; i < Inventory.Count; i++)
        {
            if (Inventory[i].ID == ID)
            {
                if(Inventory[i].Amount - Amount > 0)
                {
                    Inventory[i].Amount -= Amount;
                }
                else if(Inventory[i].Amount - Amount <= 0)
                {
                    Inventory.RemoveAt(i);
                }
            }
        }
    }

    public void PouchSwapItems(int pouchSlot, ItemData.Item SelectedItem, int CharPos)
    {
        ItemData.Item TempItem;
        for (int i = 0; i < characters.Count; i++)
        {
            if(characters[i].position == CharPos)
            {
                TempItem = characters[i].Pouch[pouchSlot];
                characters[i].Pouch[pouchSlot] = SelectedItem;
                AddItem(TempItem.ID, 1);
                DelItem(SelectedItem.ID, 1);
            }
        }
    }

    public void CharmSwap(int Slot, ItemData.Item SelectedItem, int CharPos)
    {
        ItemData.Item TempItem;
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].position == CharPos)
            {
                if (Slot == 1)
                {
                    TempItem = characters[i].Charm1;
                    characters[i].Charm1 = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
                else
                {
                    TempItem = characters[i].Charm2;
                    characters[i].Charm1 = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
            }
        }
    }

    public void GearSwap(ItemData.Item SelectedItem, int CharPos)
    {
        ItemData.Item TempItem;
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].position == CharPos)
            {
                if(SelectedItem.Type == ItemData.Catagory.Weapon)
                {
                    TempItem = characters[i].Weapon;
                    characters[i].Weapon = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
                if (SelectedItem.Type == ItemData.Catagory.Chest)
                {
                    TempItem = characters[i].Chest;
                    characters[i].Chest = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
                if (SelectedItem.Type == ItemData.Catagory.Legs)
                {
                    TempItem = characters[i].Legs;
                    characters[i].Legs = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
                if (SelectedItem.Type == ItemData.Catagory.Boots)
                {
                    TempItem = characters[i].Boots;
                    characters[i].Boots = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
            }
        }
    }

    public void UseHealingItem(ItemData.Item HealingItem, int Target)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if(characters[i].position == Target)
            {
                characters[i].CurrentHp += HealingItem.Stats.Hp;
                characters[i].CurrentMp += HealingItem.Stats.Mp;
                if(characters[i].CurrentHp > characters[i].Stats.Hp)
                {
                    characters[i].CurrentHp = characters[i].Stats.Hp;
                }

                if (characters[i].CurrentMp > characters[i].Stats.Mp)
                {
                    characters[i].CurrentMp = characters[i].Stats.Mp;
                }

                if (StateManager.InCombat)
                {
                    //Combat specific stuff goes here
                }
                DelItem(HealingItem.ID, 1);
            }
        }
    }

    /// <summary>
    /// This loads in all of the player data from their save file
    /// </summary>
    /// <param name="Path"></param>
    void LoadPlayerData(string Path)
    {
        StreamReader reader = new StreamReader(Path);
        try
        {
            int lineCount = 0;
            string line;
            string[] Array;
            CharacterData characterData;
            while (!reader.EndOfStream)
            {
                if (lineCount != 52)
                {
                    if (lineCount % 13 == 0 || lineCount == 0)
                    {
                        characterData = new CharacterData();
                        characterData.Name = ItemData.ParseEnum<ItemData.Character>(SplitData(reader.ReadLine()));
                        characterData.Level = int.Parse(SplitData(reader.ReadLine()));
                        characterData.position = int.Parse(SplitData(reader.ReadLine()));
                        characterData.Chest = itemData.GetItem(SplitData(reader.ReadLine()));
                        characterData.Legs = itemData.GetItem(SplitData(reader.ReadLine()));
                        characterData.Boots = itemData.GetItem(SplitData(reader.ReadLine()));
                        characterData.Weapon = itemData.GetItem(SplitData(reader.ReadLine()));
                        characterData.Charm1 = itemData.GetItem(SplitData(reader.ReadLine()));
                        characterData.Charm2 = itemData.GetItem(SplitData(reader.ReadLine()));
                        line = SplitData(reader.ReadLine());
                        Array = line.Split('/');
                        characterData.Stats.Hp = int.Parse(Array[0]);
                        characterData.Stats.Mp = int.Parse(Array[1]);
                        characterData.Stats.Atk = int.Parse(Array[2]);
                        characterData.Stats.Def = int.Parse(Array[3]);
                        characterData.Stats.MAtk = int.Parse(Array[4]);
                        characterData.Stats.MDef = int.Parse(Array[5]);
                        characterData.Stats.Spd = int.Parse(Array[6]);
                        characterData.CurrentHp = int.Parse(SplitData(reader.ReadLine()));
                        characterData.CurrentMp = int.Parse(SplitData(reader.ReadLine()));
                        characterData.Pouch = new();
                        line = SplitData(reader.ReadLine());
                        Array = line.Split('/');
                        for (int i = 0; i < Array.Length; i++)
                        {
                            characterData.Pouch.Add(itemData.GetItem(Array[i]));
                        }
                        characters.Add(characterData);
                        Debug.Log("A character's data was loaded");
                        lineCount += 13;
                    }
                }
                else
                {
                    Money = int.Parse(SplitData(reader.ReadLine()));
                    line = SplitData(reader.ReadLine());
                    Array = line.Split('/');
                    for (int i = 0; i < Array.Length; i++)
                    {
                        string[] Array1;
                        Array1 = Array[i].Split('|');
                        AddItem(int.Parse(Array1[0]), int.Parse(Array1[1]));
                        Debug.Log("Item from Inventory loaded");
                    }
                    line = SplitData(reader.ReadLine());
                    Array = line.Split('/');
                    for (int i = 0; i < Array.Length; i++)
                    {
                        Flags.Add(int.Parse(Array[i]));
                    }
                }
            }
            reader.Close();
        }
        catch (Exception Ex)
        {
            //This will only ever trip if the player has done a glitch/bug or has modified their file in an illegal way.
            Debug.Log("ILLEGAL DATA: PLAYER DATA IS INVALID!");
            Debug.Log(Ex.Message);
            reader.Close();
        }    
    }

    private void Start()
    {
        itemData = GetComponent<ItemData>();
        StateManager = GetComponent<StateManager>();
        LoadPlayerData("Assets/Data/SaveData.txt");
    }

    /// <summary>
    /// This splits the info data that makes the save file more readable.
    /// This function returns the actual data needed from the file.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    string SplitData(string line)
    {
        string[] Array = line.Split(':');
        line = Array[1];
        return line;
    }
}
