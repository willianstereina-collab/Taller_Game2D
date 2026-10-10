using System;
using System.IO;
using UnityEngine;

public static class JsonService
{
    public const string NombreConfig = "config.json";
    public const string NombreResumen = "resumen_partida.json";

    public static ConfigJuego CargarConfig(out string error)
    {
        error = null;
        string ruta = Path.Combine(Application.streamingAssetsPath, NombreConfig);

        try
        {
            if (!File.Exists(ruta))
            {
                error = "No se encontro " + NombreConfig + " en " + ruta;
                return null;
            }

            string texto = File.ReadAllText(ruta);
            ConfigJuego config = JsonUtility.FromJson<ConfigJuego>(texto);

            if (config == null || config.jugador == null || config.jefe == null ||
                config.recursos == null || config.peligros == null || config.requisitoJefe == null)
            {
                error = NombreConfig + " esta incompleto o mal formado";
                return null;
            }

            return config;
        }
        catch (Exception e)
        {
            error = "Error leyendo " + NombreConfig + ": " + e.Message;
            return null;
        }
    }

    public static string GuardarResumen(ResumenPartida resumen)
    {
        string ruta = Path.Combine(Application.persistentDataPath, NombreResumen);

        try
        {
            File.WriteAllText(ruta, JsonUtility.ToJson(resumen, true));
            return ruta;
        }
        catch (Exception e)
        {
            Debug.LogError("Error guardando " + NombreResumen + ": " + e.Message);
            return null;
        }
    }
}
