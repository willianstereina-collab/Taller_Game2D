using UnityEngine;

public class ZonaCaida : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out PlayerController jugador))
            jugador.CaerAlVacio();
    }
}