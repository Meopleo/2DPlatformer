using UnityEngine;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    [Header("UI és Menü")]
    public GameObject menuUI;
    public GameObject inGameUI;
    public Text scoreText;
    public Text coinText;

    // --- ÚJ: A REKORD KIJELZŐ ÉS VÁLTOZÓ ---
    public Text highScoreText;
    private int highScore = 0;

    [Header("Menü Beállítások")]
    public float gameStartHeight = 5.5f;
    public float fixedFloorY = 5f;

    private bool isGameStarted = false;

    [Header("Sablonok (Prefabs)")]
    public GameObject safetyFloorPrefab;
    public GameObject[] platformPrefabs;
    public GameObject spikePrefab;
    public GameObject coinPrefab;

    [Header("Generálás Szabályai")]
    public Transform player;
    public float spawnDistance = 25f;
    public float platformSpacing = 3.5f;
    public float safeWidth = 2.2f;

    [Header("Extrák")]
    public float minScale = 0.6f;
    public float maxScale = 1.6f;
    public float maxRotation = 20f;
    [Range(0, 100)] public int doublePlatformChance = 25;
    [Range(0, 100)] public int coinSpawnChance = 30;

    private float lastGeneratedY = 0f;
    private int zigZagDirection = 1;
    private float maxScore = 0f;
    private int totalCoins = 0;

    void Start()
    {
        if (player == null) return;
        maxScore = player.position.y;

        isGameStarted = false;
        if (inGameUI != null) inGameUI.SetActive(false);

        UpdateCoinUI();

        // --- ÚJ: REKORD BETÖLTÉSE INDULÁSKOR ---
        // Kiolvassá a "HighScore" nevű mentést. Ha még sose játszottál, 0-t ad vissza.
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreUI();
        // ----------------------------------------

        lastGeneratedY = fixedFloorY + 2f;

        while (lastGeneratedY < fixedFloorY + spawnDistance)
        {
            SpawnNextRow();
        }
    }

    void Update()
    {
        if (player == null) return;

        if (!isGameStarted)
        {
            if (player.position.y > gameStartHeight)
            {
                StartGame();
            }
        }

        if (isGameStarted && player.position.y > maxScore)
        {
            maxScore = player.position.y;
        }

        if (scoreText != null && isGameStarted)
        {
            float currentMeters = Mathf.Max(0, maxScore - fixedFloorY);
            int finalScore = Mathf.FloorToInt(currentMeters);
            scoreText.text = finalScore.ToString();

            // --- ÚJ: REKORD ELLENŐRZÉSE JÁTÉK KÖZBEN ---
            // Ha a jelenlegi pontszámod nagyobb, mint az eddigi rekordod...
            if (finalScore > highScore)
            {
                highScore = finalScore; // Átírjuk a rekordot
                PlayerPrefs.SetInt("HighScore", highScore); // Elmentjük a telefon memóriájába
                UpdateHighScoreUI(); // Frissítjük a kijelzőt
            }
            // --------------------------------------------
        }

        if (player.position.y + spawnDistance > lastGeneratedY)
        {
            SpawnNextRow();
        }
    }

    // --- ÚJ: REKORD KIÍRÁSA A UI-RA ---
    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "HighScore: " + highScore.ToString();
        }
    }

    private void StartGame()
    {
        isGameStarted = true;
        if (inGameUI != null) inGameUI.SetActive(true);

        Vector3 floorPosition = new Vector3(0f, fixedFloorY, 0f);

        GameObject prefabToUse = safetyFloorPrefab != null ? safetyFloorPrefab : platformPrefabs[0];
        GameObject safetyFloor = Instantiate(prefabToUse, floorPosition, Quaternion.identity);

        safetyFloor.tag = "Sticky";

        if (safetyFloor.GetComponent<MovingPlatform>() != null)
        {
            safetyFloor.GetComponent<MovingPlatform>().enabled = false;
        }

        safetyFloor.transform.localScale = new Vector3(25f, 0.1f, 1f);
        safetyFloor.transform.rotation = Quaternion.identity;

        BoxCollider2D bc = safetyFloor.GetComponent<BoxCollider2D>();
        if (bc == null) bc = safetyFloor.AddComponent<BoxCollider2D>();
        bc.isTrigger = false;

        Collider2D[] colliders = safetyFloor.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders) { col.enabled = false; col.enabled = true; }
        Physics2D.SyncTransforms();

        maxScore = player.position.y;
    }

    private void SpawnNextRow()
    {
        lastGeneratedY += platformSpacing;
        SpawnObject(zigZagDirection);
        if (Random.Range(0, 100) < doublePlatformChance) SpawnObject(-zigZagDirection);
        zigZagDirection *= -1;
    }

    private void SpawnObject(int sideDirection)
    {
        GameObject prefabToSpawn;
        if (Random.Range(0, 100) < 20 && spikePrefab != null) prefabToSpawn = spikePrefab;
        else if (platformPrefabs.Length > 0) prefabToSpawn = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
        else return;

        float randomX = Random.Range(0.2f, safeWidth) * sideDirection;
        Vector3 spawnPosition = new Vector3(randomX, lastGeneratedY, 0);
        GameObject newObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        newObj.transform.localScale = new Vector3(newObj.transform.localScale.x * Random.Range(minScale, maxScale), newObj.transform.localScale.y * Random.Range(minScale, maxScale), newObj.transform.localScale.z);
        newObj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-maxRotation, maxRotation));

        Collider2D[] colliders = newObj.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders) { col.enabled = false; col.enabled = true; }
        Physics2D.SyncTransforms();

        if (coinPrefab != null && Random.Range(0, 100) < coinSpawnChance)
        {
            Instantiate(coinPrefab, new Vector3(spawnPosition.x, spawnPosition.y + 1.5f, 0), Quaternion.identity);
        }
    }

    public void AddCoin(int amount) { totalCoins += amount; UpdateCoinUI(); }
    private void UpdateCoinUI() { if (coinText != null) coinText.text = totalCoins.ToString(); }
}