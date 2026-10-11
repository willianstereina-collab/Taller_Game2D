using UnityEngine;

public class Peligro : MonoBehaviour
{
    public enum TipoPeligro
    {
        Enemigo,
        Obstaculo
    }

    public TipoPeligro tipo;
    public int dano = 1;
    public float velocidad = 2f;

    public Transform puntoA;
    public Transform puntoB;

    private Transform destino;

    void Start()
    {
        if (tipo == TipoPeligro.Enemigo && puntoA != null && puntoB != null)
        {
            destino = puntoB;
        }
    }

    void Update()
    {
        if (tipo != TipoPeligro.Enemigo || puntoA == null || puntoB == null)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            destino.position,
            velocidad * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, destino.position) < 0.05f)
        {
            destino = destino == puntoA ? puntoB : puntoA;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Golpear(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Golpear(other);
    }

    void Golpear(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out PlayerController jugador))
        {
            string causa = tipo == TipoPeligro.Enemigo ? "enemigo" : "obstaculo";
            jugador.RecibirGolpe(causa, dano);
        }
    }
}
