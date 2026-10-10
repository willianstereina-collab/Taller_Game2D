using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ConfigJuego Config { get; private set; }
    public string ErrorConfig { get; private set; }
    public int Vidas { get; private set; }
    public int PuntajeTotal { get; private set; }
    public int Checkpoints { get; private set; }
    public string RutaResumen { get; private set; }
    public string EscenaActual { get; private set; }

    public List<RegistroRecoleccion> Recolecciones { get; private set; } = new List<RegistroRecoleccion>();
    public List<RegistroIncidente> Incidentes { get; private set; } = new List<RegistroIncidente>();
    public Dictionary<string, int> Inventario { get; private set; } = new Dictionary<string, int>();
    public Stack<Vector3> PilaCheckpoints { get; private set; } = new Stack<Vector3>();
    public Queue<EventoJuego> ColaEventos { get; private set; } = new Queue<EventoJuego>();

    public event Action<EventoJuego> AlProcesarEvento;

    readonly Dictionary<string, EscenaResumen> escenas = new Dictionary<string, EscenaResumen>();
    readonly MuertesResumen muertes = new MuertesResumen();
    bool cronometroActivo;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CargarConfig();
    }

    void Update()
    {
        if (cronometroActivo && EscenaActual != null)
            escenas[EscenaActual].tiempo += Time.deltaTime;

        if (ColaEventos.Count > 0)
        {
            EventoJuego evento = ColaEventos.Dequeue();
            AlProcesarEvento?.Invoke(evento);
        }
    }

    public bool CargarConfig()
    {
        Config = JsonService.CargarConfig(out string error);
        ErrorConfig = error;
        if (error != null) Debug.LogError(error);
        return Config != null;
    }

    public bool IniciarPartida()
    {
        if (!CargarConfig()) return false;

        Vidas = Config.jugador.vidas;
        PuntajeTotal = 0;
        Checkpoints = 0;
        RutaResumen = null;
        EscenaActual = null;
        cronometroActivo = false;
        Recolecciones.Clear();
        Incidentes.Clear();
        Inventario.Clear();
        PilaCheckpoints.Clear();
        ColaEventos.Clear();
        escenas.Clear();
        muertes.total = muertes.caida = muertes.enemigo = muertes.obstaculo = muertes.jefe = 0;
        return true;
    }

    public void IniciarEscena(string nombre)
    {
        if (!escenas.ContainsKey(nombre))
            escenas[nombre] = new EscenaResumen { nombre = nombre };

        EscenaActual = nombre;
        cronometroActivo = true;
    }

    public void DetenerCronometro()
    {
        cronometroActivo = false;
    }

    public float TiempoEscena(string nombre)
    {
        return escenas.TryGetValue(nombre, out EscenaResumen e) ? e.tiempo : 0f;
    }

    public float TiempoTotal()
    {
        float total = 0f;
        foreach (EscenaResumen e in escenas.Values) total += e.tiempo;
        return total;
    }

    public void EncolarEvento(TipoEvento tipo, string id = "")
    {
        ColaEventos.Enqueue(new EventoJuego { tipo = tipo, id = id });
    }

    public RecursoConfig RecolectarRecurso(string idRecurso)
    {
        RecursoConfig recurso = Config.BuscarRecurso(idRecurso);
        if (recurso == null) return null;

        Inventario[recurso.tipo] = CantidadDe(recurso.tipo) + 1;
        PuntajeTotal += recurso.puntos;

        Recolecciones.Add(new RegistroRecoleccion
        {
            id = recurso.id,
            tipo = recurso.tipo,
            escena = EscenaActual,
            momento = TiempoTotal()
        });

        EscenaResumen escena = escenas[EscenaActual];
        escena.objetos++;
        escena.puntaje += recurso.puntos;
        return recurso;
    }

    public void SumarPuntos(int puntos)
    {
        PuntajeTotal += puntos;
        escenas[EscenaActual].puntaje += puntos;
    }

    public void DerrotarJefe()
    {
        SumarPuntos(Config.jefe.puntosVictoria);
    }

    public int CantidadDe(string tipo)
    {
        return Inventario.TryGetValue(tipo, out int cantidad) ? cantidad : 0;
    }

    public bool CumpleRequisitoJefe()
    {
        foreach (RequisitoConfig req in Config.requisitoJefe)
            if (CantidadDe(req.tipo) < req.cantidad) return false;
        return true;
    }

    public List<string> RecursosFaltantes()
    {
        List<string> faltantes = new List<string>();
        foreach (RequisitoConfig req in Config.requisitoJefe)
        {
            int actual = CantidadDe(req.tipo);
            if (actual < req.cantidad)
                faltantes.Add(req.tipo + " " + actual + "/" + req.cantidad);
        }
        return faltantes;
    }

    public string TextoRequisito(string tipo)
    {
        foreach (RequisitoConfig req in Config.requisitoJefe)
            if (req.tipo == tipo) return CantidadDe(tipo) + "/" + req.cantidad;
        return CantidadDe(tipo).ToString();
    }

    public bool RegistrarGolpe(string causa, int dano)
    {
        Vidas = Mathf.Max(0, Vidas - dano);
        escenas[EscenaActual].golpes++;
        Incidentes.Add(new RegistroIncidente
        {
            tipo = "golpe",
            causa = causa,
            escena = EscenaActual,
            tiempo = TiempoTotal()
        });

        if (Vidas > 0) return false;

        RegistrarMuerte(causa);
        return true;
    }

    public void RegistrarMuerte(string causa)
    {
        muertes.total++;
        escenas[EscenaActual].muertes++;

        switch (causa)
        {
            case "caida": muertes.caida++; break;
            case "enemigo": muertes.enemigo++; break;
            case "obstaculo": muertes.obstaculo++; break;
            case "jefe": muertes.jefe++; break;
        }

        Incidentes.Add(new RegistroIncidente
        {
            tipo = "muerte",
            causa = causa,
            escena = EscenaActual,
            tiempo = TiempoTotal()
        });

        Vidas = Config.jugador.vidas;
    }

    public void RegistrarCheckpoint(Vector3 posicion)
    {
        PilaCheckpoints.Push(posicion);
        Checkpoints++;
    }

    public bool TieneCheckpoint(out Vector3 posicion)
    {
        if (PilaCheckpoints.Count > 0)
        {
            posicion = PilaCheckpoints.Peek();
            return true;
        }

        posicion = Vector3.zero;
        return false;
    }

    public ResumenPartida GenerarResumen(string resultado)
    {
        ResumenPartida resumen = new ResumenPartida
        {
            jugador = Config.jugador.nombre,
            resultado = resultado,
            puntajeTotal = PuntajeTotal,
            tiempoTotal = TiempoTotal(),
            checkpoints = Checkpoints
        };

        foreach (EscenaResumen e in escenas.Values)
        {
            resumen.escenas.Add(e);
            resumen.totalObjetos += e.objetos;
            resumen.golpesRecibidos += e.golpes;
        }

        foreach (KeyValuePair<string, int> par in Inventario)
            resumen.recursos.Add(new RecursoCantidad { tipo = par.Key, cantidad = par.Value });

        resumen.muertes = muertes;
        return resumen;
    }

    public ResumenPartida FinalizarPartida(string resultado)
    {
        DetenerCronometro();
        ResumenPartida resumen = GenerarResumen(resultado);
        RutaResumen = JsonService.GuardarResumen(resumen);
        return resumen;
    }

    public EscenaResumen DatosEscena(string nombre)
    {
        return escenas.TryGetValue(nombre, out EscenaResumen e) ? e : new EscenaResumen { nombre = nombre };
    }
}