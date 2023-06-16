using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject PlayerPrefab;
    static GameObject Player;
    public List<Item> ItemList;
    public GameState State;
    public struct Item
    {
        int ID;
        string Name;
        Catagory Type;
        int Amount;
        string Description;
        DamageType Element;
        Stats Stats;
        List<EffectData> Effects;
        List<CharmData> CharmEffects;
        List<DamageType> Resistance;
        List<DamageType> Vulnerable;
    }
    public enum Catagory
    {
        Consumable,
        Weapon,
        Armour,
        Charm
    }
    public struct EffectData
    {
        EffectNames Name;
        DamageType Element;
        int Duration;
        int Damage;
        int MpDamage;
        string Source;
    }

    public enum EffectNames
    {
        Burn,
        Flamable,
        Soaked
    }

    public enum DamageType
    {
        Water,
        Fire,
        Ice,
        Poison,
        True,
        Earth,
        Restore,
        Electric
    }

    public struct Stats
    {
        int Hp;
        int Mp;
        int Atk;
        int Def;
        int MAtk;
        int MDef;
        int Spd;
    }

    public struct CharmData
    {
        string Name;
        string CheckingFor;
        Trigger Trigger;
        EffectData EffectData;
    }

    public enum Trigger
    {
        OnHit,
        RoundStart,
        HpCheck,
        EnemyCheck,
        StatusCheck,
        CombatStart
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Player = Instantiate(PlayerPrefab);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Player);
        State = GameState.LoadingItems;
        LoadItems();
    }

    private void LoadItems()
    {
        
    }

    public enum GameState{
        Overworld,
        CombatStart,
        CharacterTurn1,
        CharacterTurn2,
        CharacterTurn3,
        CharacterTurn4,
        EnemyTurn,
        Defeat,
        Victory,
        Cutscene,
        LoadingItems
    }
}
