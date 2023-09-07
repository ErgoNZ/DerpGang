using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatLogic : MonoBehaviour
{
    PlayerData PData;
    SkillData SkillData;
    StateManager StateManager;
    public GameObject[] CharacterActions = new GameObject[4];
    public List<Enemy> enemies = new();
    public List<SpeedInfo> TurnOrder;
    int CurrentActionBeingPicked = 0;
    public Sprite AttackIco, DefendIco, FleeIco, PouchIco, SkillIco, TActIco, EmptyIco;
    public GameObject InfoArea;
    CharacterAction ActionData = new();
    List<CharacterAction> CharacterActionList = new();

    public struct SpeedInfo
    {
        public int Speed;
        public int position;
    }
    public struct CharacterAction
    {
        public Action action;
        public List<ItemData.Character> neededCharacters;
        public SkillData.Skill Skill;
        public int target;
        public ItemData.Item Item;
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
        GetTurnOrder();
        NextCharacterAction();
    }
    public void PickAction(string Action)
    {
        switch (Action)
        {
            case "Attack":
                ToggleAttackMenu(true);
                break;
            case "Magic":

                break;
            case "Defend":
                ConfirmAction("Defend");
                break;
            case "TAct":

                break;
            case "Pouch":

                break;
            case "Flee":
                if (CanFlee())
                {
                    ConfirmAction("Flee");
                }
                break;
            default:
                break;
        }
    }
    public bool CanFlee()
    {
        int alivePartyMembers = 0;
        for (int i = 0; i < PData.characters.Count; i++)
        {
            if(PData.characters[i].CurrentHp > 0)
            {
                alivePartyMembers++;
            }
        }

        if(alivePartyMembers < 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void DisableFlee()
    {
        for (int i = 0; i < CharacterActions.Length; i++)
        {
            CharacterActions[i].transform.GetChild(6).GetComponent<Button>().interactable = false;
        }
    }
    public void ConfirmAction(string Action)
    {
        if(Action == "Flee")
        {
            int collectiveSpeed = 0;
            int enemySpeed = 0;
            double fleeChance;
            for (int i = 0; i < PData.characters.Count; i++)
            {
                if(PData.characters[i].CurrentHp > 0)
                {
                    collectiveSpeed += PData.characters[i].Stats.Spd;
                }
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if(enemies[i].CurrentHp > 0)
                {
                    enemySpeed += enemies[i].Stats.Spd;
                }
            }
            fleeChance = collectiveSpeed / enemySpeed;
            double roll = Random.Range(0, 100);
            if(roll <= fleeChance)
            {
                EndCombat(true);
            }
        }

        if(Action == "Attack")
        {
            DisableFlee();
            CurrentActionBeingPicked++;
            NextCharacterAction();
        }

        if(Action == "Defend")
        {
            CharacterActions[CurrentActionBeingPicked].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = DefendIco;
            CurrentActionBeingPicked++;
            NextCharacterAction();
        }
        CharacterActionList.Add(ActionData);
    }
    public void StartCombat()
    {
        FillCharacterInfo();
        GetTurnOrder();
    }
    public void RoundStart()
    {

    }
    public void RoundEnd()
    {
        GetTurnOrder();
    }
    public void NextCharacterAction()
    {
        if(CurrentActionBeingPicked < 4)
        {
            StartCoroutine(ActionBoxShow(CurrentActionBeingPicked));
        }
        if(CurrentActionBeingPicked > 0)
        {
            StartCoroutine(ActionBoxHide(CurrentActionBeingPicked - 1));
        }
    }
    IEnumerator ActionBoxShow(int ActionBox)
    {
        float timeElapsed = 0;
        float yPosition = CharacterActions[ActionBox].transform.localPosition.y;
        float xPosition = CharacterActions[ActionBox].transform.localPosition.x;
        float lerpDuration = 0.2f;
        float currentYPos = -740f;
        float targetYPos = -340f;
        while (timeElapsed < lerpDuration)
        {
            yPosition = Mathf.Lerp(currentYPos, targetYPos, timeElapsed / lerpDuration);
            CharacterActions[ActionBox].transform.localPosition = new Vector3(xPosition, yPosition);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        CharacterActions[ActionBox].transform.localPosition = new Vector3(xPosition, targetYPos);
        yield break;
    }

    IEnumerator ActionBoxHide(int ActionBox)
    {
        float timeElapsed = 0;
        float yPosition = CharacterActions[ActionBox].transform.localPosition.y;
        float xPosition = CharacterActions[ActionBox].transform.localPosition.x;
        float lerpDuration = 0.2f;
        float targetYPos = -740f;
        float currentYPos = -340f;
        while (timeElapsed < lerpDuration)
        {
            yPosition = Mathf.Lerp(currentYPos, targetYPos, timeElapsed / lerpDuration);
            CharacterActions[ActionBox].transform.localPosition = new Vector3(xPosition, yPosition);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        CharacterActions[ActionBox].transform.localPosition = new Vector3(xPosition, targetYPos);
        yield break;
    }
    public void EndCombat(bool fledFromCombat)
    {
        if(fledFromCombat == false)
        {

        }
    }
    public void ToggleAttackMenu(bool toggle)
    {
        GameObject AttackBtn, DefendBtn, FleeBtn, PouchBtn, TActBtn, SkillBtn, AttackMenu;
        AttackBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(1).gameObject;
        SkillBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(2).gameObject;
        DefendBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(3).gameObject;
        TActBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(4).gameObject;
        PouchBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(5).gameObject;
        FleeBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(6).gameObject;
        AttackMenu = CharacterActions[CurrentActionBeingPicked].transform.GetChild(7).gameObject;
        AttackBtn.SetActive(!toggle);
        SkillBtn.SetActive(!toggle);
        DefendBtn.SetActive(!toggle);
        TActBtn.SetActive(!toggle);
        PouchBtn.SetActive(!toggle);
        FleeBtn.SetActive(!toggle);
        AttackMenu.SetActive(toggle);
    }

    public void TargetPicked(int target)
    {
        ActionData = new();
        ActionData.action = Action.Attack;
        ActionData.target = target;
        for (int i = 0; i < PData.characters.Count; i++)
        {
            if (PData.characters[i].position == CurrentActionBeingPicked)
            {
                ActionData.neededCharacters.Add(PData.characters[i].Name);
            }
        }
        ConfirmAction("Attack");
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

    public void ButtonHover(string button)
    {
        if(button == "Flee" && CanFlee() == false)
        {
            InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("I can't just leave everyone here! There has to be something I can do!");
        }
        if(button == "TAct")
        {
            InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("Do we even have the coordination to do this?");
        }
    }

    public void ButtonLeave()
    {
        InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("");
    }
}
