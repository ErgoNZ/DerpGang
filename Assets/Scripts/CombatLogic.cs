using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatLogic : MonoBehaviour
{
    PlayerData PData;
    SkillData SkillData;
    StateManager StateManager;
    GameObject[] CharacterActions = new GameObject[4];
    public List<Enemy> enemies = new();
    public List<SpeedInfo> TurnOrder;

    public struct SpeedInfo
    {
        public int Speed;
        public int position;
    }
    public struct CharacterAction
    {
        Action action;
        List<ItemData.Character> neededCharacters;
        SkillData.Skill Skill;
        int target;
        ItemData.Item Item;
    }
    public enum Action
    {
        Attack,
        Magic,
        Defend,
        TAct,
        Pouch,
        Flee
    }
    public class Enemy
    {
        public string Name;
        public ItemData.Stats Stats;
        public int position;
        public int CurrentHp;
        public int CurrentMp;
        public ItemData.Element BasicAtkElement;
        public ItemData.Element Resistance;
        public ItemData.Element vulnerable;
    }
    // Start is called before the first frame update
    void Start()
    {
        PData = GetComponent<PlayerData>();
        StateManager = GetComponent<StateManager>();
        CharacterActions[0] = GameObject.Find("CharacterChoices1");
        CharacterActions[1] = GameObject.Find("CharacterChoices2");
        CharacterActions[2] = GameObject.Find("CharacterChoices3");
        CharacterActions[3] = GameObject.Find("CharacterChoices4");
        GetTurnOrder();
    }

    private void Update()
    {

    }
    public void StartCombat()
    {
        FillCharacterInfo();
        GetTurnOrder();
    }

    public void RoundEnd()
    {
        GetTurnOrder();
    }

    public void GetTurnOrder()
    {
        TurnOrder = new();
        SpeedInfo speedInfo;
        for (int i = 0; i < PData.characters.Count; i++)
        {
            speedInfo.position = PData.characters[i].position;
            speedInfo.Speed = PData.characters[i].Stats.Spd;
            TurnOrder.Add(speedInfo);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            speedInfo.position = enemies[i].position;
            speedInfo.Speed = enemies[i].Stats.Spd;
            TurnOrder.Add(speedInfo);
        }
        TurnOrder.Sort((s1, s2) => s1.Speed.CompareTo(s2.Speed));
        TurnOrder.Reverse();
    }

    public void FillCharacterInfo()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int p = 0; p < 4; p++)
            {
                if(PData.characters[p].position == i + 1)
                {
                    CharacterActions[i].transform.GetChild(0).transform.GetChild(1).GetComponent<Slider>().value = (float)PData.characters[p].CurrentHp / (float)PData.characters[p].Stats.Hp;
                    CharacterActions[i].transform.GetChild(0).transform.GetChild(2).GetComponent<Slider>().value = (float)PData.characters[p].CurrentMp / (float)PData.characters[p].Stats.Mp;
                    CharacterActions[i].transform.GetChild(0).transform.GetChild(5).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[p].CurrentHp + " / " + PData.characters[p].Stats.Hp);
                    CharacterActions[i].transform.GetChild(0).transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[p].CurrentMp + " / " + PData.characters[p].Stats.Mp);
                    CharacterActions[i].transform.GetChild(0).transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[p].Name + "");
                }
            }
        }
    }
}
