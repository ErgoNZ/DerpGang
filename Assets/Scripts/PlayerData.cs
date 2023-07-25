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
    List<CharacterData> characters;
    void LoadPlayerData(string Path)
    {
        StreamReader reader = new StreamReader(Path);
        int lineCount = 0;
        string line;
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
                    characterData.Chest = itemData.GetItem(int.Parse(reader.ReadLine()));
                    characters.Add(characterData);
                }
            }
            lineCount++;
        }
    }

    private void Start()
    {
        LoadPlayerData("Assets/Data/SaveData.txt");
    }
}
