using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlataformaMovil1 : MonoBehaviour
{
    [SerializeField] string idPlataforma;
    [SerializeField] Vector2 desplazamiento = new Vector2(4f, 0f);
    [SerializeField] float velocidad = 2f;
    [SerializeField] bool activaAlInicio = true;

    Rigidbody2D rb;
    Vector2 inicio, fin;
    bool haciaFin = true;
    bool activa;
    readonly List<Rigidbody2D> pasajeros = new List<Rigidbody2D>();

    public string Id => idPlataforma;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        inicio = rb.position;
        fin = inicio + desplazamiento;
        activa = activaAlInicio;
    }

    public void Activar() { activa = true; }

    void FixedUpdate()
    {
        if (!activa) return;

        Vector2 destino = haciaFin ? fin : inicio;
        Vector2 nueva = Vector2.MoveTowards(rb.position, destino, velocidad * Time.fixedDeltaTime);
        Vector2 delta = nueva - rb.position;

        rb.MovePosition(nueva);
        foreach (Rigidbody2D p in pasajeros) p.position += delta;

        if (nueva == destino) haciaFin = !haciaFin;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.rigidbody != null && col.transform.position.y > transform.position.y)
            if (!pasajeros.Contains(col.rigidbody)) pasajeros.Add(col.rigidbody);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.rigidbody != null) pasajeros.Remove(col.rigidbody);
    }
}