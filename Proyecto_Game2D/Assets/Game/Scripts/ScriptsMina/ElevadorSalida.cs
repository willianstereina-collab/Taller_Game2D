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
        if (!other.CompareTag("Player"))
            return;

        MinaController mina = FindObjectOfType<MinaController>();

        if (mina == null || GameManager.Instance == null)
            return;

        if (mina.CumpleRequisitosElevador())
        {
            GameManager.Instance.IniciarEscena(escenaSiguiente);
        }
        else
        {
            Debug.Log("Todavia faltan recursos para usar el elevador.");
        }
    }
}
    // Update is called once per frame
