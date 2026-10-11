using UnityEngine;

public class ZonaEvento : MonoBehaviour
{
    [SerializeField] TipoEvento tipo;
    [SerializeField] string id;
    bool usada;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (usada) return;
        if (other.GetComponentInParent<PlayerController>() == null) return;

        usada = true;
        GameManager.Instance.EncolarEvento(tipo, id);
    }
}