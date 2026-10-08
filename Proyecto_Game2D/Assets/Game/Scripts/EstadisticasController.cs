using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EstadisticasController : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text textoJugador;
    [SerializeField] TMP_Text textoTabla;
    [SerializeField] TMP_Text textoRecursos;
    [SerializeField] TMP_Text textoMuertes;
    [SerializeField] TMP_Text textoCheckpoints;
    [SerializeField] TMP_Text textoRuta;
    [SerializeField] Button botonMenu;
    [SerializeField] string escenaMenu = "Menu";
    [SerializeField] string escenaMina = "Mina";
    [SerializeField] string escenaCriatura = "Criatura";

    void Awake()
    {
        panel.SetActive(false);
        botonMenu.onClick.AddListener(VolverAlMenu);
    }

    public void Mostrar(string resultado)
    {
        GameManager gm = GameManager.Instance;
        ResumenPartida resumen = gm.FinalizarPartida(resultado);
        EscenaResumen mina = gm.DatosEscena(escenaMina);
        EscenaResumen criatura = gm.DatosEscena(escenaCriatura);

        textoJugador.text = resumen.jugador + " - " + resumen.resultado;

        textoTabla.text =
            Fila("Dato", "Mina", "Criatura", "Total") + "\n" +
            Fila("Tiempo", Segundos(mina.tiempo), Segundos(criatura.tiempo), Segundos(resumen.tiempoTotal)) + "\n" +
            Fila("Puntaje", mina.puntaje.ToString(), criatura.puntaje.ToString(), resumen.puntajeTotal.ToString()) + "\n" +
            Fila("Objetos", mina.objetos.ToString(), criatura.objetos.ToString(), resumen.totalObjetos.ToString()) + "\n" +
            Fila("Golpes", mina.golpes.ToString(), criatura.golpes.ToString(), resumen.golpesRecibidos.ToString()) + "\n" +
            Fila("Muertes", mina.muertes.ToString(), criatura.muertes.ToString(), resumen.muertes.total.ToString());

        string recursos = "Recursos recolectados\n";
        foreach (RecursoCantidad r in resumen.recursos)
            recursos += r.tipo + ": " + r.cantidad + "\n";
        textoRecursos.text = recursos;

        textoMuertes.text =
            "Muertes por causa\n" +
            "Caida: " + resumen.muertes.caida + "\n" +
            "Enemigo: " + resumen.muertes.enemigo + "\n" +
            "Obstaculo: " + resumen.muertes.obstaculo + "\n" +
            "Jefe: " + resumen.muertes.jefe;

        textoCheckpoints.text = "Checkpoints activados: " + resumen.checkpoints;
        textoRuta.text = "Resumen guardado en: " + (gm.RutaResumen ?? "no se pudo guardar");

        panel.SetActive(true);
    }

    void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenu);
    }

    static string Fila(string dato, string a, string b, string c)
    {
        return dato + "<pos=35%>" + a + "<pos=55%>" + b + "<pos=78%>" + c;
    }

    static string Segundos(float valor)
    {
        return valor.ToString("F1") + " s";
    }
}
