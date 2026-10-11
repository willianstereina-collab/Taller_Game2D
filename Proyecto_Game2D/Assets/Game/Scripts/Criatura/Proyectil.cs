using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Proyectil : MonoBehaviour
{
    int dano;

    public void Configurar(Vector2 direccion, float velocidad, int danoImpacto, float vida)
    {
        dano = danoImpacto;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.linearVelocity = direccion * velocidad;
        Destroy(gameObject, vida);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Boss jefe = other.GetComponentInParent<Boss>();
        if (jefe != null)
        {
            jefe.RecibirDano(dano);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger && other.GetComponentInParent<PlayerController>() == null)
            Destroy(gameObject);
    }
}