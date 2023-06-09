using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControler : MonoBehaviour
{
    public float speed = 7.00f;
    public float gravityMultipler = 4f;
    private float gravity = -9.81f;
    private Vector3 playerMovement;
    private float yVelocity = 0;
    public float jumpPower = 0.15f;
    CharacterController Cr;
    // Start is called before the first frame update
    void Start()
    {       
        Cr = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
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

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "RoomTransition")
        {
            string Room;
            string Direction;
            Room = collider.gameObject.GetComponent<RoomTransitionLogic>().GoToRoom.ToString();
            Direction = collider.gameObject.GetComponent<RoomTransitionLogic>().FaceDirection.ToString();            
        }
    }
}
