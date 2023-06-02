using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public float speed = 7.00f;
    public float gravityMultipler = 2f;
    private float gravity = -9.81f;
    private Vector3 playerMovement;
    private float yVelocity = 0;
    public float jumpPower = 0.3f;
    CharacterController Cr;
    public bool isSliding;
    private Vector3 slideMovement;
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

        UpdateSlideMovement();

        if (slideMovement == Vector3.zero)
        {
            isSliding = false;
        }
        else
        {
            isSliding = true;
        }


        if (grounded)
        {
            yVelocity = -0.05f;
        }
        else
        {
            yVelocity += gravity * Time.deltaTime * Time.deltaTime * gravityMultipler;
        }
        playerMovement.y = yVelocity;

        if (isSliding == true)
        {
            Vector3 sliding = slideMovement;
            sliding.y = yVelocity;
            Cr.Move(sliding);
        }
        else
        {
            Jump(grounded);
            Cr.Move(playerMovement);
        }
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

    void UpdateSlideMovement()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 0.5f))
        {
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);

            if(angle >= Cr.slopeLimit)
            {
                slideMovement = Vector3.ProjectOnPlane(new Vector3(0,yVelocity,0), hitInfo.normal);
                return;
            }
        }

        slideMovement = Vector3.zero;
    }
}
