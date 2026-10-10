using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;

    private Transform destino;

    void Start()
    {
        destino = puntoB;
    }

    void Update()
    {
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
}
    // Update is called once per frame

