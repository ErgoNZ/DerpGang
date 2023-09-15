using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PlayerData : MonoBehaviour
{
    ItemData itemData;
    SkillData SkillData;
    public TextAsset playerData;
    public class CharacterData
    {
        public int position;
        public ItemData.Character Name;
        public int Level;
        public ItemData.Stats Stats;
        public int CurrentHp, CurrentMp;
        public List<ItemData.Element> Resistance;
        public List<ItemData.Element> Vulnerable;
        public List<ItemData.EffectData> StatusEffects;
        public ItemData.Item Chest;
        public ItemData.Item Legs;
        public ItemData.Item Boots;
        public ItemData.Item Weapon;
        public ItemData.Item Charm1;
        public ItemData.Item Charm2;
        public List<ItemData.Item> Pouch;
        public List<SkillData.Skill> Skills;
        public bool InParty;
    }
    public List<int> Flags = new();
    public List<CharacterData> characters = new();
    public List<ItemData.Item> Inventory = new();

    /// <summary>
    /// Searches the players inventory for an item and returns the item data if it is found in the inventory.
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
    /// <summary>
    /// Adds an item to the inventory based on id.
    /// It checks whether or not the player already has a item of the same id and if it does it just adds the amount values together.
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="Amount"></param>
    public void AddItem(int ID, int Amount)
    {
        ItemData.Item Item;
        //Checks the inventory to see if the a version of the item is already in the players inventory
        for (int i = 0; i < Inventory.Count; i++)
        {
            //If there is an item in the inventory with the same id, it adds the two amounts together.
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
    /// <summary>
    /// Deletes an item from the player's inventory based on id and amount.
    /// If the item amount drops to 0 it completely removes the item from your inventory.
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="Amount"></param>
    public void DelItem(int ID, int Amount)
    {
        //Search through the inventory 
        for (int i = 0; i < Inventory.Count; i++)
        {
            //If the id matches the item being removed
            if (Inventory[i].ID == ID)
            {
                //If removing the amount requested from the item doesn't result in it being 0 just remove the ammount needed.
                if(Inventory[i].Amount - Amount > 0)
                {
                    Inventory[i].Amount -= Amount;
                }
                //If removing the amount requested drops the amount to 0 or below remove the item from the inventory.
                else if(Inventory[i].Amount - Amount <= 0)
                {
                    Inventory.RemoveAt(i);
                }
            }
        }
    }
    /// <summary>
    /// Swaps an item out from a characters pouch with another selected item
    /// </summary>
    /// <param name="pouchSlot"></param>
    /// <param name="SelectedItem"></param>
    /// <param name="CharPos"></param>
    public void PouchSwapItems(int pouchSlot, ItemData.Item SelectedItem, int CharPos)
    {
        ItemData.Item TempItem;
        //Checks through each character
        for (int i = 0; i < characters.Count; i++)
        {
            //If the position of the character is the same as the targeted character
            if(characters[i].position == CharPos)
            {
                //Swap the items around
                TempItem = characters[i].Pouch[pouchSlot];
                characters[i].Pouch[pouchSlot] = SelectedItem;
                AddItem(TempItem.ID, 1);
                DelItem(SelectedItem.ID, 1);
            }
        }
    }
    /// <summary>
    /// Swaps a charm from a character with another selected charm
    /// </summary>
    /// <param name="Slot"></param>
    /// <param name="SelectedItem"></param>
    /// <param name="CharPos"></param>
    public void CharmSwap(int Slot, ItemData.Item SelectedItem, int CharPos)
    {
        ItemData.Item TempItem;
        //Checks through each character
        for (int i = 0; i < characters.Count; i++)
        {
            //If the position of the character is the same as the targeted character
            if (characters[i].position == CharPos)
            {
                //If charm slot 1 else slot 2
                if (Slot == 1)
                {
                    //Swap the items around
                    TempItem = characters[i].Charm1;
                    characters[i].Charm1 = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
                else
                {
                    //Swap the items around
                    TempItem = characters[i].Charm2;
                    characters[i].Charm1 = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
            }
        }
    }
    /// <summary>
    /// Swaps a piece of gear from a character with another piece of gear
    /// </summary>
    /// <param name="SelectedItem"></param>
    /// <param name="CharPos"></param>
    public void GearSwap(ItemData.Item SelectedItem, int CharPos)
    {
        ItemData.Item TempItem;
        //Checks through each character
        for (int i = 0; i < characters.Count; i++)
        {
            //If the position of the character is the same as the targeted character
            if (characters[i].position == CharPos)
            {
                //Each if just checks if the type of item matches the catergory required
                if(SelectedItem.Type == ItemData.Catagory.Weapon)
                {
                    //Swap the items around
                    TempItem = characters[i].Weapon;
                    characters[i].Weapon = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
                if (SelectedItem.Type == ItemData.Catagory.Chest)
                {
                    //Swap the items around
                    TempItem = characters[i].Chest;
                    characters[i].Chest = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
                if (SelectedItem.Type == ItemData.Catagory.Legs)
                {
                    //Swap the items around
                    TempItem = characters[i].Legs;
                    characters[i].Legs = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
                if (SelectedItem.Type == ItemData.Catagory.Boots)
                {
                    //Swap the items around
                    TempItem = characters[i].Boots;
                    characters[i].Boots = SelectedItem;
                    AddItem(TempItem.ID, 1);
                    DelItem(SelectedItem.ID, 1);
                }
            }
        }
    }
    /// <summary>
    /// Uses a healing item on the targeted character.
    /// This decreases the amount of the healing item by 1.
    /// </summary>
    /// <param name="HealingItem"></param>
    /// <param name="Target"></param>
    public void UseHealingItem(ItemData.Item HealingItem, int Target)
    {
        //Checks through all character and finds the character that has the position that matches the target positon
        for (int i = 0; i < characters.Count; i++)
        {
            if(characters[i].position == Target)
            {
                //Increase the characters stats by the items hp and mp values
                characters[i].CurrentHp += HealingItem.Stats.Hp;
                characters[i].CurrentMp += HealingItem.Stats.Mp;
                //If hp goes over the character max hp set it to the max hp
                if(characters[i].CurrentHp > characters[i].Stats.Hp)
                {
                    characters[i].CurrentHp = characters[i].Stats.Hp;
                }
                //If mp goes over the character max mp set it to the max mp
                if (characters[i].CurrentMp > characters[i].Stats.Mp)
                {
                    characters[i].CurrentMp = characters[i].Stats.Mp;
                }
                //Delete the item as it was used
                DelItem(HealingItem.ID, 1);
            }
        }
    }

    /// <summary>
    /// This loads in all of the player data from their save file
    /// </summary>
    /// <param name="Path"></param>
    public void LoadPlayerData()
    {
        Flags = new();
        characters = new();
        Inventory = new();
        try
        {
            //Splits the text files data by each line
            string[] saveData = playerData.text.Split(Environment.NewLine);
            int lineCount = 0;
            string line;
            string[] Array;
            //For each character's data that needs to be loaded
            for (int i = 0; i < 4; i++)
            {
                CharacterData characterData;
                characterData = new CharacterData();
                characterData.Skills = new();
                //Load data line by line and increment the linecount after each line is read
                characterData.Name = ItemData.ParseEnum<ItemData.Character>(SplitData(saveData[lineCount++]));
                characterData.Level = int.Parse(SplitData(saveData[lineCount++]));
                characterData.position = int.Parse(SplitData(saveData[lineCount++]));
                characterData.Chest = itemData.GetItem(SplitData(saveData[lineCount++]));
                characterData.Legs = itemData.GetItem(SplitData(saveData[lineCount++]));
                characterData.Boots = itemData.GetItem(SplitData(saveData[lineCount++]));
                characterData.Weapon = itemData.GetItem(SplitData(saveData[lineCount++]));
                characterData.Charm1 = itemData.GetItem(SplitData(saveData[lineCount++]));
                characterData.Charm2 = itemData.GetItem(SplitData(saveData[lineCount++]));
                line = SplitData(saveData[lineCount++]);
                //Splits the line where / is present to split the stats to be easier to load into memory
                Array = line.Split('/');
                characterData.Stats.Hp = int.Parse(Array[0]);
                characterData.Stats.Mp = int.Parse(Array[1]);
                characterData.Stats.Atk = int.Parse(Array[2]);
                characterData.Stats.Def = int.Parse(Array[3]);
                characterData.Stats.MAtk = int.Parse(Array[4]);
                characterData.Stats.MDef = int.Parse(Array[5]);
                characterData.Stats.Spd = int.Parse(Array[6]);
                characterData.CurrentHp = int.Parse(SplitData(saveData[lineCount++]));
                characterData.CurrentMp = int.Parse(SplitData(saveData[lineCount++]));
                characterData.Pouch = new();
                line = SplitData(saveData[lineCount++]);
                //Split to be easier to handle and add the item to the pounch
                Array = line.Split('/');
                for (int p = 0; p < Array.Length; p++)
                {
                    characterData.Pouch.Add(itemData.GetItem(Array[p]));
                }
                line = SplitData(saveData[lineCount++]);
                //Split the line to be easier to handle and load each skill listed
                Array = line.Split('/');
                for (int s = 0; s < Array.Length; s++)
                {
                    characterData.Skills.Add(SkillData.skillList[int.Parse(Array[s])]);
                }
                //Add that character's data to the character list
                characters.Add(characterData);
                Debug.Log("A character's data was loaded");
            }            
            //Splits the line into easier pieces to handle and loads the amount of each item into the players inventory
            line = SplitData(saveData[lineCount++]);
            Array = line.Split('/');
            for (int i = 0; i < Array.Length; i++)
            {
                string[] Array1;
                //Splits again to get the item id and amount of the item
                Array1 = Array[i].Split('|');
                AddItem(int.Parse(Array1[0]), int.Parse(Array1[1]));
                Debug.Log("Item from Inventory loaded");
            }
            //Splits the line so data is easier to handle
            line = SplitData(saveData[lineCount++]);
            Array = line.Split('/');
            //For each flag listed in the file add the number it is currently is to the flag list
            for (int i = 0; i < Array.Length; i++)
            {
                Flags.Add(int.Parse(Array[i]));
            }
        }
        catch (Exception Ex)
        {
            //This will only ever trip if the player has done a glitch/bug or has modified their file in an illegal way.
            Debug.Log("ILLEGAL DATA: PLAYER DATA IS INVALID!");
            Debug.Log(Ex.Message);
        }    
    }

    private void Start()
    {
        itemData = GetComponent<ItemData>();
        SkillData = GetComponent<SkillData>();
        SkillData.ReadSkillData();
        LoadPlayerData();
    }

    /// <summary>
    /// This splits the info data that makes the save file more readable.
    /// This function returns the actual data needed from the file.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    string SplitData(string line)
    {
        //Split remove garbage data that is just used to make editing the save file easier
        string[] Array = line.Split(':');
        line = Array[1];
        return line;
    }
}
