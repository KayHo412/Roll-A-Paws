using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    private float movementX;
    private float movementY;
    private bool jumpRequested = false;
    private bool isGrounded = true;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("UI & Game States")]
    public TextMeshProUGUI countText;
    private int count;
    public GameObject FinishPanel;
    public EnemyAI enemyScript;

    private CollectibleSpawner spawner;
    private int targetMaxCollectibles = 7;

    void Start()
    {
        // Get the Rigidbody component attached to the player
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        spawner = FindFirstObjectByType<CollectibleSpawner>();
        if (spawner != null)        {
            targetMaxCollectibles = spawner.TotalCollectibles;
        }

        count = 0;
        SetCountText();
        FinishPanel.SetActive(false);
    }

    void OnMove(InputValue movementValue) //Input Listener for movement input
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;

        Debug.Log("Movement X: " + movementX + ", Movement Y: " + movementY);
    }

    void OnJump()
    {
        if (isGrounded)
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        // 1. Handle Ground Checking
        CheckGroundStatus();

        // 2. Handle Movement Physics
        Vector3 movement = new Vector3(movementX, 0.0f, movementY).normalized;

        // Apply velocity directly on X and Z axis to give snappy animal controls
        Vector3 targetVelocity = movement * speed;
        Vector3 currentVelocity = rb.linearVelocity; // Note: Use rb.velocity instead if you encounter compatibility flags in older preview settings

        rb.linearVelocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);

        // 3. Handle Rotations (Face the moving direction)
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // 4. Handle Jumping Physics
        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }

        // 5. Feed the Animator Variables
        if (animator != null)
        {
            // Set "Vert" to the magnitude of movement so it blends idle -> walk -> run
            animator.SetFloat("Vert", movement.magnitude);

            // Set "State" to 1 if moving, 0 if idling (matching the asset layout)
            animator.SetFloat("State", movement.magnitude > 0.1f ? 1f : 0f);
        }
    }

    void CheckGroundStatus()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject); // Destroy the player object
            countText.gameObject.SetActive(false);
            FinishPanel.SetActive(true);
            FinishPanel.GetComponentInChildren<TextMeshProUGUI>().text = "You Lose!";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    void SetCountText()
{
    countText.text = "Number of coins collected: " + count.ToString() + " / " + targetMaxCollectibles.ToString();

    if (count >= targetMaxCollectibles)
    {
        EnemyAI[] activeEnemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        foreach (EnemyAI enemy in activeEnemies)
        {
            if (enemy != null) enemy.TriggerDissolve();
        }

        SceneManagement sceneManager = Object.FindFirstObjectByType<SceneManagement>();
        if (sceneManager != null)
        {
            sceneManager.ShowNextLevelButton();
        }

        countText.gameObject.SetActive(false);
        FinishPanel.SetActive(true);
        FinishPanel.GetComponentInChildren<TextMeshProUGUI>().text = "You Win!";
    }
}
}

