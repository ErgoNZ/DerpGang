using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Diagnostics;
using UnityEngine.AI;

public class PlayerControler : MonoBehaviour
{
    public float speed = 7.00f;
    public float gravityMultipler = 4f;
    private float gravity = -9.81f;
    private Vector3 playerMovement;
    private float yVelocity = 0;
    public float jumpPower = 0.24f;
    public string room;
    public string direction;
    CharacterController Cr;
    CombatLogic CombatLogic;
    GameObject lastEnemyHit;
    // Start is called before the first frame update
    void Start()
    {       
        Cr = GetComponent<CharacterController>();
        CombatLogic = GameObject.Find("GameManager").GetComponent<CombatLogic>();
    }

    //Fixed update runs on a strict 60fps system making physics consistant on any computer.
    void FixedUpdate()
    {
        bool grounded = GroundCheck();
        float xMove = Input.GetAxisRaw("Horizontal"); // d key changes value to 1, a key changes value to -1
        float zMove = Input.GetAxisRaw("Vertical"); // w key changes value to 1, s key changes value to -1
        playerMovement.x = xMove * speed * Time.deltaTime;
        playerMovement.z = zMove * speed * Time.deltaTime;

        //If grounded lock the players yVelocity to make movement smoother.
        if (grounded)
        {
            yVelocity = -0.05f;
        }
        else
        {
            //Adds to the yVelocity while falling 
            yVelocity += gravity * Time.deltaTime * Time.deltaTime * gravityMultipler;
        }
        playerMovement.y = yVelocity;

        //Checks if the player wants to jump and then moves the character.
        Jump(grounded);
        Cr.Move(playerMovement);
    }
    /// <summary>
    /// Checks if the player is on the ground.
    /// </summary>
    /// <returns></returns>
    bool GroundCheck(){
        return Cr.isGrounded;
    }
    /// <summary>
    /// Checks if the player wants to jump and can jump.
    /// </summary>
    /// <param name="grounded"></param>
    void Jump(bool grounded){
        if(Input.GetKey(KeyCode.Space) && grounded){
            yVelocity = 1f * jumpPower;
            playerMovement.y = yVelocity * Time.deltaTime;
        }
    }
    /// <summary>
    /// If the player runs into a trigger.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        //If the trigger is a room transition trigger get the script from it and change scenes to where the room transition is sending the player
        if (other.tag == "RoomTransition")
        {
            RoomTransitionLogic roomTransitionLogic;
            roomTransitionLogic = other.GetComponent<RoomTransitionLogic>();
            room = roomTransitionLogic.GoToRoom.ToString();
            direction = roomTransitionLogic.FaceDirection.ToString();
            playerMovement = new Vector3(0, 0, 0);
            SceneManager.LoadScene(room);
            transform.localPosition = roomTransitionLogic.SpawnPosition;
            print(transform.localPosition);
        }
        //If the trigger is an enemy disable the enemy and start combat
        if(other.tag == "Enemy")
        {
            other.gameObject.SetActive(false);
            other.GetComponent<Rigidbody>().useGravity = false;
            other.GetComponent<BoxCollider>().enabled = false;
            other.GetComponent<NavMeshAgent>().enabled = false;
            lastEnemyHit = other.gameObject;
            CombatLogic.enemies = other.gameObject.GetComponent<EnemyData>().enemies;
            CombatLogic.StartCombat();
        }
    }
    /// <summary>
    /// If the player stays inside of the trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        //If the trigger is an Npc trigger constantly sets that the player is in range of the Npc to true
        if (other.tag == "NPC")
        {
            other.GetComponent<NpcLogic>().toggleInRange(true);
        }
    }
    /// <summary>
    /// If the player leaves a trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        //If the player leaves the Npc's range sets the npc to act like the player isn't in range of them. (Player just can't talk with them outside of their range)
        if (other.tag == "NPC")
        {
            other.GetComponent<NpcLogic>().toggleInRange(false);
        }
    }
    /// <summary>
    /// This is called when the player flees from combat
    /// </summary>
    /// <returns></returns>
    public IEnumerator FledFromCombat()
    {
        //This activates the enemy that started combat with the player and lets them run away before going after them again.
        lastEnemyHit.GetComponent<Rigidbody>().useGravity = true;
        lastEnemyHit.SetActive(true);
        yield return new WaitForSeconds(3);
        lastEnemyHit.GetComponent<BoxCollider>().enabled = true;
        lastEnemyHit.GetComponent<NavMeshAgent>().enabled = true;
        yield break;
    }
}
