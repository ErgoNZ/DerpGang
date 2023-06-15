using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject PlayerPrefab;
    static GameObject Player;
    public struct Item
    {
        int ID;
        string Name;
        Catagory Type;
        int Amount;
        string Description;
        DamageType Element;
        int Hp;
        int Mp;
        List<EffectData> Effects;
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
        Cutscene
    }
}
