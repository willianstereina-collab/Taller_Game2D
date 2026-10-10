using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    bool activado;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activado) return;
        if (!other.CompareTag("Player")) return;

        activado = true;
        GameManager.Instance.RegistrarCheckpoint(transform.position);
    }
}
