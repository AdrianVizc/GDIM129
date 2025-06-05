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
    private float bufferApproachDistance = 1f;

    //Timers
    private float stateTimer;
    private float idleDuration = 10f;
    private float lurkDuration = 7f;
    private float disappearDuration = 5f;

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
        ChangeStates();
        HandleStates();
        Debug.Log(currentState);
    }

    private void ChangeStates()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.idle:
                if (stateTimer <= 0f)
                {
                    currentState = EnemyState.approach;
                    stateTimer = 0f;
                    hasDestination = false;
                }
                break;

            case EnemyState.approach:
                if (agent.remainingDistance < maxApproachDistance)
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
                if (agent.remainingDistance < maxApproachDistance)
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
            Vector3 target = player.transform.position + direction * (fogViewDistance + bufferApproachDistance - 1f);
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
        if (!hasDestination)
        {
            agent.SetDestination(GetRandomPointInFog());
            hasDestination = true;
        }
    }

    private void DisappearLogic()
    {
        agent.ResetPath();
        hasDestination = false;

        Vector3 directionAway = (transform.position - player.transform.position).normalized;
        transform.position += directionAway * disappearSpeed * Time.deltaTime;
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
            //Create a random circle of size maxApproachDistance
            Vector2 randomCircle = Random.insideUnitCircle.normalized * (fogViewDistance + bufferApproachDistance);

            //Pick a point with respect to where the player is. Also y --> z from 2D to 3D space
            Vector3 possiblePoint = player.transform.position + new Vector3(randomCircle.x, player.transform.position.y, randomCircle.y);
            
            //Check where this point is with respect to the player
            Vector3 pointToPlayer = possiblePoint - player.transform.position;
            
            //If this point is outside the player's fogViewDistance + some buffer
            if (pointToPlayer.magnitude < fogViewDistance + bufferApproachDistance)
            {
                //Then we can set it as our location to go
                possiblePoint = player.transform.position + pointToPlayer.normalized * (fogViewDistance + bufferApproachDistance);
            }

            possiblePoint.y = player.transform.position.y;

            if (GetValidNavmeshPoint(possiblePoint) != transform.position)
                return GetValidNavmeshPoint(possiblePoint);
        }

        return GetValidNavmeshPoint(transform.position + Random.insideUnitSphere * 10f);
    }
}
