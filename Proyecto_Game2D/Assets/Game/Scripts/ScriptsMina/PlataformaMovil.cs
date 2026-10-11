using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlataformaMovil : MonoBehaviour
{
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;

    private Transform destino;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        destino = puntoB;
    }

    void FixedUpdate()
    {
        Vector2 nuevaPosicion = Vector2.MoveTowards(
            rb.position,
            destino.position,
            velocidad * Time.fixedDeltaTime
        );

        rb.MovePosition(nuevaPosicion);

        if (Vector2.Distance(rb.position, destino.position) < 0.05f)
        {
            destino = destino == puntoA ? puntoB : puntoA;
        }
    }
}