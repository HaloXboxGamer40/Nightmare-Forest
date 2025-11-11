using System.Collections.Generic;
using UnityEngine;

// This is old ass code I havn't played around with in years
// probably shouldn't use it but im to lazy to write a new
// player controller
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {

    // Inspecter Assigned variables
    [Header("Movement")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public bool IsWalking;
    public float runSpeed = 9f;
    public float speed = 5f;

    [Header("Camera")]
    public Transform cam;

    [Header("Jumping")]
    public float jumpStrength = 2;
    public event System.Action Jumped;

    [Header("Ground Checks")]
    [Tooltip("Maximum distance from the ground.")]
    public float distanceThreshold = .15f;

    [Tooltip("Whether this transform is grounded now.")]
    public bool isGrounded = true;

    public event System.Action Grounded;

    // Player Pos, this is cached so we can fix a bug
    Vector3 playerPos;

    // Extra ground check stuff
    const float OriginOffset = .001f;
    Vector3 RaycastOrigin => transform.position + Vector3.up * OriginOffset;
    float RaycastDistance => distanceThreshold + OriginOffset;

    // Put these in a settings menu at a later date
    [SerializeField]
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    // Velocity and other stuff
    Vector2 velocity;
    Vector2 frameVelocity;

    // Rigidbody ref and speed overrides list
    Rigidbody rb;
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    // Set our rigidbody
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        playerPos = new Vector3(0, 0, 0);
    }

    private void Start() {
        // Lock the mouse cursor to the game screen.
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate() {
        
        // Get inputs and move thee player
        MovePlayer();

    }

    private void Update() {
        
        // Move the camera
        UpdateCamera();
        //FixedPlayerPositionCalculation();

    }

    private void LateUpdate() {
        
        // Ground check MUST be first
        GroundCheck();
        Jump();

    }

    private void FixedPlayerPositionCalculation() {

        // Gaurdian clause so we don't do all of this every frame
        if (transform.position.Equals(playerPos))
            return;
        
        if (!isGrounded)
            return;
        
        transform.position = playerPos;

    }

    // Handle jumping
    private void Jump() {
        // Jump when the Jump button is pressed and we are on the ground.
        if (Input.GetButtonDown("Jump") && isGrounded) {
            // Use the cached Rigidbody and apply an impulse for jump.
            rb.AddForce(Vector3.up * 3 * jumpStrength, ForceMode.Impulse);
            Jumped?.Invoke();
        }
    }

    // Handle checking if we are touching the ground
    private void GroundCheck() {
        // Check if we are grounded now using the RaycastDistance property.
        bool isGroundedNow = Physics.Raycast(RaycastOrigin, Vector3.down, RaycastDistance);

        // If we aren't already grounded if we aren't in the air then we will tell that we're grounded
        if (isGroundedNow && !isGrounded)
            Grounded?.Invoke();

        isGrounded = isGroundedNow;
    }

    private void MovePlayer() {
        // Check if we're running
        IsRunning = canRun && Input.GetKey(KeyCode.LeftShift);

        // If we're running then we will override our speed to go faster
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();

        // Get inputs for wasd and arrow keys
        Vector3 localInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        // Now we calculate our local and world movements
        Vector3 localMove = localInput * targetMovingSpeed;
        Vector3 worldMove = transform.TransformDirection(localMove);

        // IsWalking = true when the player has movement input and is NOT running
        IsWalking = !IsRunning && localInput.sqrMagnitude > 0.0001f;

        // Update our position
        rb.MovePosition(rb.position + worldMove * Time.fixedDeltaTime);

        if (IsWalking || IsRunning)
            playerPos = transform.position;
    }


    // Move camera
    private void UpdateCamera() {

        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        // Rotate camera up-down and controller left-right from velocity.
        cam.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);

    }
}