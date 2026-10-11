using UnityEngine;
using System.Collections;

public class EfectosJugador : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float velocidadBase = 5f;
    public float fuerzaSaltoBase = 9f;

    private float velocidadActual;
    private float fuerzaSaltoActual;

    private Coroutine efectoVelocidad;
    private Coroutine efectoSalto;

    void Start()
    {
        velocidadActual = velocidadBase;
        fuerzaSaltoActual = fuerzaSaltoBase;
    }

    public void AplicarEfecto(string id)
    {
        if (id == "hierro")
        {
            if (efectoVelocidad != null)
            {
                StopCoroutine(efectoVelocidad);
            }

            efectoVelocidad = StartCoroutine(
                MejorarVelocidad(1.5f, 5f)
            );
        }
        else if (id == "cobre")
        {
            if (efectoSalto != null)
            {
                StopCoroutine(efectoSalto);
            }

            efectoSalto = StartCoroutine(
                MejorarSalto(1.3f, 5f)
            );
        }
    }

    IEnumerator MejorarVelocidad(float multiplicador, float duracion)
    {
        velocidadActual = velocidadBase * multiplicador;

        yield return new WaitForSeconds(duracion);

        velocidadActual = velocidadBase;
        efectoVelocidad = null;
    }

    IEnumerator MejorarSalto(float multiplicador, float duracion)
    {
        fuerzaSaltoActual = fuerzaSaltoBase * multiplicador;

        yield return new WaitForSeconds(duracion);

        fuerzaSaltoActual = fuerzaSaltoBase;
        efectoSalto = null;
    }

    public float ObtenerVelocidad()
    {
        return velocidadActual;
    }

    public float ObtenerFuerzaSalto()
    {
        return fuerzaSaltoActual;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
