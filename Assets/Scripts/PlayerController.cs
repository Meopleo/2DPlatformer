using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Fizika és Erő")]
    [SerializeField] private float launchForceMultiplier = 5f;
    [SerializeField] private float maxForce = 20f;
    [SerializeField] private float iceDamping = 10f;

    [Header("Játékmenet Beállítások")]
    [SerializeField] private int maxJumps = 3;

    public float menuHeight = 6f;

    [Header("Vizuális Beállítások")]
    [SerializeField] private float lineWidth = 0.05f;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private LineRenderer lr;

    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private bool isDragging = false;
    private int currentJumps;
    private bool isSticky = false;
    private float lastJumpTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.enabled = false;
        currentJumps = maxJumps;
    }

    private void Update()
    {
        if (transform.position.y < menuHeight)
        {
            currentJumps = maxJumps;
        }

        HandleInput();
        CheckFallDeath();
    }

    private void CheckFallDeath()
    {
        float screenBottomEdge = mainCamera.transform.position.y - mainCamera.orthographicSize;
        if (transform.position.y < screenBottomEdge - 0.5f)
        {
            Debug.Log("Kiestél a képernyőről! Újraindítás...");
            RestartLevel();
        }
    }

    private void HandleInput()
    {
        if (currentJumps <= 0 && !isSticky) return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            lr.enabled = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            dragEndPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            DrawTrajectory();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            lr.enabled = false;
            LaunchPlayer();
        }
    }

    private void DrawTrajectory()
    {
        Vector2 launchDirection = dragStartPos - dragEndPos;
        float forceMagnitude = Mathf.Clamp(launchDirection.magnitude * launchForceMultiplier, 0f, maxForce);
        Vector3 startPoint = transform.position;
        Vector3 endPoint = startPoint + (Vector3)(launchDirection.normalized * (forceMagnitude / launchForceMultiplier));
        lr.SetPosition(0, startPoint);
        lr.SetPosition(1, endPoint);
    }

    private void LaunchPlayer()
    {
        Vector2 launchDirection = dragStartPos - dragEndPos;
        float forceMagnitude = Mathf.Clamp(launchDirection.magnitude * launchForceMultiplier, 0f, maxForce);

        // ---> INNEN VETTÜK KI A GYENGÍTÉST! <---
        // Mostantól szabadon, teljes erőből repülhetsz a menüben is.

        DetachFromWall();
        rb.AddForce(launchDirection.normalized * forceMagnitude, ForceMode2D.Impulse);

        currentJumps--;
        lastJumpTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision) { TryAttach(collision); }
    private void OnCollisionStay2D(Collision2D collision) { TryAttach(collision); }

    private void TryAttach(Collision2D collision)
    {
        // JAVÍTÁS: 0.1f-ről 0.02f-re csökkentettük a várakozást!
        // Így hiába lősz ki izomból egy közeli falra, a szörny azonnal tapadni fog, és nem pattan le.
        if (Time.time > lastJumpTime + 0.02f && !isSticky)
        {
            if (collision.gameObject.CompareTag("Sticky"))
            {
                AttachToWall();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Sticky")) DetachFromWall();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ice")) rb.linearDamping = iceDamping;
        if (collision.gameObject.CompareTag("Spike")) RestartLevel();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ice")) rb.linearDamping = 0f;
    }

    private void RestartLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    private void AttachToWall() { isSticky = true; rb.linearVelocity = Vector2.zero; rb.gravityScale = 0f; currentJumps = maxJumps; }
    private void DetachFromWall() { isSticky = false; rb.gravityScale = 1f; }
}