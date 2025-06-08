using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private enum State
    {
        Stalking,
        Lurking,
        BackingAway,
        DartingAway,
        Pausing
    }

    private State currentState;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject player;

    [Header("Distances")]
    [SerializeField] private float minDistance = 5f;       // Never closer than this
    [SerializeField] private float lurkDistance = 8f;      // Distance to lurk/watch player
    [SerializeField] private float dartDistance = 20f;     // Distance to dart away to

    [Header("Speeds")]
    [SerializeField] private float stalkSpeed = 2f;
    [SerializeField] private float backAwaySpeed = 3.5f;
    [SerializeField] private float dartSpeed = 8f;

    [Header("Timers")]
    [SerializeField] private float backAwayDuration = 3f;
    [SerializeField] private float pauseDuration = 4f;

    private float timer;

    private void Start()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player");
        currentState = State.Lurking;
        timer = 0f;
        agent.speed = stalkSpeed;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Stalking:
                StalkPlayer();
                break;
            case State.Lurking:
                Lurk();
                break;
            case State.BackingAway:
                BackAway();
                break;
            case State.DartingAway:
                DartAway();
                break;
            case State.Pausing:
                Pause();
                break;
        }
    }

    private void StalkPlayer()
    {
        agent.speed = stalkSpeed;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Move to a position exactly minDistance away, following player direction
        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
        Vector3 targetPos = player.transform.position - dirToPlayer * minDistance;

        agent.SetDestination(targetPos);

        // Face player smoothly
        FacePlayer();

        // If player gets closer than minDistance, back away
        if (distance < minDistance)
        {
            timer = backAwayDuration;
            currentState = State.BackingAway;
        }
        else if (distance > lurkDistance + 2f)
        {
            // If enemy too far, start stalking to close gap
            currentState = State.Stalking;
        }
    }

    private void Lurk()
    {
        agent.ResetPath();

        // Position at lurkDistance from player (if needed, move there)
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > lurkDistance + 0.5f || distance < lurkDistance - 0.5f)
        {
            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
            Vector3 targetPos = player.transform.position - dirToPlayer * lurkDistance;
            agent.speed = stalkSpeed;
            agent.SetDestination(targetPos);
        }
        else
        {
            agent.ResetPath();
        }

        FacePlayer();

        // If player approaches closer than lurkDistance but farther than minDistance, stalk
        if (distance < lurkDistance && distance > minDistance)
        {
            currentState = State.Stalking;
        }
        // If player comes very close, back away
        else if (distance <= minDistance)
        {
            timer = backAwayDuration;
            currentState = State.BackingAway;
        }
    }

    private void BackAway()
    {
        agent.speed = backAwaySpeed;

        // Move away from player but still face them
        Vector3 dirAway = (transform.position - player.transform.position).normalized;
        Vector3 retreatTarget = transform.position + dirAway * 1.5f; // small step back

        agent.SetDestination(retreatTarget);

        FacePlayer();

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // Dart away to a random spot far from player
            Vector3 randomDir = Random.onUnitSphere;
            randomDir.y = 0;
            Vector3 dartTarget = player.transform.position + randomDir.normalized * dartDistance;
            agent.speed = dartSpeed;
            agent.SetDestination(dartTarget);

            currentState = State.DartingAway;
        }
    }

    private void DartAway()
    {
        FacePlayer();

        // Check if reached dart destination or close enough
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            timer = pauseDuration;
            currentState = State.Pausing;
            agent.ResetPath();
        }
    }

    private void Pause()
    {
        FacePlayer();

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            currentState = State.Lurking;
        }
    }

    private void FacePlayer()
    {
        Vector3 lookDir = (player.transform.position - transform.position);
        lookDir.y = 0; // keep only horizontal rotation
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }
    }
}
