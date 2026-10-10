using UnityEngine;

// SOLO PARA PRUEBAS. Se borra antes de entregar.
[DefaultExecutionOrder(-1000)]
public class CriaturaTestBootstrap : MonoBehaviour
{
    void Awake()
    {
        // Si venimos de la Mina, el GameManager real ya existe: no hacer nada.
        if (GameManager.Instance != null) return;

        GameObject go = new GameObject("GameManager (TEST)");
        GameManager gm = go.AddComponent<GameManager>(); // su Awake corre aquí mismo

        if (!gm.IniciarPartida())
        {
            Debug.LogError("Bootstrap: no se pudo cargar config.json");
            return;
        }

        gm.IniciarEscena("Criatura");
    }
}
