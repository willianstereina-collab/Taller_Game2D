using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] TMP_Text textoSaludo;
    [SerializeField] TMP_Text textoError;
    [SerializeField] Button botonJugar;
    [SerializeField] Button botonSalir;
    [SerializeField] string escenaMina = "Mina";

    void Start()
    {
        GameManager gm = GameManager.Instance;
        bool ok = gm != null && gm.Config != null;

        botonJugar.interactable = ok;
        textoError.gameObject.SetActive(!ok);

        if (ok)
            textoSaludo.text = "Hola, " + gm.Config.jugador.nombre;
        else
            textoError.text = gm != null ? gm.ErrorConfig : "GameManager no encontrado en la escena Menu";

        botonJugar.onClick.AddListener(Jugar);
        botonSalir.onClick.AddListener(Salir);
    }

    void Jugar()
    {
        if (GameManager.Instance.IniciarPartida())
            SceneManager.LoadScene(escenaMina);
        else
            textoError.text = GameManager.Instance.ErrorConfig;
    }

    void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
