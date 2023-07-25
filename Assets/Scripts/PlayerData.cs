using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerData : MonoBehaviour
{
    ItemData itemData;
    struct CharacterData
    {
        public int position;
        public ItemData.Character Name;
        public int Level;
        public ItemData.Stats Stats;
        public List<ItemData.Element> Resistance;
        public List<ItemData.Element> Vulnerable;
        public List<ItemData.EffectData> Effects;
        public ItemData.Item Chest;
        public ItemData.Item Legs;
        public ItemData.Item Boots;
        public ItemData.Item Weapon;
        public ItemData.Item Charm1;
        public ItemData.Item Charm2;
    }

    List<int> Flags;
    List<CharacterData> characters = new List<CharacterData>();
    List<ItemData.Item> Inventory = new List<ItemData.Item>();
    void LoadPlayerData(string Path)
    {
        StreamReader reader = new StreamReader(Path);
        int lineCount = 0;
        string line;
        string[] Array;
        CharacterData characterData = new CharacterData();
        while (!reader.EndOfStream)
        {
            if(lineCount <= 48)
            {
                if(lineCount%12 == 0 || lineCount == 0)
                {
                    characterData = new CharacterData();
                    characterData.Name = ItemData.ParseEnum<ItemData.Character>(reader.ReadLine());
                    characterData.position = int.Parse(reader.ReadLine());
                    characterData.Chest = itemData.GetItem(reader.ReadLine());
                    characterData.Legs = itemData.GetItem(reader.ReadLine());
                    characterData.Boots = itemData.GetItem(reader.ReadLine());
                    characterData.Weapon = itemData.GetItem(reader.ReadLine());
                    characterData.Charm1 = itemData.GetItem(reader.ReadLine());
                    characterData.Charm2 = itemData.GetItem(reader.ReadLine());
                    line = reader.ReadLine();
                    Array = line.Split('/');
                    characterData.Stats.Hp = int.Parse(Array[0]);
                    characterData.Stats.Mp = int.Parse(Array[1]);
                    characterData.Stats.Atk = int.Parse(Array[2]);
                    characterData.Stats.MAtk = int.Parse(Array[3]);
                    characterData.Stats.Def = int.Parse(Array[4]);
                    characterData.Stats.MDef = int.Parse(Array[5]);
                    characterData.Stats.Spd = int.Parse(Array[6]);
                    characters.Add(characterData);
                }
            }
            lineCount++;
        }
    }

    private void Start()
    {
        itemData = GetComponent<ItemData>();
        LoadPlayerData("Assets/Data/SaveData.txt");
    }
}
