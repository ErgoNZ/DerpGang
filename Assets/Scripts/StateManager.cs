using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance;
    public GameObject PlayerPrefab;
    static GameObject Player;
    public GameObject Menu;
    public GameObject EventManager;
    public GameObject OverWorldUI;
    public GameState State = GameState.Overworld;
    bool menuOpen = false;
    public bool inCombat = false;
    PlayerControler PlayerControler;
    private void Awake()
    {
        //This just makes sure that only one of this script can ever be active at a time
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        //Setting things up so required data persists between scenes
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
        //Opens the inventory menu
        if (Input.GetKeyDown(KeyCode.Escape) && State != GameState.Combat && State != GameState.Talking && State != GameState.MainMenu)
        {
            menuOpen = !menuOpen;
            State = GameState.Overworld;
            Menu.SetActive(menuOpen);
            OverWorldUI.SetActive(menuOpen);
            if (menuOpen == true)
            {
                State = GameState.Menu;
            }
        }
        PlayerControler.enabled = true;

        //Stops the player from moving when they shouldn't be able to
        if (State == GameState.Menu || State == GameState.Combat || State == GameState.Talking || State == GameState.MainMenu)
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
        Talking,
        MainMenu
    }
    /// <summary>
    /// Does what the function is called
    /// </summary>
    public void ResetPlayerPos()
    {
        Player.transform.position = new(-2, 2, 0);
    }
}
