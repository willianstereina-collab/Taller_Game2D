using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CriaturaController : MonoBehaviour
{
    [Serializable]
    public class GrupoObjetos
    {
        public string id;
        public GameObject[] objetos;
    }

    [Header("HUD")]
    [SerializeField] TMP_Text textoVidas;
    [SerializeField] TMP_Text textoPuntaje;
    [SerializeField] TMP_Text textoTiempo;
    [SerializeField] TMP_Text textoRecursos;
    [SerializeField] TMP_Text textoMensaje;
    [SerializeField] TMP_Text textoAyuda;

    [Header("Jefe")]
    [SerializeField] Boss boss;
    [SerializeField] DisparoJugador disparo;
    [SerializeField] Slider sliderJefe;
    [SerializeField] TMP_Text textoFase;

    [Header("Final")]
    [SerializeField] EstadisticasController estadisticas;

    [Header("Eventos")]
    [SerializeField] List<GrupoObjetos> gruposEnemigos;

    public const string NombreEscena = "Criatura";

    readonly Dictionary<string, PlataformaMovil1> plataformas = new Dictionary<string, PlataformaMovil1>();
    GameManager gm;
    float ocultarMensajeEn;
    bool combateActivo;

    void Start()
    {
        gm = GameManager.Instance;
        gm.IniciarEscena(NombreEscena); 

        foreach (PlataformaMovil1 p in FindObjectsByType<PlataformaMovil1>(FindObjectsSortMode.None))
            plataformas[p.Id] = p;

        foreach (GrupoObjetos g in gruposEnemigos)
            foreach (GameObject o in g.objetos) o.SetActive(false);

        if (sliderJefe != null) sliderJefe.gameObject.SetActive(false);
        if (textoFase != null) textoFase.gameObject.SetActive(false);
        Poner(textoMensaje, "");
        Poner(textoAyuda, "");

        if (boss != null) boss.AlMorir += Victoria;
        gm.AlProcesarEvento += ProcesarEvento;
    }

    void OnDestroy()
    {
        if (gm != null) gm.AlProcesarEvento -= ProcesarEvento;
        if (boss != null) boss.AlMorir -= Victoria;
    }

    void Update()
    {
        if (gm == null) return;

        Poner(textoVidas, "Vidas: " + gm.Vidas);
        Poner(textoPuntaje, "Puntaje: " + gm.PuntajeTotal);

        float t = gm.TiempoEscena(NombreEscena);
        Poner(textoTiempo, "Tiempo: " + Mathf.FloorToInt(t / 60).ToString("00") + ":" + Mathf.FloorToInt(t % 60).ToString("00"));

        string recursos = "";
        foreach (RequisitoConfig req in gm.Config.requisitoJefe)
            recursos += req.tipo + ": " + gm.TextoRequisito(req.tipo) + "\n";
        Poner(textoRecursos, recursos);

        if (combateActivo && boss != null)
        {
            if (sliderJefe != null) sliderJefe.value = boss.VidaNormalizada;
            Poner(textoFase, "Fase " + (boss.Fase + 1));
        }

        if (ocultarMensajeEn > 0f && Time.time > ocultarMensajeEn)
        {
            Poner(textoMensaje, "");
            ocultarMensajeEn = 0f;
        }
    }

    void ProcesarEvento(EventoJuego evento)
    {
        switch (evento.tipo)
        {
            case TipoEvento.AparecerEnemigos:
                foreach (GrupoObjetos g in gruposEnemigos)
                    if (g.id == evento.id)
                        foreach (GameObject o in g.objetos) o.SetActive(true);
                Mostrar("¡Cuidado, murciélagos!");
                break;

            case TipoEvento.ActivarPlataforma:
                if (plataformas.TryGetValue(evento.id, out PlataformaMovil1 p)) p.Activar();
                break;

            case TipoEvento.MostrarMensaje:
                Mostrar(evento.id);
                break;

            case TipoEvento.IniciarCombate:
                if (combateActivo || boss == null) break;
                combateActivo = true;
                if (sliderJefe != null) { sliderJefe.gameObject.SetActive(true); sliderJefe.value = 1f; }
                if (textoFase != null) textoFase.gameObject.SetActive(true);
                if (disparo != null) disparo.Habilitar(true);
                boss.Iniciar();
                Poner(textoAyuda, "F: disparar");
                Mostrar("¡El jefe despierta! Presiona F para disparar", 5f);
                break;

            case TipoEvento.CambioFase:
                if (boss != null && int.TryParse(evento.id, out int fase) && boss.FaseSolicitada == fase)
                {
                    boss.AplicarFase(fase);
                    Mostrar("¡El jefe entra en la fase " + (fase + 1) + "!");
                }
                break;
        }
    }

    void Victoria()
    {
        if (!combateActivo) return;
        combateActivo = false;

        gm.DerrotarJefe();             
        if (disparo != null) disparo.Habilitar(false);
        if (sliderJefe != null) sliderJefe.gameObject.SetActive(false);
        if (textoFase != null) textoFase.gameObject.SetActive(false);
        Poner(textoAyuda, "");
        Poner(textoMensaje, "");

        if (estadisticas != null) estadisticas.Mostrar("victoria"); 
        Time.timeScale = 0f;
    }

    void Mostrar(string mensaje, float segundos = 3f)
    {
        Poner(textoMensaje, mensaje);
        ocultarMensajeEn = Time.time + segundos;
    }

    static void Poner(TMP_Text texto, string valor)
    {
        if (texto != null) texto.text = valor;
    }
}