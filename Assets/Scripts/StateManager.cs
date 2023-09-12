using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Diagnostics;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance;
    public GameObject PlayerPrefab;
    static GameObject Player;
    public GameObject Menu;
    public GameObject EventManager;
    public GameState State = GameState.Overworld;
    bool MenuOpen = false;
    public bool InCombat = false;
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
        DontDestroyOnLoad(EventManager);
        State = GameState.LoadingItems;
        PlayerControler = Player.GetComponent<PlayerControler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && State != GameState.Combat && State != GameState.Talking)
        {
            MenuOpen = !MenuOpen;
            State = GameState.Overworld;
            Menu.SetActive(MenuOpen);
            if (MenuOpen == true)
            {
                State = GameState.Menu;
            }
        }
        PlayerControler.enabled = true;

        if (State == GameState.Menu || State == GameState.Combat || State == GameState.Talking)
        {
            PlayerControler.enabled = false;
        }
    }

    

    public enum GameState{
        Overworld,
        Combat,
        Defeat,
        Victory,
        Cutscene,
        LoadingItems,
        Menu,
        Talking
    }
}
