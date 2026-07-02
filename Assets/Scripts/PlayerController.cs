using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))] // A célzóvonalhoz szükséges komponens
public class PlayerController : MonoBehaviour
{
    [Header("Fizika és Erő")]
    [SerializeField] private float launchForceMultiplier = 5f; // Erőszorzó a kilövéshez
    [SerializeField] private float maxForce = 20f;             // Maximális kilövési erő
    [SerializeField] private float iceDamping = 10f; // Mennyire fékezzen le a jég


    [Header("Játékmenet Beállítások")]
    [SerializeField] private int maxJumps = 3;                 // Alapértelmezett lövések száma

    [Header("Vizuális Beállítások")]
    [SerializeField] private float lineWidth = 0.05f;


    private Rigidbody2D rb;
    private Camera mainCamera;
    private LineRenderer lr;

    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private bool isDragging = false;
    private int currentJumps;
    private bool isSticky = false; // Falon vagyunk-e éppen

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>(); // Lekérjük a Line Renderert

        mainCamera = Camera.main; // Kamera gyorsítótárazása (cache-elés a teljesítményért)

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.enabled = false; // A játék elején elrejtjük a vonalat
        currentJumps = maxJumps;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Ha elfogyott a lövés és nem vagyunk falon, nem engedünk lőni (lezuhan)
        if (currentJumps <= 0 && !isSticky) return;

        // 1. Kattintás / Érintés kezdete
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            lr.enabled = true; // Vonal bekapcsolása
        }

        // 2. Húzás (Célzás)
        if (Input.GetMouseButton(0) && isDragging)
        {
            dragEndPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            DrawTrajectory(); // Vonal frissítése folyamatosan
        }

        // 3. Elengedés (Kilövés)
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            lr.enabled = false; // Vonal eltüntetése kilövéskor
            LaunchPlayer();
        }
    }

    private void DrawTrajectory()
    {
        // Kiszámoljuk a lövés irányát
        Vector2 launchDirection = dragStartPos - dragEndPos;

        // Kiszámoljuk az erőt, de itt is limitáljuk, hogy a vonal ne nőjön a végtelenségig
        float forceMagnitude = Mathf.Clamp(launchDirection.magnitude * launchForceMultiplier, 0f, maxForce);

        // A vonal kezdőpontja a karakter közepe
        Vector3 startPoint = transform.position;

        // A végpont a kiszámolt irány, megszorozva a limitált erővel (visszaosztva a szorzóval, hogy a vonal arányos maradjon)
        Vector3 endPoint = startPoint + (Vector3)(launchDirection.normalized * (forceMagnitude / launchForceMultiplier));

        // Beállítjuk a vonal két végpontját (0. pont és 1. pont)
        lr.SetPosition(0, startPoint);
        lr.SetPosition(1, endPoint);
    }

    private void LaunchPlayer()
    {
        // Kiszámoljuk a húzás irányát és távolságát (Vektor kivonás)
        // Mivel csúzli, az ellentétes irányba kell lőnünk, mint amerre húztuk
        Vector2 launchDirection = dragStartPos - dragEndPos;

        // Az erő nagysága a vektor hossza szorozva a szorzónkkal
        float forceMagnitude = Mathf.Clamp(launchDirection.magnitude * launchForceMultiplier, 0f, maxForce);

        // Ha a játékos falon állt, elengedjük a fixálást
        DetachFromWall();

        // Fizikai erő alkalmazása (Impulzus szerűen, azonnali sebességváltozás)
        rb.AddForce(launchDirection.normalized * forceMagnitude, ForceMode2D.Impulse);

        // Csökkentjük a lövések számát
        currentJumps--;
        Debug.Log($"Lövések száma: {currentJumps}");
    }



    // 1. Szilárd testeknek ütközés (Tapadós platform)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Csak akkor tapadunk oda, ha "Sticky" a címkéje
        if (collision.gameObject.CompareTag("Sticky"))
        {
            AttachToWall();
        }
        // Ha nem Sticky (pl. zöld csúszós fal), akkor a Unity fizikája 
        // magától lepattint róla minket, a kódnak nincs dolga vele!
    }

    // 2. "Szellem" testekbe lépés (Jégmező amin átmegyünk)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            // Amikor belépünk a jégbe, ráadjuk a hatalmas légellenállást
            rb.linearDamping = iceDamping;
        }
    }

    // 3. Kilépés a "Szellem" testekből
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            // Ha kijöttünk a jégből, visszaállítjuk a normál közeget
            rb.linearDamping = 0f;
        }
    }

    private void AttachToWall()
    {
        isSticky = true;
        rb.linearVelocity = Vector2.zero; // Megállítja a mozgást
        rb.gravityScale = 0f;             // Kikapcsolja a zuhanást
        currentJumps = maxJumps;          // Újratölti a lövéseket
        Debug.Log("Falon vagyunk, lövések újratöltve!");
    }

    private void DetachFromWall()
    {
        isSticky = false;
        rb.gravityScale = 1f; // Visszakapcsoljuk a normál gravitációt
    }
}