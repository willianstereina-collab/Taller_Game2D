using System;

[Serializable]
public class RegistroRecoleccion
{
    public string id;
    public string tipo;
    public string escena;
    public float momento;
}

[Serializable]
public class RegistroIncidente
{
    public string tipo;
    public string causa;
    public string escena;
    public float tiempo;
}

public enum TipoEvento
{
    ActivarPlataforma,
    AparecerEnemigos,
    MostrarMensaje,
    IniciarCombate,
    CambioFase
}

public class EventoJuego
{
    public TipoEvento tipo;
    public string id;
}
