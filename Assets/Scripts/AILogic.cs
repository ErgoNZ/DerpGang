using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILogic : MonoBehaviour
{
    //
    //
    //I didn't think of any of this code that is in this file! Credit goes to Dave/GameDevelopment from Youtube. Video:https://www.youtube.com/watch?v=UjkSFoLxesw
    //
    //
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;
    StateManager StateManager;

    //EnemyPatrol
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //states
    public float sightRange = 0.01f;
    public bool playerInSight;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        player = GameObject.Find("Player(Clone)").transform;
        StateManager = GameObject.Find("GameManager").GetComponent<StateManager>();
    }


    private void Update()
    {
        //checks if player is in sight range
        playerInSight = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        agent.transform.rotation = new(0,0,0,0);

        if (!playerInSight || StateManager.State == StateManager.GameState.Combat)
        {
            Patroling();
        }
        if (playerInSight && StateManager.State != StateManager.GameState.Combat)
        {
            Chasing();
        }
    }
    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 dsitanceToWalkPoint = transform.position - walkPoint;

        if (dsitanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    private void SearchWalkPoint()
    {
        float RandomZ = Random.Range(-walkPointRange, walkPointRange);
        float RandomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + RandomX, transform.position.y, transform.position.z + RandomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void Chasing()
    {
        agent.SetDestination(player.position);
    }
}

