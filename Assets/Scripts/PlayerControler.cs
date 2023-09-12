using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Diagnostics;

public class PlayerControler : MonoBehaviour
{
    public float speed = 7.00f;
    public float gravityMultipler = 4f;
    private float gravity = -9.81f;
    private Vector3 playerMovement;
    private float yVelocity = 0;
    public float jumpPower = 0.24f;
    public string Room;
    public string Direction;
    CharacterController Cr;
    CombatLogic CombatLogic;
    // Start is called before the first frame update
    void Start()
    {       
        Cr = GetComponent<CharacterController>();
        CombatLogic = GameObject.Find("GameManager").GetComponent<CombatLogic>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool grounded = GroundCheck();
        float xMove = Input.GetAxisRaw("Horizontal"); // d key changes value to 1, a key changes value to -1
        float zMove = Input.GetAxisRaw("Vertical"); // w key changes value to 1, s key changes value to -1
        playerMovement.x = xMove * speed * Time.deltaTime;
        playerMovement.z = zMove * speed * Time.deltaTime;


        if (grounded)
        {
            yVelocity = -0.05f;
        }
        else
        {
            yVelocity += gravity * Time.deltaTime * Time.deltaTime * gravityMultipler;
        }
        playerMovement.y = yVelocity;

        Jump(grounded);
        Cr.Move(playerMovement);
    }

    bool GroundCheck(){
        return Cr.isGrounded;
    }

    void Jump(bool grounded){
        if(Input.GetKey(KeyCode.Space) && grounded){
            yVelocity = 1f * jumpPower;
            playerMovement.y = yVelocity * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided!");
        if (other.tag == "RoomTransition")
        {
            RoomTransitionLogic roomTransitionLogic;
            roomTransitionLogic = other.GetComponent<RoomTransitionLogic>();
            Room = roomTransitionLogic.GoToRoom.ToString();
            Direction = roomTransitionLogic.FaceDirection.ToString();
            playerMovement = new Vector3(0, 0, 0);
            SceneManager.LoadScene(Room);
            transform.localPosition = roomTransitionLogic.SpawnPosition;
            print(transform.localPosition);
        }
        if(other.tag == "Enemy")
        {
            other.GetComponent<Rigidbody>().useGravity = false;
            other.GetComponent<BoxCollider>().enabled = false;
            CombatLogic.enemies = other.gameObject.GetComponent<EnemyData>().enemies;
            CombatLogic.StartCombat();
        }
        if(other.tag == "NPC")
        {
            other.GetComponent<NpcLogic>().toggleInRange(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "NPC")
        {
            other.GetComponent<NpcLogic>().toggleInRange(false);
        }
    }
}
