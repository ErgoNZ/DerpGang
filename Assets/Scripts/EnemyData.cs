using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    /// <summary>
    /// Just setup for data needed.
    /// Anything public except the enemy list is set while in the unity editor.
    /// </summary>
    SkillData SkillData;
    public List<CombatLogic.Enemy> enemies;
    public int numEnemies;
    [Header("Enemy1")]
    public string Name1;
    public int hp1;
    public int mp1;
    public int atk1;
    public int def1;
    public int mAtk1;
    public int mDef1;
    public int spd1;
    public List<int> skills1;
    [Header("Enemy2")]
    public string Name2;
    public int hp2;
    public int mp2;
    public int atk2;
    public int def2;
    public int mAtk2;
    public int mDef2;
    public int spd2;
    public List<int> skills2;
    [Header("Enemy3")]
    public string Name3;
    public int hp3;
    public int mp3;
    public int atk3;
    public int def3;
    public int mAtk3;
    public int mDef3;
    public int spd3;
    public List<int> skills3;
    [Header("Enemy4")]
    public string Name4;
    public int hp4;
    public int mp4;
    public int atk4;
    public int def4;
    public int mAtk4;
    public int mDef4;
    public int spd4;
    public List<int> skills4;
    [Header("EncounterVaraibles")]
    public int goldEarned;
    public int xpEarned;
    public CombatLogic.Enemy enemy1, enemy2, enemy3, enemy4;
    // Start is called before the first frame update
    void Start()
    {
        SkillData = GetComponent<SkillData>();
        enemies = new();
        CreateEnemies();
    }
    /// <summary>
    /// Creates the enemies based on the data inputted into this script from the unity editor
    /// </summary>
    public void CreateEnemies()
    {
        string[] Name = { Name1, Name2, Name3, Name4 }; 
        int[] hp = { hp1, hp2, hp3, hp4 };
        int[] mp = { mp1, mp2, mp3, mp4 };
        int[] atk = { atk1, atk2, atk3, atk4 };
        int[] def = { def1, def2, def3, def4 };
        int[] mAtk = { mAtk1, mAtk2, mAtk3, mAtk4 };
        int[] mDef = { mDef1, mDef2, mDef3, mDef4 };
        int[] spd = { spd1, spd2, spd3, spd4 };
        List<int>[] skills = { skills1, skills2, skills3, skills4 };
        //based on the amount of enemies it creates an enemy based on the data given and adds it to the list.
        for (int i = 0; i < numEnemies; i++)
        {
            if(hp[i] > 0)
            {
                CombatLogic.Enemy enemy = new();
                enemy.Name = Name[i];
                enemy.CurrentHp = hp[i];
                enemy.Stats.Hp = hp[i];
                enemy.Stats.Mp = mp[i];
                enemy.CurrentMp = mp[i];
                enemy.Stats.Atk = atk[i];
                enemy.Stats.Def = def[i];
                enemy.Stats.MAtk = mAtk[i];
                enemy.Stats.MDef = mDef[i];
                enemy.Stats.Spd = spd[i];
                enemy.position = 5 + i;
                List<int> skillList = skills[i];
                for (int s = 0; s < skillList.Count; s++)
                {
                    enemies[i].skills.Add(SkillData.GetSkill(skillList[s]));
                }
                enemies.Add(enemy);
            }
        }
    }
}
