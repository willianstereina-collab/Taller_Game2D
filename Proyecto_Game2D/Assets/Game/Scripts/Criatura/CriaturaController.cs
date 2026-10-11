using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("Eventos")]
    [SerializeField] List<GrupoObjetos> gruposEnemigos;

    public const string NombreEscena = "Criatura";

    readonly Dictionary<string, PlataformaMovil1> plataformas = new Dictionary<string, PlataformaMovil1>();
    GameManager gm;
    float ocultarMensajeEn;

    void Start()
    {
        gm = GameManager.Instance;
        gm.IniciarEscena(NombreEscena);

        foreach (PlataformaMovil1 p in FindObjectsByType<PlataformaMovil1>(FindObjectsSortMode.None))
            plataformas[p.Id] = p;

        foreach (GrupoObjetos g in gruposEnemigos)
            foreach (GameObject o in g.objetos) o.SetActive(false);

        Poner(textoMensaje, "");
        gm.AlProcesarEvento += ProcesarEvento;
    }

    void OnDestroy()
    {
        if (gm != null) gm.AlProcesarEvento -= ProcesarEvento;
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
                Mostrar("¡El jefe despierta!");
                break;

            case TipoEvento.CambioFase:
                break;
        }
    }

    void Mostrar(string mensaje)
    {
        Poner(textoMensaje, mensaje);
        ocultarMensajeEn = Time.time + 3f;
    }

    static void Poner(TMP_Text texto, string valor)
    {
        if (texto != null) texto.text = valor;
    }
}