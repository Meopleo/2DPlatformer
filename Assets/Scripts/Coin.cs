using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Érme beállítása")]
    public int coinValue = 1; // Mennyit ér egy érme?

    // Ez a metódus fut le, amikor valami átrepül a Trigger-en
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Csak akkor csinálunk bármit, ha az a valami a Játékos (Player)
        if (collision.CompareTag("Player"))
        {
            // Megkeressük a pályán a LevelGeneratort (Aki a pénzt számolja)
            LevelGenerator generator = FindAnyObjectByType<LevelGenerator>();

            if (generator != null)
            {
                generator.AddCoin(coinValue); // Szólunk neki, hogy adjon hozzá pénzt
            }

            // Pénz felvéve -> Azonnal megsemmisítjük az érmét a pályáról!
            Destroy(gameObject);
        }
    }
}