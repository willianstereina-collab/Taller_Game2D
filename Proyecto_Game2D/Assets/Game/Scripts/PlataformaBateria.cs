using UnityEngine;

public class PlataformaBateria : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;

    private Transform destino;
    private bool activada = false;

    void Start()
    {
        destino = puntoB;
    }

    void Update()
    {
        if (!activada)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            destino.position,
            velocidad * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, destino.position) < 0.05f)
        {
            destino = destino == puntoA ? puntoB : puntoA;
        }
    }

    public void ActivarPlataforma()
    {
        activada = true;
    }
}
