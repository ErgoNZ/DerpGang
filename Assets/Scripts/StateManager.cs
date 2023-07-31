using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Diagnostics;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject PlayerPrefab;
    static GameObject Player;
    public GameObject Menu;
    public GameObject EventManager;
    public GameState State;
    bool MenuOpen = false;
    PlayerControler PlayerControler;

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
        DontDestroyOnLoad(Menu);
        DontDestroyOnLoad(EventManager);
        State = GameState.LoadingItems;
        PlayerControler = Player.GetComponent<PlayerControler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuOpen = !MenuOpen;
            State = GameState.Overworld;
            Menu.SetActive(MenuOpen);
            if(MenuOpen == true)
            {
                State = GameState.Menu;
            }
        }
        PlayerControler.enabled = true;

        if (State == GameState.Menu)
        {
            PlayerControler.enabled = false;
        }
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
        LoadingItems,
        Menu
    }
}
