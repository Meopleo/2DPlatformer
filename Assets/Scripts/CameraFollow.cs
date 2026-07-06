using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Követési beállítások")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float yOffset = 3.5f;

    [Header("Menü Beállítások")]
    public float startMovingHeight = 5.5f;

    private float currentCameraY;
    private Rigidbody2D targetRb;
    private float menuCameraY;

    // --- ÚJ: A kamera memóriája ---
    private bool hasGameStarted = false;

    private void Start()
    {
        menuCameraY = transform.position.y;
        currentCameraY = menuCameraY;
        hasGameStarted = false; // Induláskor alaphelyzetbe tesszük

        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
        }
    }

    private void LateUpdate()
    {
        if (target == null || targetRb == null) return;

        // 1. Ha még NEM indult el a játék
        if (!hasGameStarted)
        {
            if (target.position.y < startMovingHeight)
            {
                // Még a menüben ugrálunk, a kamera kőkeményen a menün marad!
                Vector3 lockedPos = new Vector3(transform.position.x, menuCameraY, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, lockedPos, smoothSpeed * Time.deltaTime);
                return;
            }
            else
            {
                // Amint átlépjük a határt, a kamera örökre elfelejti a menüt!
                hasGameStarted = true;
            }
        }

        // 2. --- HA A JÁTÉK MÁR ELINDULT ---
        // Innentől a kamera soha többé nem mehet lefelé!
        float desiredCameraY = target.position.y + yOffset;

        if (desiredCameraY > currentCameraY && targetRb.linearVelocity.y > 0.1f)
        {
            currentCameraY = desiredCameraY;
        }

        Vector3 targetPos = new Vector3(transform.position.x, currentCameraY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}