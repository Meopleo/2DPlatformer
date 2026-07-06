using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Mozgás beállítások")]
    public float speed = 2f;
    public float distance = 1.5f;

    private float startX;
    private Rigidbody2D rb;

    void Start()
    {
        startX = transform.position.x;
        rb = GetComponent<Rigidbody2D>();

        // JAVÍTÁS: A helyes Unity kulcsszó a RigidbodyType2D!
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;
    }

    void FixedUpdate()
    {
        float newX = startX + Mathf.Sin(Time.time * speed) * distance;
        Vector2 targetPosition = new Vector2(newX, transform.position.y);

        rb.MovePosition(targetPosition);
    }
}