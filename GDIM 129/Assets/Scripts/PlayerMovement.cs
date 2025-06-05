using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    [HideInInspector] public bool isDialogueOn;

    [Header("Footstep Audio")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip interiorStepClip;
    [SerializeField] private AudioClip exteriorStepClip;
    [SerializeField] private float stepRate = 0.5f;
    private float stepTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        isDialogueOn = false;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        rb.drag = grounded ? groundDrag : 0f;

        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        if (!isDialogueOn)
        {
            MovePlayer();
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void HandleFootsteps()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (grounded && flatVel.magnitude > 0.1f && !isDialogueOn)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                AudioClip clipToPlay = null;
                float pitch = 1f;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, playerHeight * 0.6f))
                {
                    if (hit.collider.CompareTag("InteriorFloor"))
                    {
                        clipToPlay = interiorStepClip;
                        pitch = Random.Range(0.95f, 1.05f); // Slight variation
                    }
                    else if (hit.collider.CompareTag("ExteriorFloor"))
                    {
                        clipToPlay = exteriorStepClip;
                        pitch = Random.Range(0.8f, 1.2f); // More variation
                    }
                }

                if (clipToPlay != null && footstepSource != null)
                {
                    footstepSource.pitch = pitch;
                    footstepSource.clip = clipToPlay;
                    footstepSource.Play();
                }

                stepTimer = stepRate;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
}
