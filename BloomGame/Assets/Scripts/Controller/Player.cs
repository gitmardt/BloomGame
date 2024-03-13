using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using Cinemachine.Utility;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public static Player instance;

    public float health = 25;
    public float maxHealth = 30;
    public float ammo = 100;
    public float maxAmmo = 150;
    public float lightAmmo = 4;
    public float maxLightAmmo = 5;

    public Image ammoBar, healthBar;
    public ObjectArrayCounter LightMinionUI;

    [Header("Hitmarker info")]
    public Hitmarker hitmarker;
    public SpriteAnimationManager hitmarkerAM;
    public SpriteAnimation In, Out, Shoot1, Shoot2;
    private int shootIndex = 0;

    //Temp projectiles
    [Header("Projectiles")]
    public GameObject projectile;
    public bool twoBarrels;
    public Transform barrel, barrel2, projectileParent;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 1f;
    public float projectileSpread = 6f;

    public InputMaster controls;

    [Header("Main References")]
    public CinemachineVirtualCameraBase mainCamera;
    public Transform cinemachineBrainCamera;
    public Transform aimTarget, aimTargetClose;
    private Rigidbody rb;

    [Header("Movement")]
    public float aimSensitivity = 1;
    public float minimumAimDistance = 0.3f;
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
    private Vector3 inputDirection;
    private Vector3 moveDirection;
    private RaycastHit groundHit;
    private bool inAir = false;
    private float targetHeight;
    private float currentHeight;
    private bool crouching = false;
    private Plane groundPlane = new(Vector3.up,Vector3.zero);
    private bool aiming;

    [HideInInspector] public Vector3 aimDirection;
    [HideInInspector] public Vector3 mousePosition;

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;

        instance = this;

        rb = GetComponent<Rigidbody>();

        controls = new InputMaster();

        //Movement
        controls.Combat.Movement.performed += ctx => inputDirection = ctx.ReadValue<Vector2>();
        controls.Combat.Movement.canceled += ctx => inputDirection = Vector2.zero;

        //Aim
        controls.Combat.Aim.performed += ctx => ChangeView(false);
        controls.Combat.Aim.canceled += ctx => ChangeView(true);

        ////Look
        //controls.Combat.Look.performed += ctx => lookDirection = ctx.ReadValue<Vector2>();
        //controls.Combat.Look.performed += ctx => lookDirection = Vector2.zero;

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

    private void Start()
    {
        LightMinionUI.amount = lightAmmo;
        ChangeView(true);
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Combat) StartCoroutine(StartDelay());
        else controls.Disable();
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.2f);
        controls.Enable();
    }

    private void ChangeView(bool aim)
    {
        aiming = aim;
        if (aim)
        {
            mainCamera.Follow = aimTarget;
            hitmarkerAM.PlaySpriteAnimation(In, hitmarker.image);
            LightMinionSpawner.instance.activeMode = LightMinionSpawner.ActiveMode.off;
            LightMinionSpawner.instance.StopSpawning();
        }
        else 
        { 
            mainCamera.Follow = aimTargetClose;
            hitmarkerAM.PlaySpriteAnimation(Out, hitmarker.image);
            LightMinionSpawner.instance.activeMode = LightMinionSpawner.ActiveMode.spawning;
            LightMinionSpawner.instance.StartSpawning();
        }
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        LockCursor();
        Aim();
        UpdateUISliders();
    }

    void FixedUpdate() 
    {
        Move();
    }

    private void UpdateUISliders()
    {
        ammoBar.fillAmount = Utility.Remap(ammo, 0, maxAmmo, 0, 1);
        healthBar.fillAmount = Utility.Remap(health, 0, maxHealth, 0, 1);
    }

    private void Aim()
    {
        groundPlane.SetNormalAndPosition(Vector3.up, new Vector3(0, transform.position.y, 0));
        Ray ray = Camera.main.ScreenPointToRay(controls.Combat.Look.ReadValue<Vector2>());

        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow); 

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            mousePosition = point;

            aimDirection = point - transform.position;
            aimDirection.y = 0; 
        }
    }

    private void Move()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit))
        {
            if (inputDirection.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg;
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
            else
            {
                moveDirection = Vector3.zero;
            }

            if (aimDirection.magnitude > minimumAimDistance) 
            {
                Quaternion lookRotation = Quaternion.LookRotation(aimDirection.normalized);
                //Debug.Log(transform.rotation + " " + lookRotation.eulerAngles);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * aimSensitivity); 
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
            canJump = true;
            rb.useGravity = false;
            rb.drag = groundDrag;

            // set the target height to the float height plus the distance to the ground
            if (!crouching) targetHeight = groundHit.point.y + floatHeight;
            else targetHeight = groundHit.point.y + crouchHeight;

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
            canJump = false;
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

    private bool CheckAmmo()
    {
        if (ammo == 0)
        {
            Debug.Log("No ammo");
            return false;
        }
        else return true;
    }

    public void OnShoot()
    {
        if (LightMinionSpawner.instance.activeMode == LightMinionSpawner.ActiveMode.spawning ||
            LightMinionSpawner.instance.activeMode == LightMinionSpawner.ActiveMode.removal)
        {
            LightMinionSpawner.instance.SpawnLight();
            return;
        }

        if (!CheckAmmo()) return;
        ammo--;

        shootIndex++;
        if (shootIndex == 100) shootIndex = 0;

        if (aiming)
        {
            if (shootIndex % 2 == 0) hitmarkerAM.PlaySpriteAnimation(Shoot1, hitmarker.image);
            else hitmarkerAM.PlaySpriteAnimation(Shoot2, hitmarker.image);
        }

        Vector3 spawnPosition;
        if (twoBarrels)
        {
            if (shootIndex % 2 == 0) spawnPosition = barrel.position;
            else spawnPosition = barrel2.position;
        }
        else 
        {
            spawnPosition = barrel.position;
        }

        Vector3 newAim = aimDirection - barrel.localPosition;
        newAim.y = 0;
        GameObject projectile = Instantiate(this.projectile, spawnPosition, Quaternion.LookRotation(newAim.normalized), projectileParent);
        ParticleProjectile pp = projectile.GetComponent<ParticleProjectile>();
        pp.speed = projectileSpeed;
        pp.spread = projectileSpread;
        StartCoroutine(ShootRoutine(projectile));
    }

    private IEnumerator ShootRoutine(GameObject projectile)
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(projectile);
    }
}
