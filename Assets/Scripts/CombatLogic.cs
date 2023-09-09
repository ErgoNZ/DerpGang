using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatLogic : MonoBehaviour
{
    PlayerData PData;
    StateManager StateManager;
    public GameObject SkillPrefab;
    List<GameObject> SkillPrefabList = new();
    public GameObject[] CharacterActions = new GameObject[4];
    public List<Enemy> enemies = new();
    public List<SpeedInfo> TurnOrder;
    int CurrentActionBeingPicked = 0;
    public Sprite AttackIco, DefendIco, FleeIco, PouchIco, SkillIco, TActIco, EmptyIco;
    public GameObject InfoArea;
    CharacterAction ActionData = new();
    List<CharacterAction> CharacterActionList = new();
    string actionType;
    public GameObject Canvas;
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
        public int itemSlot;
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
    }
    public void PickAction(string Action)
    {
        switch (Action)
        {
            case "Attack":
                actionType = "Attack";
                ToggleTargetMenu(true);
                break;
            case "Magic":
                actionType = "Magic";
                ToggleMagicMenu(true);
                break;
            case "Defend":
                actionType = "Defend";
                ConfirmAction("Defend");
                break;
            case "TAct":
                actionType = "TAct";
                break;
            case "Pouch":
                actionType = "Pouch";
                ToggleItemMenu(true);
                
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
                StartCoroutine(ActionBoxHide(0));
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
        if(Action == "Pouch")
        {
            CharacterActions[CurrentActionBeingPicked].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = PouchIco;
        }
        if(Action == "Attack")
        {
            CharacterActions[CurrentActionBeingPicked].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = AttackIco;
        }
        if(Action == "Defend")
        {
            ActionData.action = CombatLogic.Action.Defend;
            ActionData.target = CurrentActionBeingPicked + 1;
            AddCharacterToNeeded();
            CharacterActions[CurrentActionBeingPicked].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = DefendIco;
        }
        if(Action == "Magic")
        {
            CharacterActions[CurrentActionBeingPicked].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = SkillIco;
        }
        DisableFlee();
        CharacterActionList.Add(ActionData);
        CurrentActionBeingPicked++;
        NextCharacterAction();
        if(CurrentActionBeingPicked == 4)
        {
            for (int i = 0; i < CharacterActionList.Count; i++)
            {
                string line = "";
                line += "Action: " + CharacterActionList[i].action + "\n";
                line += "Needed Characters: " + CharacterActionList[i].neededCharacters.Count + "\n";
                line += "Target: " + CharacterActionList[i].target + "\n";
                if (CharacterActionList[i].Skill.Name != null)
                {
                    line += "Skill: " + CharacterActionList[i].Skill.Name + "\n";
                }
                if(CharacterActionList[i].Item != null)
                {
                    line += "Item: " + CharacterActionList[i].Item.Name;
                }
                Debug.Log(line);
            }
        }
    }
    public void StartCombat()
    {
        StateManager.State = StateManager.GameState.Combat; 
        Canvas.SetActive(true);
        GetTurnOrder();
        NextCharacterAction();
        FillCharacterInfo();
    }
    public void RoundStart()
    {
        CharacterActionList.Clear();
        for (int i = 0; i < PData.characters.Count; i++)
        {
            CharacterActions[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = EmptyIco;
        }
    }
    public void RoundEnd()
    {
        GetTurnOrder();
    }
    /// <summary>
    /// This processes all actions taken by the player.w
    /// Will also have enemies take their actions when their turn comes up in the turn order.
    /// </summary>
    public void ExecuteAllAction()
    {

    }
    /// <summary>
    /// Logic for an enemy taking their turn
    /// </summary>
    /// <param name="Enemy"></param>
    public void EnemyTakesAction(int Enemy)
    {

    }
    public void NextCharacterAction()
    {
        ActionData = new();
        if (CurrentActionBeingPicked < 4)
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
        float yPosition;
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
        float yPosition;
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
    IEnumerator InfoBoxToggle(bool toggle)
    {
        float timeElapsed = 0;
        float yPosition;
        float xPosition = InfoArea.transform.localPosition.x;
        float lerpDuration = 0.2f;
        float currentYPos = -740f;
        float targetYPos = -340f;
        if (!toggle)
        {
            currentYPos = -340f;
            targetYPos = -740f;
        }
        while (timeElapsed < lerpDuration)
        {
            yPosition = Mathf.Lerp(currentYPos, targetYPos, timeElapsed / lerpDuration);
            InfoArea.transform.localPosition = new Vector3(xPosition, yPosition);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        InfoArea.transform.localPosition = new Vector3(xPosition, targetYPos);
        yield break;
    }
    public void EndCombat(bool fledFromCombat)
    {
        if(fledFromCombat == false)
        {

        }
    }
    public void ToggleTargetMenu(bool toggle)
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
        for (int i = 0; i < 4; i++)
        {
            AttackMenu.transform.GetChild(i).GetComponent<Button>().interactable = true;
        }
        if (toggle)
        {
            if (actionType == "Pouch" && ActionData.Item.Element == ItemData.Element.Restore || actionType == "Magic" && ActionData.Skill.Element == ItemData.Element.Restore)
            {
                for (int i = 0; i < PData.characters.Count; i++)
                {
                    for (int p = 0; p < PData.characters.Count; p++)
                    {
                        if (PData.characters[p].position == i + 1)
                        {
                            //Replaces enemy names with party members names to show it targets party members
                            AttackMenu.transform.GetChild(i).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Name + "");
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if(i < enemies.Count)
                    {
                        AttackMenu.transform.GetChild(i).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(enemies[i].Name);
                    }
                    else
                    {
                        AttackMenu.transform.GetChild(i).GetComponent<Button>().interactable = false;
                    }
                }
            }
        }
    }
    public void ToggleItemMenu(bool toggle)
    {
        GameObject AttackBtn, DefendBtn, FleeBtn, PouchBtn, TActBtn, SkillBtn, ItemMenu;
        AttackBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(1).gameObject;
        SkillBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(2).gameObject;
        DefendBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(3).gameObject;
        TActBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(4).gameObject;
        PouchBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(5).gameObject;
        FleeBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(6).gameObject;
        ItemMenu = CharacterActions[CurrentActionBeingPicked].transform.GetChild(8).gameObject;
        AttackBtn.SetActive(!toggle);
        SkillBtn.SetActive(!toggle);
        DefendBtn.SetActive(!toggle);
        TActBtn.SetActive(!toggle);
        PouchBtn.SetActive(!toggle);
        FleeBtn.SetActive(!toggle);
        ItemMenu.SetActive(toggle);
        ItemMenuSetup(ItemMenu);
        StartCoroutine(InfoBoxToggle(toggle));
    }
    public void ItemMenuSetup(GameObject ItemMenu)
    {
        GameObject[] ItemBtns = new GameObject[5];
        ItemBtns[0] = ItemMenu.transform.GetChild(0).gameObject;
        ItemBtns[1] = ItemMenu.transform.GetChild(1).gameObject;
        ItemBtns[2] = ItemMenu.transform.GetChild(2).gameObject;
        ItemBtns[3] = ItemMenu.transform.GetChild(3).gameObject;
        ItemBtns[4] = ItemMenu.transform.GetChild(4).gameObject;

        for (int i = 0; i < PData.characters.Count; i++)
        {
            if(PData.characters[i].position == CurrentActionBeingPicked + 1)
            {
                ItemBtns[0].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Pouch[0].Name);
                ItemBtns[1].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Pouch[1].Name);
                ItemBtns[2].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Pouch[2].Name);
                ItemBtns[3].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Pouch[3].Name);
                ItemBtns[4].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(PData.characters[i].Pouch[4].Name);
                for (int b = 0; b < ItemBtns.Length; b++)
                {
                    ItemBtns[b].GetComponent<Button>().interactable = true;
                    if (PData.characters[i].Pouch[b].ID == 0)
                    {
                        ItemBtns[b].GetComponent<Button>().interactable = false;
                    }
                }
            }
        }
    }
    public void ItemPicked(int slot)
    {
        for (int i = 0; i < PData.characters.Count; i++)
        {
            if (PData.characters[i].position == CurrentActionBeingPicked + 1)
            {
                ActionData.neededCharacters = new();
                ActionData.Item = PData.characters[i].Pouch[slot];
                ActionData.neededCharacters.Add(PData.characters[i].Name);
                ToggleTargetMenu(true);
                ToggleItemMenu(false);
            }
        }
    }
    public void TargetPicked(int target)
    {
        ActionData.target = target;
        AddCharacterToNeeded();
        if(actionType == "Pouch")
        {
            ActionData.action = Action.Pouch;
            ToggleTargetMenu(false);
            ConfirmAction("Pouch");
        }
        else if (actionType == "Magic")
        {
            ActionData.action = Action.Magic;
            ToggleTargetMenu(false);
            ConfirmAction("Magic");
        }
        else
        {
            ActionData.action = Action.Attack;
            ConfirmAction("Attack");
        }
    }
    public void ToggleMagicMenu(bool toggle)
    {
        GameObject AttackBtn, DefendBtn, FleeBtn, PouchBtn, TActBtn, SkillBtn, SkillMenu;
        RectTransform SkillRect;
        SkillPrefabList.Clear();
        AttackBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(1).gameObject;
        SkillBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(2).gameObject;
        DefendBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(3).gameObject;
        TActBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(4).gameObject;
        PouchBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(5).gameObject;
        FleeBtn = CharacterActions[CurrentActionBeingPicked].transform.GetChild(6).gameObject;
        SkillMenu = CharacterActions[CurrentActionBeingPicked].transform.GetChild(9).gameObject;
        SkillRect = CharacterActions[CurrentActionBeingPicked].transform.GetChild(9).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        AttackBtn.SetActive(!toggle);
        SkillBtn.SetActive(!toggle);
        DefendBtn.SetActive(!toggle);
        TActBtn.SetActive(!toggle);
        PouchBtn.SetActive(!toggle);
        FleeBtn.SetActive(!toggle);
        SkillMenu.SetActive(toggle);
        StartCoroutine(InfoBoxToggle(toggle));
        if (toggle)
        {
            for (int i = 0; i < PData.characters.Count; i++)
            {
                if (PData.characters[i].position == CurrentActionBeingPicked + 1)
                {
                    SkillRect.sizeDelta = new(SkillRect.sizeDelta.x, 66 * PData.characters[i].Skills.Count);
                    for (int s = 0; s < PData.characters[i].Skills.Count; s++)
                    {
                        SkillPrefabList.Add(Instantiate(SkillPrefab));
                        SkillPrefabList[s].transform.SetParent(CharacterActions[i].transform.GetChild(9).GetChild(1).GetChild(0).GetChild(0).GetChild(0).transform);
                        SkillPrefabList[s].GetComponent<SkillBtnLogic>().Skill = PData.characters[i].Skills[s];
                        SkillPrefabList[s].GetComponent<SkillBtnLogic>().SetUpText();
                        SkillPrefabList[s].transform.localPosition = new(0, (float)(SkillRect.rect.yMax - (66.66 * s) - 34.47));
                        SkillPrefabList[s].transform.localScale = new(1, 1, 1);
                        if(PData.characters[i].CurrentMp < SkillPrefabList[s].GetComponent<SkillBtnLogic>().Skill.MpCost)
                        {
                            SkillPrefabList[s].GetComponent<Button>().interactable = false;
                        }
                    }
                }
            }
        }
    }
    public void SkillPicked(SkillData.Skill skill)
    {
        ActionData.Skill = skill;
        ToggleMagicMenu(false);
        if(skill.range == ItemData.Range.All)
        {
            AddCharacterToNeeded();
            ConfirmAction("Magic");
        }
        else
        {
            ToggleTargetMenu(true);
        }
    }
    public void AddCharacterToNeeded()
    {
        ActionData.neededCharacters = new();
        for (int i = 0; i < PData.characters.Count; i++)
        {
            if (PData.characters[i].position == CurrentActionBeingPicked + 1)
            {
                ActionData.neededCharacters.Add(PData.characters[i].Name);
            }
        }
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
            StartCoroutine(InfoBoxToggle(true));
            InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("I can't just leave everyone here! There has to be something I can do!");
        }
        if(button == "TAct")
        {
            StartCoroutine(InfoBoxToggle(true));
            InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("Do we even have the coordination to do this?");
        }
    }
    public void ButtonLeave()
    {
        if(InfoArea.transform.localPosition.y > -740)
        {
            StartCoroutine(InfoBoxToggle(false));
        }
        InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("");
    }
}