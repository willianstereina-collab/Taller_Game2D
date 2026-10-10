using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public string escenaSiguiente = "Escena2";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance == null) return;

        if (GameManager.Instance.CumpleRequisitoJefe())
        {
            GameManager.Instance.IniciarEscena(escenaSiguiente);
        }
        else
        {
            Debug.Log(
                "Todavia faltan recursos: " +

                GameManager.Instance.RecursosFaltantes()
            );
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
