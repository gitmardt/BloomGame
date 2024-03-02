using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMovement : MonoBehaviour
{
    public InputMaster controls;

    [Header("References")]
    public Transform cam;
    private Rigidbody rb;

    [Header("Movement")]
    public float speed = 6f;
    public float sprintMultiplier = 2f;
    public float turnSmoothTime = 0.1f;
    public float airSpeedMultiplier = 0.4f;
    public float gravity = -9.81f;
    public float floatHeight = 5f;
    public float crouchHeight = 2f;
    public float floatDamp = 0.5f;

    [Header("Jumping")]
    //jumping
    public float jumpingPower = 24f;
    public float jumpingCooldown = 1f;
    private bool canJump;

    [Header("Dash")]
    public float dashAmount = 50f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;
    private bool canDash;

    [Header("RB Settings")]
    public float groundDrag = 4;
    public float airDrag = 0;

    //Private variables
    private float turnSmoothVelocity;
    private Vector3 inputDirection;
    private Vector3 moveDirection;
    private RaycastHit groundHit;
    private bool inAir = false;
    private float targetHeight;
    private float currentHeight;
    private bool crouching = false;

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        controls = new InputMaster();

        //Movement
        controls.Combat.Movement.performed += ctx => inputDirection = ctx.ReadValue<Vector2>();
        controls.Combat.Movement.canceled += ctx => inputDirection = Vector2.zero;

        //Crouching
        crouching = false;
        controls.Combat.Crouch.performed += ctx => crouching = true;
        controls.Combat.Crouch.canceled += ctx => crouching = false;

        //Jump
        canJump = true;
        controls.Combat.Jump.performed += ctx => Jump();

        //Dash
        canDash = true;
        controls.Combat.Dash.performed += ctx => Dash();

        //Shoot
        controls.Combat.Shoot.performed += ctx => OnShoot();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        LockCursor();
        Move();
    }

    private void Move()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit))
        {
            if (inputDirection.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                if (!inAir)
                {
                    rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
                }
                else
                {
                    rb.AddForce(moveDirection.normalized * speed * airSpeedMultiplier, ForceMode.Force);
                }

                SpeedControl();
            }
            CheckGravity();
        }
        else
        {
            Debug.LogWarning("No ground found");
        }
    }

    private void CheckGravity()
    {
        if (rb.position.y <= getFloatHeight())
        {
            //canJump = true;

            rb.useGravity = false;
            rb.drag = groundDrag;

            // set the target height to the float height plus the distance to the ground
            if (!crouching)
            {
                targetHeight = groundHit.point.y + floatHeight;
            }
            else
            {
                targetHeight = groundHit.point.y + crouchHeight;
            }


            // smoothly adjust the current height towards the target float height
            currentHeight = Mathf.Lerp(transform.position.y, targetHeight, Time.deltaTime * (floatDamp - rb.velocity.y));

            // apply the new height to the rigidbody
            Vector3 newPosition = new Vector3(transform.position.x, currentHeight, transform.position.z);
            rb.MovePosition(newPosition);

            inAir = false;
        }
        else if (rb.position.y > getFloatHeight())
        {
            rb.drag = airDrag;
            rb.useGravity = true;
            inAir = true;
            //canJump = false;
        }
    }

    private void Jump() => StartCoroutine(JumpRoutine());
    private IEnumerator JumpRoutine()
    {
        if (canJump)
        {
            canJump = false;
            rb.AddForce(Vector3.up * jumpingPower, ForceMode.Impulse);
            yield return new WaitForSeconds(jumpingCooldown);
            canJump = true;
        }
    }

    private void Dash() => StartCoroutine(DashRoutine());
    private IEnumerator DashRoutine()
    {
        if (canDash)
        {
            canDash = false;
            float t = 0;
            while(t < dashDuration)
            {
                float decel = Utility.Remap(t, 0, dashDuration, 1, 0);
                rb.AddForce(moveDirection.normalized * dashAmount * decel, ForceMode.Force);
                yield return null;
                t += Time.deltaTime;
            }
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }

    float getFloatHeight() => groundHit.point.y + floatHeight;

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity if needed
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    public void OnShoot()
    {
        Debug.Log("Shoot!");
    }
}
