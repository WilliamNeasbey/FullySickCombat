using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerChunLi : MonoBehaviour
{
    [HideInInspector] public CharacterController charCont;
    [HideInInspector] public Animator anim;
    public GameObject childPlayer;
    public GameObject movIndicator; //Where is the character moving to

    [HideInInspector] public SoundManager soundMan;

    public float speed = 6.0f; //Speed of the player
    public float jumpSpeed = 8.0f; //How high does the player jump
    public float gravity = 20.0f; //Gravity applied to the player
    private bool canJump = true;
    private bool wasGrounded = false; //Dettects the first time the player is falling

    private float mass = 60.0f; // Defines the character mass
    private Vector3 impact = Vector3.zero;

    public bool airControl = true; //If player can control the direction of the movement on falling
    private float fallTime = 0f; //Time the player is falling

    public float maxDashTime = 0.25f; //Time that the player is dashing
    private float currentDashTime;
    public float dashSpeed = 20; //Dash speed of the player
    private Vector3 dashDir;
    private bool canDash = true;
    

    private Vector3 moveDirection = Vector3.zero;

    private float distToGround; //Distance to the ground for check if there is ground under the character
    private Vector3 groundNormal;

    [HideInInspector] public bool canMove = true; //If player can move or not because is attaking or hitted
    private bool hit = false; //If player is hitted it will be true

    // Camera variables
    public CinemachineVirtualCamera virtualCamera;
    private Transform cameraTransform;
    private float cameraDistance = 10f;
    public float cameraSpeed = 2.0f;
    private Vector2 cameraInput;

    //Player Health
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public Text healthText; // Reference to the UI text component for displaying health

    //double jump
    public int maxJumps = 2; // Maximum number of jumps the player can perform
    private int jumpsRemaining; // Number of jumps remaining


    void Awake()
    {
        charCont = GetComponent<CharacterController>();
        soundMan = GetComponent<SoundManager>();
        anim = GetComponentInChildren<Animator>();
        currentDashTime = maxDashTime;
        distToGround = charCont.bounds.extents.y;
        jumpsRemaining = maxJumps;

        // Get the virtual camera and its transform
        virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("Virtual camera not found in the scene!");
        }
        else
        {
            cameraTransform = virtualCamera.transform;
        }
    }
   

    void Update()
    {

        // Handle camera movement
        HandleCameraMovement();

        UpdateHealthBar();

        if (charCont.isGrounded)
        {
            if (!wasGrounded) //If it is the frame when player touches the ground
            {
                jumpsRemaining = 2; // Reset the jumps remaining when the player touches the ground
                canJump = true;
                anim.SetBool("Jump", false);
                if (fallTime > 0.2f)
                {
                    soundMan.PlaySound("Land");
                    if (!hit)
                        anim.CrossFade("FallingEnd", 0.1f);
                }
                fallTime = 0f;
            }
        }
        else
        {
            anim.SetFloat("SpeedY", charCont.velocity.y);
            if (wasGrounded && canJump) //If it is the first frame of falling
            {
                if (DistToGround() > 0.3f) //If the ground is far enough
                {
                    moveDirection.y = 0f;
                    wasGrounded = false;
                    anim.SetBool("Jump", true); //Needed for activate the jump state if character falls
                    anim.CrossFade("Falling", 0.2f);
                }
            }
            if (charCont.velocity.y < 0) //If player is falling down
                fallTime += Time.deltaTime;
        }
        wasGrounded = charCont.isGrounded;

        if (!canMove)
        {
            if (hit)
            {
                moveDirection.y -= gravity * Time.deltaTime;
                Vector3 impactGrav = new Vector3(impact.x, impact.y + moveDirection.y, impact.z); //Adds gravity to the impact force
                                                                                                  // apply the impact force:
                if (impact.magnitude > 0.2f || !charCont.isGrounded) charCont.Move(impactGrav * Time.deltaTime);
                // consumes the impact energy each cycle:
                impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

                if (charCont.isGrounded && impact.magnitude <= 0.2f)
                {
                    hit = false;
                    canMove = true;
                    anim.Play("Idle"); //To unlock the animation in some weird cases
                }
            }
            return;
        }

        if (charCont.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (moveDirection.magnitude < 0.1f) //Because if velocity is minor than 0.1 the animator dont play the correct animation
                moveDirection = Vector3.zero;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1); //Limits the vector magnitude to 1
            anim.SetFloat("Speed", moveDirection.magnitude);

            movIndicator.transform.localPosition = moveDirection; //Positionate the reference that indicates the direction of the movement

            if (Input.GetButtonDown("Dash") && canDash) //Dash
            {
                currentDashTime = 0;
                canDash = false;
                anim.Play("Slide");
                soundMan.PlaySound("Dash");
                if (moveDirection != Vector3.zero)
                {
                    dashDir = transform.TransformDirection(moveDirection).normalized;

                    //Guide the player to where they are going to move
                    Vector3 targetActPosition = new Vector3(movIndicator.transform.position.x, childPlayer.transform.position.y, movIndicator.transform.position.z);
                    childPlayer.transform.rotation = Quaternion.LookRotation(targetActPosition - childPlayer.transform.position);
                }
                else
                    dashDir = childPlayer.transform.forward;
            }
            if (currentDashTime < maxDashTime)
            {
                dashDir.y = -10f;
                currentDashTime += Time.deltaTime;

                charCont.Move(dashDir * Time.deltaTime * dashSpeed);
                return;
            }
            canDash = true;

            if (moveDirection.magnitude > 0) //Fixes the problem when there is no movement
            {
                //To rotate the controller when moving and position it correctly relative to the camera
                charCont.transform.rotation = new Quaternion(charCont.transform.rotation.x, transform.rotation.y, charCont.transform.rotation.z, transform.rotation.w);

                //Smoothly rotate the character in the xz plane towards the direction of movement
                Vector3 targetActPosition = new Vector3(movIndicator.transform.position.x, childPlayer.transform.position.y, movIndicator.transform.position.z);
                Quaternion rotation = Quaternion.LookRotation(targetActPosition - childPlayer.transform.position);
                childPlayer.transform.rotation = Quaternion.Slerp(childPlayer.transform.rotation, rotation, Time.deltaTime * 10);
            }

            //Rotate it to the player orientation
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed; // apply the horizontal speed


            moveDirection.y = -10f; //To prevent the controller from taking off when going down ramps
            if (!IsGrounded()) //If the character controller says is grounded but the raycast to the ground no
            {
                moveDirection.x += (1f - groundNormal.y) * groundNormal.x;
                moveDirection.z += (1f - groundNormal.y) * groundNormal.z;
            }

          

            if (Input.GetButtonDown("Jump") && canJump)
            {
                if (jumpsRemaining > 0) // Check if the player has jumps remaining
                {
                    moveDirection.y = jumpSpeed;
                    canJump = false;
                    anim.SetFloat("SpeedY", moveDirection.y);
                    anim.Play("Falling");
                    anim.SetBool("Jump", true);
                    soundMan.PlaySound("Jump");
                    jumpsRemaining--; // Decrease the number of jumps remaining
                }
            }

        }
        else
        {
            if (currentDashTime < maxDashTime) //If player was dashing
            {
                currentDashTime = maxDashTime;
                canDash = true;
            }
            if (airControl)
            {
                Vector3 moveDirectionTemp = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                moveDirectionTemp = Vector3.ClampMagnitude(moveDirectionTemp, 1); //Limits the vector magnitude to 1
                moveDirection = new Vector3(moveDirectionTemp.x, moveDirection.y, moveDirectionTemp.z);

                movIndicator.transform.localPosition = new Vector3(moveDirection.x, 0, moveDirection.z); //Positionate the reference that indicates the direction of the movement

                if (moveDirectionTemp.magnitude > 0) //Fixes the problem when there is no movement
                {
                    //To rotate the controller when moving and position it correctly relative to the camera
                    charCont.transform.rotation = new Quaternion(charCont.transform.rotation.x, transform.rotation.y, charCont.transform.rotation.z, transform.rotation.w);

                    //Smoothly rotate the character in the xz plane towards the direction of movement
                    Vector3 targetActPosition = new Vector3(movIndicator.transform.position.x, childPlayer.transform.position.y, movIndicator.transform.position.z);
                    Quaternion rotation = Quaternion.LookRotation(targetActPosition - childPlayer.transform.position);
                    childPlayer.transform.rotation = Quaternion.Slerp(childPlayer.transform.rotation, rotation, Time.deltaTime * 10);
                }
                // rotate it to the player orientation
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection = new Vector3(moveDirection.x * speed * 0.8f, moveDirection.y, moveDirection.z * speed * 0.8f);
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        charCont.Move(moveDirection * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamagePercentage(0.1f);
        }
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    void UpdateHealthBar()
    {
        // Update the health UI text with the current health value
        healthText.text = "Health: " + currentHealth.ToString("F0");
    }

    public void ApplyDMG(float damage)
    {
        currentHealth -= damage;

        UpdateHealthBar();

        CheckGameOver();
    }

    public void TakeDamagePercentage(float percentage)
    {
        float damage = maxHealth * percentage;
        currentHealth -= damage;

        // Check if the player's health is less than or equal to 0
        if (currentHealth <= 0)
        {
            // Call a function to load a new scene here
            LoadGameOverScene();
        }
    }

    void LoadGameOverScene()
    {
        // Load the game over scene or perform any other necessary actions
        SceneManager.LoadScene("GameOverScene");
    }



    void CheckGameOver()
    {
        if (currentHealth <= 0)
        {
            // Load the game over scene
            SceneManager.LoadScene("GameOverScene");
        }
    }


    void HandleCameraMovement()
    {
        // Get the camera input
        float cameraInputX = Input.GetAxis("Mouse X");
        float cameraInputY = Input.GetAxis("Mouse Y");

        // Rotate the player based on camera input
        transform.Rotate(Vector3.up, cameraInputX * cameraSpeed);

        // Adjust the camera distance based on camera input
        cameraDistance -= cameraInputY * cameraSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, 5f, 15f);

        // Calculate the camera rotation around the player
        Quaternion cameraRotation = Quaternion.Euler(cameraInputY * cameraSpeed, 0f, 0f);

        // Rotate the camera around the player
        Vector3 cameraOffset = cameraRotation * Vector3.back * cameraDistance;
        Vector3 cameraPosition = transform.position + cameraOffset;
        cameraTransform.position = cameraPosition;
        cameraTransform.LookAt(transform.position);
    }

    public void AddImpact(Vector3 dir, float force) //Apply a force to the player
    {
        moveDirection = Vector3.zero;
        anim.Play("Hit");

        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; //Reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    public void ApplyDMG(Vector3 dir, float force) //Apply damage to the player
    {
        if (!hit)
        {
            hit = true;
            canMove = false;
            soundMan.PlaySound("Hit");
            currentDashTime = maxDashTime; //Cancels dash if was pressed
            anim.SetFloat("Speed", 0);
            anim.GetComponent<AnimatorEvents>().DisableWeaponColl();
            AddImpact(dir, force);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        UpdateHealthBar();

        CheckGameOver();
    }

    float DistToGround() //Calculates the distance to the ground when starts to fall
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 999))
            return hit.distance - distToGround;
        else return 999;
    }

    bool IsGrounded() //Check if ground is under the character
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        groundNormal = hit.normal;
        //groundAngle = Vector3.Angle(groundNormal, Vector3.up);
    }

    public bool IsDashing()
    {
        return currentDashTime < maxDashTime;
    }
}