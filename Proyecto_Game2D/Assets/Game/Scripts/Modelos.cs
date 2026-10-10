using System;
using System.Collections.Generic;

[Serializable]
public class ConfigJuego
{
    public JugadorConfig jugador;
    public List<RequisitoConfig> requisitoJefe;
    public List<RecursoConfig> recursos;
    public List<PeligroConfig> peligros;
    public JefeConfig jefe;

    public RecursoConfig BuscarRecurso(string id)
    {
        return recursos.Find(r => r.id == id);
    }

    public PeligroConfig BuscarPeligro(string id)
    {
        return peligros.Find(p => p.id == id);
    }
}

[Serializable]
public class JugadorConfig
{
    public string nombre;
    public int vidas;
    public float velocidad;
    public float fuerzaSalto;
    public float invulnerabilidad;
}

[Serializable]
public class RequisitoConfig
{
    public string tipo;
    public int cantidad;
}

[Serializable]
public class RecursoConfig
{
    public string id;
    public string tipo;
    public int puntos;
    public string efecto;
    public float valor;
    public float duracion;
}

[Serializable]
public class PeligroConfig
{
    public string id;
    public string tipo;
    public int dano;
    public float velocidad;
}

[Serializable]
public class JefeConfig
{
    public int vida;
    public List<int> umbralesFase;
    public List<float> velocidadPorFase;
    public List<int> danoPorFase;
    public int puntosVictoria;
}

[Serializable]
public class ResumenPartida
{
    public string jugador;
    public string resultado;
    public int puntajeTotal;
    public float tiempoTotal;
    public List<EscenaResumen> escenas = new List<EscenaResumen>();
    public List<RecursoCantidad> recursos = new List<RecursoCantidad>();
    public int totalObjetos;
    public int checkpoints;
    public int golpesRecibidos;
    public MuertesResumen muertes = new MuertesResumen();
}

[Serializable]
public class EscenaResumen
{
    public string nombre;
    public float tiempo;
    public int puntaje;
    public int objetos;
    public int golpes;
    public int muertes;
}

[Serializable]
public class RecursoCantidad
{
    public string tipo;
    public int cantidad;
}

[Serializable]
public class MuertesResumen
{
    public int total;
    public int caida;
    public int enemigo;
    public int obstaculo;
    public int jefe;
}
