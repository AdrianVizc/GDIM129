using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private enum EnemyState
    {
        idle,
        approach,
        lurk,
        retreat
    };
    private EnemyState currentState;

    [SerializeField] private NavMeshAgent agent;
    private GameObject player;

    //Bounds
    [SerializeField] private float minApproachDistance; //also the fog view distance
    [SerializeField] private float maxApproachDistance;
    [SerializeField] private float bufferDistance;
    private float retreatDistance = 15f;
    private float distanceToPlayer;

    //Timers
    private float stateTimer;
    [SerializeField] private float idleDuration = 15f;
    [SerializeField] private float lurkDuration = 3f;

    //[SerializeField] private Material originalMaterial;
    //private Color currentColor;
    //[SerializeField] private float disappearRate;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentState = EnemyState.idle;
        stateTimer = idleDuration;
        //currentColor = originalMaterial.color;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeStates();
        Debug.Log(currentState);
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
    }

    private void ChangeStates()
    {
        // If there's a timer active, count it down
        if (stateTimer >= 0)
            stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            // Idle Behavior
            case EnemyState.idle:
                // If still idle AND distance > minApproachDistance (meaning the enemy has room to move closer) --> Approach
                if (stateTimer <= 0 && distanceToPlayer > minApproachDistance)
                {
                    currentState = EnemyState.approach;
                }
                else if (distanceToPlayer < minApproachDistance) // no room to move closer OR is closer than we want --> Retreat
                {
                    currentState = EnemyState.retreat;
                }
                break;

            // Approach Behavior
            case EnemyState.approach:
                // If there is room to move closer
                if (distanceToPlayer > minApproachDistance)
                {
                    // Get direction from enemy to player, normalize it so it is unit length (1)
                    Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
                    // Then the position the enemy needs to go is: based off the player's position, take direction and give it a magnitude
                    Vector3 targetPos = player.transform.position + dirToPlayer * (Random.Range(minApproachDistance, maxApproachDistance) + bufferDistance);
                    agent.SetDestination(targetPos);
                }
                else
                {
                    agent.ResetPath();
                    currentState = EnemyState.lurk;
                    stateTimer = lurkDuration;
                }
                break;

            case EnemyState.lurk:
                if (stateTimer <= 0f)
                {
                    currentState = EnemyState.retreat;
                }
                break;

            case EnemyState.retreat:
                Vector3 dirAwayFromPlayer = -(player.transform.position - transform.position).normalized;
                Vector3 retreatPos = player.transform.position + dirAwayFromPlayer * retreatDistance;
                agent.SetDestination(retreatPos);
                //currentColor.a = Mathf.Clamp01(currentColor.a - disappearRate * Time.deltaTime);
                //originalMaterial.color = currentColor;

                if (Vector3.Distance(transform.position, player.transform.position) > retreatDistance)
                {
                    currentState = EnemyState.idle;
                    stateTimer = idleDuration;
                }
                break;
        }

    }

    private bool DoesPlayerSeeMe()
    {
        if (player == null)
            return false;

        //Get Vector from player to enemy
        Vector3 playerToEnemy = (this.transform.position - player.transform.position).normalized;

        float distanceSquared = playerToEnemy.x * playerToEnemy.x + playerToEnemy.y * playerToEnemy.y + playerToEnemy.z * playerToEnemy.z;

        //Out of player view
        if (distanceSquared > minApproachDistance * minApproachDistance)
            return false;

        //Get player forward vector
        Vector3 playerForward = player.transform.forward;

        //Dot the two vectors
        float dotProduct = Vector3.Dot(playerForward, playerToEnemy);

        //This will tell us if the player is looking at the enemy, we need 0.8 for a more precise view angle
        return dotProduct > 0.8f;
    }
}
