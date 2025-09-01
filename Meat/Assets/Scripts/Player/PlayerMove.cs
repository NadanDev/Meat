using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody rb;

    public Transform orientation;

    public bool inHall;

    [Header("Movement Variables")]
    float speed = 100f;
    [SerializeField] float airMultiplier = 0.4f;
    [SerializeField] float playerHeight = 2f;
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float acceleration = 10f;
    private Vector3 movement;
    private Vector3 slopeMoveDirection;

    [Header("Jump Variables")]
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    [Header("Drag")]
    [SerializeField] float moveDrag = 5f;
    [SerializeField] float airDrag = 2f;

    [Header("Keybinds")]
    [SerializeField] KeyCode runKey = KeyCode.LeftShift;

    [Header("Head Bob")]
    [SerializeField] Transform playerCam;
    [SerializeField] float headBobAmount;
    [SerializeField] float headBobSpeed;
    float bobAmt = 0;
    float playerCamOriginalPositionY;

    [Header("Sounds")]
    [SerializeField] AudioSource Step1;
    private float stepCoolDown = 0f;
    private float stepRate = 0.8f;

    //GENERAL
    private float horizontalMove;
    private float verticalMove;

    private bool started = false;

    private RaycastHit slopeHit;

    private bool onSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool isGrounded()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerCamOriginalPositionY = playerCam.localPosition.y;

        StartCoroutine(WaitForStart());
    }


    private void Update()
    {
        if (!started)
        {
            return;
        }

        getInput();
        drag();
        controlSpeed();

        if (isGrounded())
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }
    }

    private void LateUpdate()
    {
        headBob();
    }


    private void FixedUpdate()
    {
        movePlayer();
    }


    private void getInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        if (inHall)
        {
            horizontalMove = 0;
            verticalMove = Mathf.Abs(verticalMove);
        }

        movement = (orientation.forward * verticalMove + orientation.right * horizontalMove).normalized;
        slopeMoveDirection = Vector3.ProjectOnPlane(movement, slopeHit.normal);
    }


    private void controlSpeed()
    {
        if (Input.GetKey(runKey) && isGrounded())
        {
            speed = Mathf.Lerp(speed, runSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            speed = Mathf.Lerp(speed, walkSpeed, acceleration * Time.deltaTime);
        }
    }


    private void drag()
    {
        if (isGrounded())
        {
            rb.linearDamping = moveDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
        }
    }


    private void movePlayer()
    {
        if (isGrounded() && !onSlope()) //Ground
        {
            rb.AddForce(movement * speed, ForceMode.Acceleration);
        }
        else if (isGrounded() && onSlope()) //Slope
        {
            rb.AddForce(slopeMoveDirection * speed, ForceMode.Acceleration);
        }
        else //Air
        {
            rb.AddForce(movement * speed * airMultiplier, ForceMode.Acceleration);
        }
    }

    private void headBob()
    {
        if (Mathf.Abs(horizontalMove) > 0 || Mathf.Abs(verticalMove) > 0)
        {
            bobAmt += ((Mathf.Abs(horizontalMove) + Mathf.Abs(verticalMove)) / (Mathf.Abs(horizontalMove) + Mathf.Abs(verticalMove))) * (Input.GetKey(runKey) ? headBobSpeed * 1.45f : headBobSpeed);
        }
        else
        {
            float nearestPI = (int)(bobAmt / Mathf.PI) * Mathf.PI;
            bobAmt = Mathf.Lerp(bobAmt, nearestPI, 0.01f);
        }

        float sinVal = Mathf.Sin(bobAmt);
        playerCam.localPosition = new Vector3(playerCam.localPosition.x, playerCamOriginalPositionY + (sinVal * headBobAmount), playerCam.localPosition.z);

        stepCoolDown -= Time.deltaTime;
        if ((Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) && stepCoolDown < 0f)
        {
            Step1.pitch = 1f + Random.Range(-0.3f, 0.3f);
            Step1.Play();
            stepCoolDown = Input.GetKey(runKey) ? stepRate * 0.65f : stepRate;
        }
    }

    IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(4f);
        started = true;
    }
}
