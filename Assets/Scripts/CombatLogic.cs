using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CombatLogic : MonoBehaviour
{
    PlayerData PData;
    StateManager StateManager;
    ItemData ItemData;
    PlayerControler PlayerControler;
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
    public GameObject[] CharacterSprites = new GameObject[4];
    public GameObject[] HpUpdate = new GameObject[8];
    public int CharactersDowned = 0;
    public int enemiesDowned = 0;
    public GameObject CombatScene;
    public GameObject Inventory;
    public GameObject GameOverScreen;
    public GameObject[] EnemyIcons = new GameObject[4]; 
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
        public int position;
    }
    public enum Action
    {
        Attack,
        Magic,
        Defend,
        TAct,
        Pouch,
        Flee,
        Downed
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
        public List<SkillData.Skill> skills;
    }
    // Start is called before the first frame update
    void Start()
    {
        PData = GetComponent<PlayerData>();
        StateManager = GetComponent<StateManager>();
        ItemData = GetComponent<ItemData>();
        PlayerControler = GameObject.Find("Player(Clone)").GetComponent<PlayerControler>();
    }
    /// <summary>
    /// Sets the action to be equal to the button that was pressed and either confirms the action or opens another menu because the option needs more info to work.
    /// </summary>
    /// <param name="Action"></param>
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
    /// <summary>
    /// This method checks if there are at least 2 party members alive (hp above 0) and returns true if that is the case
    /// </summary>
    /// <returns></returns>
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
        //If only 1 person is left standing return false otherwise return true
        if(alivePartyMembers < 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    /// <summary>
    /// This method turns off all flee buttons
    /// </summary>
    public void DisableFlee()
    {
        for (int i = 0; i < CharacterActions.Length; i++)
        {
            CharacterActions[i].transform.GetChild(6).GetComponent<Button>().interactable = false;
        }
    }
    /// <summary>
    /// This method confirms the action the player took with the options they picked and adds it to the action list for when everyone takes their turns
    /// </summary>
    /// <param name="Action"></param>
    public void ConfirmAction(string Action)
    {
        if(Action == "Flee")
        {
            int collectiveSpeed = 0;
            int enemySpeed = 0;
            double fleeChance;
            //Finds the collective speed of all of the party members
            for (int i = 0; i < PData.characters.Count; i++)
            {
                if(PData.characters[i].CurrentHp > 0)
                {
                    collectiveSpeed += PData.characters[i].Stats.Spd;
                }
                //Skips everyones turn to attempt to flee
                StartCoroutine(ActionBoxHide(0));
                CharacterActions[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = FleeIco;
                CurrentActionBeingPicked = 3;
            }
            //Finds the collective speed of the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if(enemies[i].CurrentHp > 0)
                {
                    enemySpeed += enemies[i].Stats.Spd;
                }
            }
            //Find the percentage chance for the party to flee
            fleeChance = (collectiveSpeed / enemySpeed) * 100;
            double roll = Random.Range(0, 100);
            //if flee chance is higher than the roll end combat by fleeing
            if(roll <= fleeChance)
            {
                EndCombat(true);
            }
        }
        if(Action == "Pouch")
        {
            //Set the characters action icon to show a pouch
            CharacterActions[CurrentActionBeingPicked].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = PouchIco;
        }
        if(Action == "Attack")
        {
            //Show the attack icon for the characters action
            CharacterActions[CurrentActionBeingPicked].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = AttackIco;
        }
        if(Action == "Defend")
        {
            //Sets up the data needed for the defend action and sets the action icon to the defend icon
            ActionData.action = CombatLogic.Action.Defend;
            ActionData.target = CurrentActionBeingPicked + 1;
            AddCharacterToNeeded();
            CharacterActions[CurrentActionBeingPicked].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = DefendIco;
        }
        if(Action == "Magic")
        {
            //Sets the action icon to be the skill/magic icon
            CharacterActions[CurrentActionBeingPicked].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = SkillIco;
        }
        //Disable flee as everyone needs to be free to flee
        DisableFlee();
        //Set the position for the action data to be the characters positon
        ActionData.position = CurrentActionBeingPicked + 1;
        if(Action != "Flee")
        {
            //Add the characters action data to the list if the selected action isn't flee
            CharacterActionList.Add(ActionData);
        }
        //Move to picking the next character's action
        CurrentActionBeingPicked++;
        NextCharacterAction();
        //If all actions have been picked
        if(CurrentActionBeingPicked == 4)
        {
            //for each action print the info out to console
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
            //Start processing all actions picked
            StartCoroutine(ExecuteAllAction());
        }
    }
    /// <summary>
    /// Sets everything up at the start of combat so everything is in its default state
    /// </summary>
    public void StartCombat()
    {
        StateManager.State = StateManager.GameState.Combat;
        CurrentActionBeingPicked = 0;
        enemiesDowned = 0;
        CharactersDowned = 0;
        CharacterActionList.Clear();
        InfoArea.transform.position = new(InfoArea.transform.position.x, -740f);
        Canvas.SetActive(true);
        Inventory.SetActive(false);
        CombatScene.SetActive(true);
        for (int i = 0; i < EnemyIcons.Length; i++)
        {
            EnemyIcons[i].GetComponent<Image>().color = new(255, 255, 255, 0);
        }
        for (int i = 0; i < CharacterActions.Length; i++)
        {
            CharacterActions[PData.characters[i].position - 1].transform.GetChild(0).GetComponent<Image>().color = GetPartyMemberColor(PData.characters[i].Name);
            CharacterActions[i].transform.localPosition = new(CharacterActions[i].transform.localPosition.x, -740f);
        }
        for (int i = 0; i < PData.characters.Count; i++)
        {
            CharacterActions[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = EmptyIco;
            CharacterActions[i].transform.GetChild(6).GetComponent<Button>().interactable = true;
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyIcons[i].GetComponent<Image>().color = new(255,255,255,255);
        }
        GetTurnOrder();
        NextCharacterAction();
        FillCharacterInfo();
    }
    /// <summary>
    /// When it is the start of the round check who is downed, clear all character's action icons
    /// </summary>
    public void RoundStart()
    {
        //clear the previouslt selected actions
        CharacterActionList.Clear();
        for (int i = 0; i < PData.characters.Count; i++)
        {
            CharacterActions[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = EmptyIco;
            CharacterActions[i].transform.GetChild(6).GetComponent<Button>().interactable = true;
            //Checking if any party members are down
            if(PData.characters[i].CurrentHp <= 0)
            {
                for (int d = 0; d < 4; d++)
                {
                    if(PData.characters[i].position == d + 1)
                    {
                        //Change the party members menu colour to be their downed colour
                        CharacterActions[i].transform.GetChild(0).GetComponent<Image>().color = GetDownPartyMemberColor(PData.characters[i].Name);
                    }
                }
            }
        }
        enemiesDowned = 0;
        //Counts the amount of enemies killed
        for (int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i].CurrentHp < 1)
            {
                enemiesDowned++;
                EnemyIcons[i].GetComponent<Image>().color = new(255, 255, 255, 0);
            }
        }
        //If enemies downed is equal to the amount of enemies in the combat encounter end the combat
        if(enemiesDowned == enemies.Count)
        {
            EndCombat(false);
            return;
        }
        //Get turn order again just in case it has changed and go back to picking each character's action
        GetTurnOrder();
        CurrentActionBeingPicked = 0;
        NextCharacterAction();
        FillCharacterInfo();
    }
    /// <summary>
    /// When the round ends find the new turn order
    /// </summary>
    public void RoundEnd()
    {
        GetTurnOrder();
    }
    /// <summary>
    /// This processes all actions taken by the player.
    /// Will also have enemies take their actions when their turn comes up in the turn order.
    /// </summary>
    IEnumerator ExecuteAllAction()
    {
        //Characters who are defending at the start of the round
        enemiesDowned = 0;
        for (int i = 0; i < CharacterActionList.Count; i++)
        {
            if(CharacterActionList[i].action == Action.Defend)
            {
                for (int c = 0; c < PData.characters.Count; c++)
                {
                    if (CharacterActionList[i].position == PData.characters[c].position)
                    {
                        PData.characters[c].Stats.Def = PData.characters[c].Stats.Def * 2;
                    }
                }
                yield return new WaitForSecondsRealtime(1);
            }
        }

        for (int i = 0; i < PData.characters.Count + enemies.Count; i++)
        {
            //If turn is currently a party members
            if(TurnOrder[i].position < 5)
            {
                //For each character in the party who has taken an action
                for (int a = 0; a < CharacterActionList.Count; a++)
                {
                    //Checks which characters turn it is
                     if(TurnOrder[i].position == CharacterActionList[a].position)
                     {
                        switch (CharacterActionList[a].action)
                        {
                            case Action.Attack:
                                yield return StartCoroutine(BasicAttackOnEnemy(PData.characters[a], CharacterActionList[a].target - 1));
                                break;
                            case Action.Magic:
                                //Checks whether or not the skill being used is set to interger mode or percentage mode
                                if(CharacterActionList[a].Skill.CostMode == SkillData.CostMode.Interger)
                                {
                                    PData.characters[a].CurrentMp -= CharacterActionList[a].Skill.MpCost;
                                    PData.characters[a].CurrentHp -= CharacterActionList[a].Skill.HpCost;
                                }
                                else
                                {
                                    PData.characters[a].CurrentMp -= PData.characters[a].Stats.Mp * CharacterActionList[a].Skill.MpCost / 100;
                                    PData.characters[a].CurrentHp -= PData.characters[a].Stats.Hp * CharacterActionList[a].Skill.HpCost / 100;
                                }
                                yield return StartCoroutine(MagicalAttackOnEnemy(PData.characters[a], CharacterActionList[a].target - 1, CharacterActionList[a].Skill));
                                break;
                            case Action.TAct:
                                //Not implemented into the game yet
                                break;
                            case Action.Pouch:
                                yield return StartCoroutine(UseItemOnAlly(CharacterActionList[a].target, CharacterActionList[a].Item, CharacterActionList[a].position, CharacterActionList[a].itemSlot));
                                break;
                            default:
                                break;
                        }
                     }
                }
            }
            else
            {
                //if it is the enemies turn and they are currently alive do their action
                if(enemies[TurnOrder[i].position - 5].CurrentHp > 0)
                {
                    yield return StartCoroutine(EnemyTakesAction(TurnOrder[i].position - 5));
                }
                else
                {
                    enemiesDowned++;
                }
            }
            //If all enemies are dead, end the combat
            if(enemiesDowned == enemies.Count)
            {
                EndCombat(false);
                yield break;
            }
            yield return new WaitForSeconds(0.2f);
            FillCharacterInfo();
        }
        //Characters who are defending at end of round have their stats go back to normal and have their icon for their action removed
        for (int i = 0; i < CharacterActionList.Count; i++)
        {
            if (CharacterActionList[i].action == Action.Defend)
            {
                CharacterSprites[i].GetComponent<Image>().sprite = null;
                for (int c = 0; c < CharacterActionList.Count; c++)
                {
                    if (CharacterActionList[i].position == PData.characters[c].position)
                    {
                        PData.characters[c].Stats.Def = PData.characters[c].Stats.Def / 2;
                    }
                }
            }
        }
        //Called round start to start the new round
        RoundStart();
        yield break;
    }
    /// <summary>
    /// Logic for an enemy taking their turn
    /// </summary>
    /// <param name="Enemy"></param>
    IEnumerator EnemyTakesAction(int EnemyPos)
    {
        int ActionBeingTaken = 1;
        int Target;
        //checking if all characters are downed
        if(CharactersDowned < 4)
        {
            switch (ActionBeingTaken)
            {
                //Attack
                case 1:
                    //pick a random target to attack and if they are dead pick a new target/action to take
                    Target = Random.Range(1, 5);
                    for (int i = 0; i < PData.characters.Count; i++)
                    {
                        if (PData.characters[i].position == Target && PData.characters[i].CurrentHp > 0)
                        {
                            yield return StartCoroutine(BasicAttackOnParty(EnemyPos, i));
                        }
                        else if (PData.characters[i].position == Target && PData.characters[i].CurrentHp == 0)
                        {
                            CheckWhoIsDowned();
                            StartCoroutine(EnemyTakesAction(EnemyPos));
                        }
                    }
                    break;
                //Use Skill
  /*              case 2:
                    Target = Random.Range(1, 5);
                    if (enemies[EnemyPos].skills != null)
                    {
                        for (int i = 0; i < PData.characters.Count; i++)
                        {
                            if (PData.characters[i].position == Target && PData.characters[i].CurrentHp > 0)
                            {
                                yield return StartCoroutine(MagicalAttackOnParty(EnemyPos, i, enemies[EnemyPos]));
                            }
                            else if (PData.characters[i].position == Target && PData.characters[i].CurrentHp == 0)
                            {
                                StartCoroutine(EnemyTakesAction(EnemyPos));
                            }
                        }
                    }
                    else
                    {
                        StartCoroutine(EnemyTakesAction(EnemyPos));
                    }
                    break;
                //Use Item (Not implemented)
                *//*case 3:
                    break;*/
                default:
                    break;
            }
        }
        //If all characters are downed start the game over sequence
        else if(CharactersDowned >= 4)
        {
            GameOver();
        }
        yield break;
    }
    /// <summary>
    /// Attacks an enemy using a basic attack
    /// </summary>
    /// <param name="character"></param>
    /// <param name="Target"></param>
    /// <returns></returns>
    IEnumerator BasicAttackOnEnemy(PlayerData.CharacterData character, int Target)
    {
        //Run damage calcualtion on the enemy and play the damage animation
        float Damage = 0f;
        double RandomMultiplier = Random.Range(0.9f,1.1f);
        int CritRoll = Random.Range(0, 21);
        Damage += (character.Stats.Atk * character.Stats.Atk) / (character.Stats.Atk + enemies[Target].Stats.Def) * (float)RandomMultiplier;
        if(CritRoll == 20)
        {
            Damage = Damage * 1.5f;
            Debug.Log("Crit!");
        }
        Damage = Mathf.Round(Damage);
        enemies[Target].CurrentHp -= (int)Damage;
        yield return StartCoroutine(HpUpdateAnimation(Target + 4, "Damage", (int)Damage));
        Debug.Log(enemies[Target].CurrentHp + "/" + enemies[Target].Stats.Hp + "\n" + character.Name + " Dmg delt: " + Damage);
        yield return true;
    }
    /// <summary>
    /// Attacks enemies using a skill/magic
    /// </summary>
    /// <param name="character"></param>
    /// <param name="Target"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    IEnumerator MagicalAttackOnEnemy(PlayerData.CharacterData character, int Target, SkillData.Skill skill)
    {
        //If the skill is multi-hit run the damage calc multiple times
        for (int i = 0; i < skill.multihit; i++)
        {
            //If the range is for a single target hit only 1 target
            if(skill.range == ItemData.Range.Single)
            {
                //Run the damage calcualation and play the damge animation
                float Damage = 0f;
                double RandomMultiplier = Random.Range(0.9f, 1.1f);
                int CritRoll = Random.Range(0, 21);
                Damage += (character.Stats.Atk * character.Stats.Atk) / (character.Stats.Atk + enemies[Target].Stats.Def) * (float)RandomMultiplier * ((float)skill.PhysicalPower / 100);
                Damage += (character.Stats.MAtk * character.Stats.MAtk) / (character.Stats.MAtk + enemies[Target].Stats.MDef) * (float)RandomMultiplier * ((float)skill.MagicalPower / 100);
                if (CritRoll == 20)
                {
                    Damage = Damage * 1.5f;
                    Debug.Log("Crit!");
                }
                Damage = Mathf.Round(Damage);
                enemies[Target].CurrentHp -= (int)Damage;
                yield return StartCoroutine(HpUpdateAnimation(Target + 4, "Damage", (int)Damage));
                Debug.Log(enemies[Target].CurrentHp + "/" + enemies[Target].Stats.Hp + "\n" + character.Name + " Dmg delt: " + Damage);
            }
            //If the range is for an AOE attack hit 3 targets (the target originally picked and the ones next to them)
            if(skill.range == ItemData.Range.Wide)
            {
                for (int e = -1; e < 2; e++)
                {
                    if (Target + e >= 0 && Target + e < enemies.Count)
                    {
                        //Run the damage calcualation and play the damge animation for each enemy hit
                        float Damage = 0f;
                        double RandomMultiplier = Random.Range(0.9f, 1.1f);
                        int CritRoll = Random.Range(0, 21);
                        Damage += (character.Stats.Atk * character.Stats.Atk) / (character.Stats.Atk + enemies[Target + e].Stats.Def) * (float)RandomMultiplier * ((float)skill.PhysicalPower / 100);
                        Damage += (character.Stats.MAtk * character.Stats.MAtk) / (character.Stats.MAtk + enemies[Target + e].Stats.MDef) * (float)RandomMultiplier * ((float)skill.MagicalPower / 100);
                        if (CritRoll == 20)
                        {
                            Damage = Damage * 1.5f;
                            Debug.Log("Crit!");
                        }
                        Damage = Mathf.Round(Damage);
                        enemies[Target + e].CurrentHp -= (int)Damage;
                        yield return StartCoroutine(HpUpdateAnimation(Target+e+4, "Damage", (int)Damage));
                        Debug.Log(enemies[Target + e].CurrentHp + "/" + enemies[Target + e].Stats.Hp + "\n" + character.Name + " Dmg delt: " + Damage);
                    }
                }
            }
            //hits all enemy currently on screen
            if(skill.range == ItemData.Range.All)
            {
                for (int e = 0; e < enemies.Count; e++)
                {
                    //Run the damage calcualation and play the damge animation
                    float Damage = 0f;
                    double RandomMultiplier = Random.Range(0.9f, 1.1f);
                    int CritRoll = Random.Range(0, 21);
                    Damage += (character.Stats.Atk * character.Stats.Atk) / (character.Stats.Atk + enemies[e].Stats.Def) * (float)RandomMultiplier * ((float)skill.PhysicalPower / 100);
                    Damage += (character.Stats.MAtk * character.Stats.MAtk) / (character.Stats.MAtk + enemies[e].Stats.MDef) * (float)RandomMultiplier * ((float)skill.MagicalPower / 100);
                    if (CritRoll == 20)
                    {
                        Damage = Damage * 1.5f;
                        Debug.Log("Crit!");
                    }
                    Damage = Mathf.Round(Damage);
                    enemies[e].CurrentHp -= (int)Damage;
                    yield return StartCoroutine(HpUpdateAnimation(e + 4, "Damage", (int)Damage));
                    Debug.Log(enemies[e].CurrentHp + "/" + enemies[e].Stats.Hp + "\n" + character.Name + " Dmg delt: " + Damage);
                }
            }          
        }

    }
    /// <summary>
    /// Uses an item on an ally based on the paramaters given
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="item"></param>
    /// <param name="CharacterUsingItem"></param>
    /// <param name="ItemSlot"></param>
    /// <returns></returns>
    IEnumerator UseItemOnAlly(int Target, ItemData.Item item, int CharacterUsingItem, int ItemSlot)
    {
        for (int i = 0; i < PData.characters.Count; i++)
        {
            //Find the character positon and compare it to the target and make sure they are alive
            if(PData.characters[i].position == Target && PData.characters[i].CurrentHp > 0)
            {
                //Heal the characters hp and mp
                PData.characters[i].CurrentHp += item.Stats.Hp;
                if (PData.characters[i].CurrentHp > PData.characters[i].Stats.Hp)
                {
                    //if over max hp set character's hp to their max
                    PData.characters[i].CurrentHp = PData.characters[i].Stats.Hp;
                }
                //Play the healing animation
                yield return StartCoroutine(HpUpdateAnimation(Target - 1, "Healing", item.Stats.Hp));
                PData.characters[i].CurrentMp += item.Stats.Mp;
                if (PData.characters[i].CurrentMp > PData.characters[i].Stats.Mp)
                {
                    //if character's mp is above max set it to max
                    PData.characters[i].CurrentMp = PData.characters[i].Stats.Mp;
                }
                //Removes the item from the character who used it. (removes the item from their pouch)
                for (int c = 0; c < PData.characters.Count; c++)
                {
                    if(PData.characters[c].position == CharacterUsingItem)
                    {
                        //Set the pouch slot to be empty
                        PData.characters[c].Pouch[ItemSlot] = ItemData.GetItem("0");
                    }
                }
            }
        }
    }
    /// <summary>
    /// Attacks a party member using a basic attack
    /// </summary>
    /// <param name="EnemyPos"></param>
    /// <param name="Target"></param>
    /// <returns></returns>
    IEnumerator BasicAttackOnParty(int EnemyPos, int Target)
    {
        //Run the damage calcualation and play the damge animation
        float Damage = 0f;
        double RandomMultiplier = Random.Range(0.9f, 1.1f);
        int CritRoll = Random.Range(0, 21);
        Damage += (enemies[EnemyPos].Stats.Atk * enemies[EnemyPos].Stats.Atk) / (enemies[EnemyPos].Stats.Atk + PData.characters[Target].Stats.Def) * (float)RandomMultiplier;
        if (CritRoll == 20)
        {
            Damage = Damage * 1.5f;
            Debug.Log("Crit!");
        }
        Damage = Mathf.Round(Damage);
        PData.characters[Target].CurrentHp -= (int)Damage;
        yield return StartCoroutine(HpUpdateAnimation(Target, "Damage", (int)Damage));
        //Sets the character's hp to 0 if their hp goes below 0s
        if(PData.characters[Target].CurrentHp < 0)
        {
            PData.characters[Target].CurrentHp = 0;
        }
        Debug.Log(PData.characters[Target].CurrentHp + "/" + PData.characters[Target].Stats.Hp + "\n" + enemies[EnemyPos].Name + " Dmg delt: " + Damage);
    }
    /// <summary>
    /// Attacks party members using a skill/magic
    /// </summary>
    /// <param name="EnemyPos"></param>
    /// <param name="Target"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
    /// This function isn't used due to time constraints
    /*IEnumerator MagicalAttackOnParty(int EnemyPos, int Target, Enemy enemy)
    {
        List<SkillData.Skill> skillsAvaliable = new();
        for (int i = 0; i < enemy.skills.Count; i++)
        {
            if(enemy.skills[i].MpCost < enemy.CurrentMp && enemy.skills[i].HpCost < enemy.CurrentHp && enemy.skills[i].CostMode == SkillData.CostMode.Interger)
            {
                skillsAvaliable.Add(enemy.skills[i]);
            }
            if(enemy.skills[i].MpCost / 100 * enemy.Stats.Mp  < enemy.CurrentMp && enemy.skills[i].HpCost / 100 * enemy.Stats.Hp < enemy.CurrentHp && enemy.skills[i].CostMode == SkillData.CostMode.Percentage)
            {
                skillsAvaliable.Add(enemy.skills[i]);
            }
        }
        if(skillsAvaliable.Count != 0)
        {
            int SkillPicked = Random.Range(0, skillsAvaliable.Count);
            SkillData.Skill PickedSkill = skillsAvaliable[SkillPicked];
            for (int i = 0; i < PickedSkill.multihit; i++)
            {
                if (PickedSkill.range == ItemData.Range.Single)
                {
                    float Damage = 0f;
                    double RandomMultiplier = Random.Range(0.9f, 1.1f);
                    int CritRoll = Random.Range(0, 21);
                    Damage += (enemy.Stats.Atk * enemy.Stats.Atk) / (enemy.Stats.Atk + PData.characters[Target].Stats.Def) * (float)RandomMultiplier * ((float)PickedSkill.PhysicalPower / 100);
                    Damage += (enemy.Stats.MAtk * enemy.Stats.MAtk) / (enemy.Stats.MAtk + PData.characters[Target].Stats.MDef) * (float)RandomMultiplier * ((float)PickedSkill.MagicalPower / 100);
                    if (CritRoll == 20)
                    {
                        Damage = Damage * 1.5f;
                        Debug.Log("Crit!");
                    }
                    Damage = Mathf.Round(Damage);
                    enemies[Target].CurrentHp -= (int)Damage;
                    yield return StartCoroutine(HpUpdateAnimation(Target, "Damage", (int)Damage));
                    if (PData.characters[Target].CurrentHp < 0)
                    {
                        PData.characters[Target].CurrentHp = 0;
                    }
                    Debug.Log(PData.characters[Target].CurrentHp + "/" + PData.characters[Target].Stats.Hp + "\n" + enemies[EnemyPos].Name + " Dmg delt: " + Damage);
                }
                if (PickedSkill.range == ItemData.Range.Wide)
                {
                    for (int e = -1; e < 2; e++)
                    {
                        if (Target + e >= 0 && Target + e < PData.characters.Count)
                        {
                            float Damage = 0f;
                            double RandomMultiplier = Random.Range(0.9f, 1.1f);
                            int CritRoll = Random.Range(0, 21);
                            Damage += (enemy.Stats.Atk * enemy.Stats.Atk) / (enemy.Stats.Atk + PData.characters[Target].Stats.Def) * (float)RandomMultiplier * ((float)PickedSkill.PhysicalPower / 100);
                            Damage += (enemy.Stats.MAtk * enemy.Stats.MAtk) / (enemy.Stats.MAtk + PData.characters[Target].Stats.MDef) * (float)RandomMultiplier * ((float)PickedSkill.MagicalPower / 100);
                            if (CritRoll == 20)
                            {
                                Damage = Damage * 1.5f;
                                Debug.Log("Crit!");
                            }
                            Damage = Mathf.Round(Damage);
                            enemies[Target + e].CurrentHp -= (int)Damage;
                            yield return StartCoroutine(HpUpdateAnimation(Target + e, "Damage", (int)Damage));
                            if (PData.characters[Target + e].CurrentHp < 0)
                            {
                                PData.characters[Target + e].CurrentHp = 0;
                            }
                            Debug.Log(PData.characters[e].CurrentHp + "/" + PData.characters[e].Stats.Hp + "\n" + enemies[EnemyPos].Name + " Dmg delt: " + Damage);
                        }
                    }
                }
                if (PickedSkill.range == ItemData.Range.All)
                {
                    for (int e = 0; e < PData.characters.Count; e++)
                    {
                        float Damage = 0f;
                        double RandomMultiplier = Random.Range(0.9f, 1.1f);
                        int CritRoll = Random.Range(0, 21);
                        Damage += (enemy.Stats.Atk * enemy.Stats.Atk) / (enemy.Stats.Atk + PData.characters[e].Stats.Def) * (float)RandomMultiplier * ((float)PickedSkill.PhysicalPower / 100);
                        Damage += (enemy.Stats.MAtk * enemy.Stats.MAtk) / (enemy.Stats.MAtk + PData.characters[e].Stats.MDef) * (float)RandomMultiplier * ((float)PickedSkill.MagicalPower / 100);
                        if (CritRoll == 20)
                        {
                            Damage = Damage * 1.5f;
                            Debug.Log("Crit!");
                        }
                        Damage = Mathf.Round(Damage);
                        enemies[e].CurrentHp -= (int)Damage;
                        yield return StartCoroutine(HpUpdateAnimation(e, "Damage", (int)Damage));
                        if (PData.characters[e].CurrentHp < 0)
                        {
                            PData.characters[e].CurrentHp = 0;
                        }
                        Debug.Log(PData.characters[e].CurrentHp + "/" + PData.characters[e].Stats.Hp + "\n" + enemies[EnemyPos].Name + " Dmg delt: " + Damage);
                    }
                }
            }
        }
        else
        {
            EnemyTakesAction(EnemyPos);
        }

    }*/
    /// <summary>
    /// Scrolls to the next characters action
    /// </summary>
    public void NextCharacterAction()
    {
        ActionData = new();
        //Scrolls over to the next character's options box
        if (CurrentActionBeingPicked < 4)
        {
            StartCoroutine(ActionBoxShow(CurrentActionBeingPicked));
        }
        if(CurrentActionBeingPicked > 0)
        {
            StartCoroutine(ActionBoxHide(CurrentActionBeingPicked - 1));
        }
        //If the character is downed skip their turn
        for (int i = 0; i < 4; i++)
        {
            if (PData.characters[i].CurrentHp <= 0 && PData.characters[i].position == CurrentActionBeingPicked + 1)
            {
                FillCharacterInfo();
                CurrentActionBeingPicked++;
                NextCharacterAction();
            }
        }
    }
    /// <summary>
    /// Lerps the action box to be visable so the player can make a choice
    /// </summary>
    /// <param name="ActionBox"></param>
    /// <returns></returns>
    IEnumerator ActionBoxShow(int ActionBox)
    {
        //Plays the animation to show the targeted action box
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
    /// <summary>
    /// Lerps the action box to be hidden
    /// </summary>
    /// <param name="ActionBox"></param>
    /// <returns></returns>
    IEnumerator ActionBoxHide(int ActionBox)
    {
        //plays the animation to hide the targeted action box
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
    /// <summary>
    /// Toggles whether or not the info box should be seen by the player
    /// </summary>
    /// <param name="toggle"></param>
    /// <returns></returns>
    IEnumerator InfoBoxToggle(bool toggle)
    {
        //shows or hide the info box based on the toggle input
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
    /// <summary>
    /// Ends combat and checks whether the party fled, died or won the fight
    /// </summary>
    /// <param name="fledFromCombat"></param>
    public void EndCombat(bool fledFromCombat)
    {
        for (int i = 0; i < HpUpdate.Length; i++)
        {
            HpUpdate[i].GetComponent<TMPro.TextMeshProUGUI>().color = new Color32(255, 0, 0, 0);
        }
        if (fledFromCombat == false)
        {

        }
        for (int i = 0; i < HpUpdate.Length; i++)
        {
            HpUpdate[i].GetComponent<TMPro.TextMeshProUGUI>().color = new Color32(255, 0, 0, 0);
        }
        CombatScene.SetActive(false);
        StateManager.State = StateManager.GameState.Overworld;
    }
    /// <summary>
    /// Shows the target menu and changes info based 
    /// </summary>
    /// <param name="toggle"></param>
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
                            AttackMenu.transform.GetChild(i).GetComponent<Button>().interactable = false;
                            if(ActionData.action == Action.Pouch)
                            {
                                if(ActionData.Item.Range == ItemData.Range.Single)
                                {
                                    if (i == CurrentActionBeingPicked)
                                    {
                                        AttackMenu.transform.GetChild(i).GetComponent<Button>().interactable = true;
                                    }
                                }
                            }
                            if (ActionData.action == Action.Magic)
                            {
                                if (ActionData.Skill.range == ItemData.Range.Single)
                                {
                                    if (i == CurrentActionBeingPicked)
                                    {
                                        AttackMenu.transform.GetChild(i).GetComponent<Button>().interactable = true;
                                    }
                                }
                            }
                            if (ActionData.action == Action.Pouch && ActionData.Item.Range == ItemData.Range.Wide)
                            {
                                if (i == CurrentActionBeingPicked - 1 && CurrentActionBeingPicked -1 >= 0 && PData.characters[p].CurrentHp > 0)
                                {
                                    AttackMenu.transform.GetChild(i).GetComponent<Button>().interactable = true;
                                }
                                if (i == CurrentActionBeingPicked && PData.characters[p].CurrentHp > 0)
                                {
                                    AttackMenu.transform.GetChild(i).GetComponent<Button>().interactable = true;
                                }
                                if (i == CurrentActionBeingPicked + 1 && CurrentActionBeingPicked + 1 < 4 && PData.characters[p].CurrentHp > 0)
                                {
                                    AttackMenu.transform.GetChild(i).GetComponent<Button>().interactable = true;
                                }
                            }
                            if(ActionData.action == Action.Pouch && ActionData.Item.Range == ItemData.Range.All)
                            {
                                if(PData.characters[p].CurrentHp > 0)
                                {
                                    AttackMenu.transform.GetChild(i).GetComponent<Button>().interactable = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if(i < enemies.Count && enemies[i].CurrentHp > 0)
                    {
                        AttackMenu.transform.GetChild(i).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(enemies[i].Name);
                    }
                    else
                    {
                        AttackMenu.transform.GetChild(i).GetComponent<Button>().interactable = false;
                        AttackMenu.transform.GetChild(i).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("");
                    }
                }
            }
        }
    }
    /// <summary>
    /// Toggle the item menu to be hiden or shown
    /// </summary>
    /// <param name="toggle"></param>
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
    /// <summary>
    /// Sets up the pouch menu to show all of the items in that character's pouch
    /// </summary>
    /// <param name="ItemMenu"></param>
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
    /// <summary>
    /// Based on the button clicked on set up the character's action with the inputted data
    /// </summary>
    /// <param name="slot"></param>
    public void ItemPicked(int slot)
    {
        for (int i = 0; i < PData.characters.Count; i++)
        {
            if (PData.characters[i].position == CurrentActionBeingPicked + 1)
            {
                ActionData.action = Action.Pouch;
                ActionData.neededCharacters = new();
                ActionData.Item = PData.characters[i].Pouch[slot];
                ActionData.neededCharacters.Add(PData.characters[i].Name);
                ToggleTargetMenu(true);
                ToggleItemMenu(false);
            }
        }
    }
    /// <summary>
    /// When the target is picked it sets up the data needed for the action data and hides the target menu
    /// </summary>
    /// <param name="target"></param>
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
            ToggleTargetMenu(false);
            ConfirmAction("Attack");
        }
    }
    /// <summary>
    /// This method shows or hides the magic menu based on what the toggle is set to
    /// </summary>
    /// <param name="toggle"></param>
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
    /// <summary>
    /// Handles what to do when a skill is picked 
    /// </summary>
    /// <param name="skill"></param>
    public void SkillPicked(SkillData.Skill skill)
    {
        ActionData.Skill = skill;
        //Removes all skill button prefabs
        for (int i = 0; i < SkillPrefabList.Count; i++)
        {
            Destroy(SkillPrefabList[i]);
        }
        //Clear the prefab list
        SkillPrefabList.Clear();
        //Hide magic menu
        ToggleMagicMenu(false);
        //If the picked skills range is set to all there is no need to pick a target else make the player pick a target
        if(skill.range == ItemData.Range.All)
        {
            AddCharacterToNeeded();
            ActionData.action = Action.Magic;
            ConfirmAction("Magic");
        }
        else
        {
            ToggleTargetMenu(true);
        }
    }
    /// <summary>
    /// Just adds the character to the action data info to show which characters are doing that action
    /// </summary>
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
    /// <summary>
    /// Gets turn order based on the speed stats of all party members and enemies
    /// </summary>
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
    /// <summary>
    /// Fills in each characters info needed for combat
    /// </summary>
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
    /// <summary>
    /// Shows info the info box and sets the text to be based on what button is being hovered over
    /// </summary>
    /// <param name="button"></param>
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
        if(button == "Item1")
        {
            for (int i = 0; i < PData.characters.Count; i++)
            {
                if(PData.characters[i].position == CurrentActionBeingPicked + 1 && PData.characters[i].Pouch[0].ID != 0)
                {
                    string Info = "";
                    if(PData.characters[i].Pouch[0].Stats.Hp > 0)
                    {
                        Info += "Hp: " + PData.characters[i].Pouch[0].Stats.Hp + "\n";
                    }
                    if(PData.characters[i].Pouch[0].Stats.Mp > 0)
                    {
                        Info += "Mp: " + PData.characters[i].Pouch[0].Stats.Mp + "\n";
                    }
                    Info += "Range: " + PData.characters[i].Pouch[0].Range + "\n";
                    Info += "Element: " + PData.characters[i].Pouch[0].Element;
                    InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(Info);
                }
            }
        }
        if (button == "Item2")
        {
            for (int i = 0; i < PData.characters.Count; i++)
            {
                if (PData.characters[i].position == CurrentActionBeingPicked + 1 && PData.characters[i].Pouch[1].ID != 0)
                {
                    string Info = "";
                    if (PData.characters[i].Pouch[1].Stats.Hp > 0)
                    {
                        Info += "Hp: " + PData.characters[i].Pouch[1].Stats.Hp + "\n";
                    }
                    if (PData.characters[i].Pouch[1].Stats.Mp > 0)
                    {
                        Info += "Mp: " + PData.characters[i].Pouch[1].Stats.Mp + "\n";
                    }
                    Info += "Range: " + PData.characters[i].Pouch[1].Range + "\n";
                    Info += "Element: " + PData.characters[i].Pouch[1].Element;
                    InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(Info);
                }
            }
        }
        if (button == "Item3")
        {
            for (int i = 0; i < PData.characters.Count; i++)
            {
                if (PData.characters[i].position == CurrentActionBeingPicked + 1 && PData.characters[i].Pouch[2].ID != 0)
                {
                    string Info = "";
                    if (PData.characters[i].Pouch[2].Stats.Hp > 0)
                    {
                        Info += "Hp: " + PData.characters[i].Pouch[2].Stats.Hp + "\n";
                    }
                    if (PData.characters[i].Pouch[2].Stats.Mp > 0)
                    {
                        Info += "Mp: " + PData.characters[i].Pouch[2].Stats.Mp + "\n";
                    }
                    Info += "Range: " + PData.characters[i].Pouch[2].Range + "\n";
                    Info += "Element: " + PData.characters[i].Pouch[2].Element;
                    InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(Info);
                }
            }
        }
        if (button == "Item4")
        {
            for (int i = 0; i < PData.characters.Count; i++)
            {
                if (PData.characters[i].position == CurrentActionBeingPicked + 1 && PData.characters[i].Pouch[3].ID != 0)
                {
                    string Info = "";
                    if (PData.characters[i].Pouch[3].Stats.Hp > 0)
                    {
                        Info += "Hp: " + PData.characters[i].Pouch[3].Stats.Hp + "\n";
                    }
                    if (PData.characters[i].Pouch[3].Stats.Mp > 0)
                    {
                        Info += "Mp: " + PData.characters[i].Pouch[3].Stats.Mp + "\n";
                    }
                    Info += "Range: " + PData.characters[i].Pouch[3].Range + "\n";
                    Info += "Element: " + PData.characters[i].Pouch[3].Element;
                    InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(Info);
                }
            }
        }
        if (button == "Item5")
        {
            for (int i = 0; i < PData.characters.Count; i++)
            {
                if (PData.characters[i].position == CurrentActionBeingPicked + 1 && PData.characters[i].Pouch[4].ID != 0)
                {
                    string Info = "";
                    if (PData.characters[i].Pouch[4].Stats.Hp > 0)
                    {
                        Info += "Hp: " + PData.characters[i].Pouch[4].Stats.Hp + "\n";
                    }
                    if (PData.characters[i].Pouch[4].Stats.Mp > 0)
                    {
                        Info += "Mp: " + PData.characters[i].Pouch[4].Stats.Mp + "\n";
                    }
                    Info += "Range: " + PData.characters[i].Pouch[4].Range + "\n";
                    Info += "Element: " + PData.characters[i].Pouch[4].Element;
                    InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText(Info);
                }
            }
        }
    }
    /// <summary>
    /// Hides the info box and removes all text
    /// </summary>
    /// <param name="button"></param>
    public void ButtonLeave(string button)
    {
        if(InfoArea.transform.localPosition.y > -740 && button != "Item")
        {
            StartCoroutine(InfoBoxToggle(false));
        }
        InfoArea.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().SetText("");
    }
    /// <summary>
    /// Based on what character is passed to the method it returns that character's colour
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public Color32 GetPartyMemberColor(ItemData.Character character)
    {
        switch (character)
        {
            case ItemData.Character.Seth:
                return new Color32(0, 133, 79, 255);
            case ItemData.Character.Susie:
                return new Color32(0, 70, 191, 255);
            case ItemData.Character.Shiana:
                return new Color32(191, 19, 0, 255);
            case ItemData.Character.Brody:
                return new Color32(131, 0, 191, 255);
            default:
                return new Color32(145, 145, 145, 255);
        }
    }
    /// <summary>
    /// Based on what character is passed to the method it returns that character's downed colour
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public Color32 GetDownPartyMemberColor(ItemData.Character character)
    {
        switch (character)
        {
            case ItemData.Character.Seth:
                return new Color32(0, 92, 55, 255);
            case ItemData.Character.Susie:
                return new Color32(0, 39, 107, 255);
            case ItemData.Character.Shiana:
                return new Color32(117, 12, 0, 255);
            case ItemData.Character.Brody:
                return new Color32(72, 1, 105, 255);
            default:
                return new Color32(77, 77, 77, 255);
        }
    }
    /// <summary>
    /// This is just the hp update animation that is red when taking damage and green for when gaining or healing any stats like mp
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="Type"></param>
    /// <param name="Hp"></param>
    /// <returns></returns>
    IEnumerator HpUpdateAnimation(int Target, string Type, int Hp)
    {
        float timeElapsed = 0;
        float lerpDuration = 1f;
        float transparency;
        Color32 colour;
        HpUpdate[Target].GetComponent<TMPro.TextMeshProUGUI>().SetText(Hp + "");
        if (Type == "Damage")
        {
            if(Hp == 0)
            {
                HpUpdate[Target].GetComponent<TMPro.TextMeshProUGUI>().SetText("Miss");
            }
            colour = new Color32(255, 0, 0, 255);
            HpUpdate[Target].GetComponent<TMPro.TextMeshProUGUI>().color = colour;
        }
        else
        {
            colour = new Color32(0, 255, 0, 255);
            HpUpdate[Target].GetComponent<TMPro.TextMeshProUGUI>().color = colour;
        }
        while (timeElapsed < lerpDuration)
        { 
            transparency = Mathf.Lerp(255, 0, timeElapsed / lerpDuration);
            colour = new Color32(colour.r, colour.g, colour.b, (byte)transparency);
            HpUpdate[Target].GetComponent<TMPro.TextMeshProUGUI>().color = colour;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        colour = new Color32(colour.r, colour.g, colour.b, 0);
        HpUpdate[Target].GetComponent<TMPro.TextMeshProUGUI>().color = colour;
        yield break;
    }
    /// <summary>
    /// Runs through every character and checks who is downed
    /// If everyone is downed play game over screen
    /// </summary>
    public void CheckWhoIsDowned()
    {
        CharactersDowned = 0;
        for (int i = 0; i < PData.characters.Count; i++)
        {
            if(PData.characters[i].CurrentHp <= 0)
            {
                CharactersDowned++;
            }
        }

        if(CharactersDowned == 4)
        {
            StopAllCoroutines();
            GameOver();
        }
    }
    /// <summary>
    /// Starts game over animation and clears any hp update animations playing
    /// </summary>
    public void GameOver()
    {
        for (int i = 0; i < HpUpdate.Length; i++)
        {
            HpUpdate[i].GetComponent<TMPro.TextMeshProUGUI>().color = new Color32(255, 0, 0, 0);
        }
        StartCoroutine(GameOverAnimation());
        EndCombat(false);
    }
    /// <summary>
    /// Fades in the game over screen
    /// </summary>
    /// <returns></returns>
    IEnumerator GameOverAnimation()
    {
        float timeElapsed = 0;
        float lerpDuration = 2.5f;
        float transparency;
        Color32 colour;
        colour = new Color32(255, 255, 255, 255);
        GameOverScreen.SetActive(true);
        Image image = GameOverScreen.GetComponent<Image>();
        while (timeElapsed < lerpDuration)
        {
            transparency = Mathf.Lerp(0, 255, timeElapsed / lerpDuration);
            colour = new Color32(colour.r, colour.g, colour.b, (byte)transparency);
            image.color = colour;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        colour = new Color32(colour.r, colour.g, colour.b, 255);
        image.color = colour;
        yield break;
    }
}