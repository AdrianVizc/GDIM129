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
        retreat,
        disappear
    };
    private EnemyState currentState;

    [SerializeField] private NavMeshAgent agent;
    private GameObject player;

    //Bounds
    [SerializeField] private float fogViewDistance;
    [SerializeField] private float maxApproachDistance;

    //Timers
    private float stateTimer;
    private float idleDuration = 15f;
    private float lurkDuration = 5f;
    private float disappearDuration = 3f;

    private bool hasDestination;

    //Mesh
    private float disappearSpeed = 2f;
    //private Color originalColor;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentState = EnemyState.idle;
        stateTimer = idleDuration;
    }

    // Update is called once per frame
    void Update()
    {
        //ChangeStates();
        //HandleStates();
        //Debug.Log(currentState);
        if (!hasDestination)
        {
            Vector2 pointAroundPlayer = Random.insideUnitCircle.normalized * maxApproachDistance;

            Vector3 possiblePoint = player.transform.position + new Vector3(pointAroundPlayer.x, player.transform.position.y, pointAroundPlayer.y);

            Vector3 playerToPoint = possiblePoint - player.transform.position;

            if (playerToPoint.magnitude > fogViewDistance)
            {
                possiblePoint = player.transform.position + playerToPoint.normalized * maxApproachDistance;
                possiblePoint.y = player.transform.position.y;
            }

            agent.SetDestination(possiblePoint);
            hasDestination = true;
        }

    }

    private void ChangeStates()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.idle:
                if (DoesPlayerSeeMe())
                {
                    currentState = EnemyState.retreat;
                }
                if (stateTimer <= 0f)
                {
                    currentState = EnemyState.approach;
                    stateTimer = 0f;
                    hasDestination = false;
                }

                break;

            case EnemyState.approach:
                if (DoesPlayerSeeMe())
                {
                    currentState = EnemyState.retreat;
                    break;
                }
                if (agent.remainingDistance <= maxApproachDistance)
                {
                    currentState = EnemyState.lurk;
                    stateTimer = lurkDuration;
                }
                break;

            case EnemyState.lurk:
                if (stateTimer <= 0f)
                {
                    currentState = EnemyState.retreat;
                    stateTimer = 0f;
                }
                break;

            case EnemyState.retreat:
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = EnemyState.disappear;
                    stateTimer = disappearDuration;
                    hasDestination = false;
                }
                break;

            case EnemyState.disappear:
                if (stateTimer <= 0f)
                {
                    currentState = EnemyState.idle;
                    stateTimer = idleDuration;
                    hasDestination = false;
                }
                break;
        }

    }

    private void HandleStates()
    {
        switch(currentState)
        {
            case EnemyState.idle:
                IdleLogic();
                break;
            case EnemyState.approach:
                ApproachLogic();
                break;
            case EnemyState.lurk:
                LurkLogic();
                break;
            case EnemyState.retreat:
                RetreatLogic();
                break;
            case EnemyState.disappear:
                DisappearLogic();
                break;
        }
    }

    private void IdleLogic()
    {
        if (!hasDestination)
        {
            agent.SetDestination(GetRandomPointInFog());
            hasDestination = true;
            Debug.Log("IDLE");
        }
    }

    private void ApproachLogic()
    {
        if (!hasDestination)
        {
            Vector3 direction = (transform.position - player.transform.position).normalized;
            Vector3 target = player.transform.position + direction * (maxApproachDistance);
            agent.SetDestination(GetValidNavmeshPoint(target));
            hasDestination = true;  
        }
    }

    private void LurkLogic()
    {
        agent.ResetPath();
        hasDestination = false;
    }

    private void RetreatLogic()
    {
        agent.ResetPath();
        hasDestination = false;

        Vector3 directionAway = (transform.position - player.transform.position).normalized;
        transform.position += directionAway * disappearSpeed * Time.deltaTime;
    }

    private void DisappearLogic()
    {
        //agent.ResetPath();
        //hasDestination = false;

        //Vector3 directionAway = (transform.position - player.transform.position).normalized;
        //transform.position += directionAway * disappearSpeed * Time.deltaTime;
    }

    private Vector3 GetValidNavmeshPoint(Vector3 target)
    {
        //Checking if the Vector3 target is a valid position for the NavMesh
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            return hit.position;
        return transform.position;
    }

    private Vector3 GetRandomPointInFog()
    {
        const int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            //Create a circle around player to consider fog
            Vector2 pointAroundPlayer = Random.insideUnitCircle.normalized * maxApproachDistance;

            //Pick a point with respect to where the player is. Also y --> z from 2D to 3D space
            Vector3 possiblePoint = player.transform.position + new Vector3(pointAroundPlayer.x, player.transform.position.y, pointAroundPlayer.y);
            
            //Check where this point is with respect to the player. This is the vector from player to point.
            Vector3 playerToPoint = possiblePoint - player.transform.position;
            
            //If this point is outside the player's fogViewDistance
            if (playerToPoint.magnitude > fogViewDistance)
            {
                //Then we can set it as our location to go
                possiblePoint = player.transform.position + playerToPoint.normalized * maxApproachDistance;
                possiblePoint.y = player.transform.position.y;
            }
            else
            {
                possiblePoint = transform.position;
            }

            if (GetValidNavmeshPoint(possiblePoint) != transform.position)
                return possiblePoint;
        }

        return GetValidNavmeshPoint(transform.position + Random.insideUnitSphere * 2f);
    }

    private bool DoesPlayerSeeMe()
    {
        if (player == null)
            return false;

        //Get Vector from player to enemy
        Vector3 playerToEnemy = (this.transform.position - player.transform.position).normalized;

        float distanceSquared = playerToEnemy.x * playerToEnemy.x + playerToEnemy.y * playerToEnemy.y + playerToEnemy.z * playerToEnemy.z;

        //Out of player view
        if (distanceSquared > fogViewDistance * fogViewDistance)
            return false;

        //Get player forward vector
        Vector3 playerForward = player.transform.forward;

        //Dot the two vectors
        float dotProduct = Vector3.Dot(playerForward, playerToEnemy);

        //This will tell us if the player is looking at the enemy, we need 0.8 for a more precise view angle
        return dotProduct > 0.8f;
    }
}
