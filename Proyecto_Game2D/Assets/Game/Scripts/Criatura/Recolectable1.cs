using UnityEngine;

public class Recolectable1 : MonoBehaviour
{
    [SerializeField] string idRecurso = "hierro";  
    bool recogido;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (recogido) return;
        PlayerController jugador = other.GetComponentInParent<PlayerController>();
        if (jugador == null) return;

        RecursoConfig recurso = GameManager.Instance.RecolectarRecurso(idRecurso);
        if (recurso == null)
        {
            Debug.LogError("Recolectable: no existe '" + idRecurso + "' en config.json");
            return;
        }

        recogido = true;
        jugador.AplicarEfecto(recurso);
        Destroy(gameObject);
    }
}