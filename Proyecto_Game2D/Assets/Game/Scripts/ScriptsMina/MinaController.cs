using TMPro;
using UnityEngine;

public class MinaController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public TMP_Text textoVidas;
    public TMP_Text textoTiempo;
    public TMP_Text textoRecursos;

    private float tiempoTranscurrido = 0f;
    private Vector3 ultimoCheckpoint;
    private bool tieneCheckpoint = false;

    void Update()
    {
        if (GameManager.Instance == null) return;

        tiempoTranscurrido += Time.deltaTime;

        // Mostrar vidas
        if (textoVidas != null)
        {
            textoVidas.text =
                "Vidas: " + GameManager.Instance.Vidas;
        }

        // Mostrar tiempo
        if (textoTiempo != null)
        {
            int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60);
            int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60);

            textoTiempo.text = "Tiempo: " +
                minutos.ToString("00") + ":" +
                segundos.ToString("00");
        }

        // Mostrar recursos
        if (textoRecursos != null && GameManager.Instance != null)
        {
            int minerales = GameManager.Instance.CantidadDe("mineral");
            int baterias = GameManager.Instance.CantidadDe("bateria");
            int fragmentos = GameManager.Instance.CantidadDe("mapa");

            textoRecursos.text =
                "Recursos para el elevador:\n" +
                "Minerales: " + minerales + "/6\n" +
                "Baterias: " + baterias + "/2\n" +
                "Fragmentos de mapa: " + fragmentos + "/4";
        }
    }

    public void RecibirDano(int dano)
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        bool murio = GameManager.Instance.RegistrarGolpe("caida", dano);

        if (murio)
        {
            ReaparecerEnCheckpoint();
        }
    }

    public void ReaparecerEnCheckpoint()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.TieneCheckpoint(out Vector3 posicion))
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");

            if (jugador != null)
            {
                jugador.transform.position = posicion;

                Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
    }

    public bool CumpleRequisitosElevador()
    {
        if (GameManager.Instance == null)
            return false;

        return GameManager.Instance.CantidadDe("mineral") >= 6 &&
               GameManager.Instance.CantidadDe("bateria") >= 2 &&
               GameManager.Instance.CantidadDe("mapa") >= 4;
    }

    public void ActivarCheckpoint(Vector3 posicion)
    {
        ultimoCheckpoint = posicion;
        tieneCheckpoint = true;
    }

    public void ReaparecerEnCheckpoint(GameObject jugador)
    {
        if (jugador == null || !tieneCheckpoint)
        {
            return;
        }

        jugador.transform.position = ultimoCheckpoint;
    }
}
