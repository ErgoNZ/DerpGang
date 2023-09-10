using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    SkillData SkillData;
    CombatLogic CombatLogic;
    public List<CombatLogic.Enemy> enemies;
    public int numEnemies;
    [Header("Enemy1")]
    public string Name1;
    public int Hp1;
    public int Mp1;
    public int Atk1;
    public int Def1;
    public int MAtk1;
    public int MDef1;
    public int Spd1;
    public List<int> Skills1;
    [Header("Enemy2")]
    public string Name2;
    public int Hp2;
    public int Mp2;
    public int Atk2;
    public int Def2;
    public int MAtk2;
    public int MDef2;
    public int Spd2;
    public List<int> Skills2;
    [Header("Enemy3")]
    public string Name3;
    public int Hp3;
    public int Mp3;
    public int Atk3;
    public int Def3;
    public int MAtk3;
    public int MDef3;
    public int Spd3;
    public List<int> Skills3;
    [Header("Enemy4")]
    public string Name4;
    public int Hp4;
    public int Mp4;
    public int Atk4;
    public int Def4;
    public int MAtk4;
    public int MDef4;
    public int Spd4;
    public List<int> Skills4;
    [Header("EncounterVaraibles")]
    public int goldEarned;
    public int XpEarned;
    public CombatLogic.Enemy enemy1, enemy2, enemy3, enemy4;
    // Start is called before the first frame update
    void Start()
    {
        SkillData = GetComponent<SkillData>();
        CombatLogic = GetComponent<CombatLogic>();
        enemies = new();
        CreateEnemies();
    }
    public void CreateEnemies()
    {
        string[] Name = { Name1, Name2, Name3, Name4 }; 
        int[] Hp = { Hp1, Hp2, Hp3, Hp4 };
        int[] Mp = { Mp1, Mp2, Mp3, Mp4 };
        int[] Atk = { Atk1, Atk2, Atk3, Atk4 };
        int[] Def = { Def1, Def2, Def3, Def4 };
        int[] MAtk = { MAtk1, MAtk2, MAtk3, MAtk4 };
        int[] MDef = { MDef1, MDef2, MDef3, MDef4 };
        int[] Spd = { Spd1, Spd2, Spd3, Spd4 };
        List<int>[] Skills = { Skills1, Skills2, Skills3, Skills4 };
        for (int i = 0; i < numEnemies; i++)
        {
            if(Hp[i] > 0)
            {
                CombatLogic.Enemy enemy = new();
                enemy.Name = Name[i];
                enemy.CurrentHp = Hp[i];
                enemy.Stats.Hp = Hp[i];
                enemy.Stats.Mp = Mp[i];
                enemy.CurrentMp = Mp[i];
                enemy.Stats.Atk = Atk[i];
                enemy.Stats.Def = Def[i];
                enemy.Stats.MAtk = MAtk[i];
                enemy.Stats.MDef = MDef[i];
                enemy.Stats.Spd = Spd[i];
                enemy.position = 5 + i;
                List<int> skillList = Skills[i];
                for (int s = 0; s < skillList.Count; s++)
                {
                    SkillData.GetSkill(skillList[s]);
                }
                enemies.Add(enemy);
            }
        }
    }
}
