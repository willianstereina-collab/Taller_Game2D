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
        if (textoRecursos != null &&
            GameManager.Instance.Config != null &&
            GameManager.Instance.Config.requisitoJefe != null)
        {
            string mensaje = "Recursos para el elevador:\n";

            foreach (RequisitoConfig requisito in
                GameManager.Instance.Config.requisitoJefe)
            {
                int actual =
                    GameManager.Instance.CantidadDe(requisito.tipo);

                mensaje += requisito.tipo + ": " +
                    actual + "/" + requisito.cantidad + "\n";
            }

            textoRecursos.text = mensaje;
        }
    }

    public void RecibirDano(int dano)
    {
        // CONECTAR DEL DAÑO DEL PJ
    }
}
