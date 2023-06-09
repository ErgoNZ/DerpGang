using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject PlayerPrefab;
    GameObject Player;

    private void Awake()
    {
        Instance = this;
        Player = Instantiate(PlayerPrefab);
    }

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Player, new Vector3(0, 1, 0), Quaternion.identity);
        Debug.Log("this executed!");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("this executed!");
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
