using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Követési beállítások")]
    [SerializeField] private Transform target;       // A játékos (Player), akit követünk
    [SerializeField] private float smoothSpeed = 5f; // Milyen finoman kövesse a kamera

    private float maxPositionY; // A legmagasabb pont, amit a kamera elért

    private void Start()
    {
        // Elmentjük a kamera kezdő magasságát
        maxPositionY = transform.position.y;
    }

    private void LateUpdate() // A LateUpdate garantálja a rángatásmentes követést
    {
        // Biztonsági ellenőrzés: ha nincs bedobva a Player, ne csináljon semmit
        if (target == null) return;

        // Csak akkor akarjuk a kamerát mozgatni, ha a játékos feljebb van, mint az eddigi maximum
        if (target.position.y > maxPositionY)
        {
            maxPositionY = target.position.y;
        }

        // Kiszámoljuk a kamera új pozícióját (az X és a Z marad a régi, csak az Y változik)
        Vector3 targetPosition = new Vector3(transform.position.x, maxPositionY, transform.position.z);

        // A Lerp (Linear Interpolation) segítségével lágy, "csúszós" mozgást adunk a kamerának
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}