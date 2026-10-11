using UnityEngine;

public class Recolectable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public string id;
    public PlataformaBateria plataforma;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        switch (id)
        {
            case "bateria":
                if (plataforma != null)
                {
                    plataforma.ActivarPlataforma();
                }
                break;

            case "hierro":
            case "cobre":
                EfectosJugador efectos = other.GetComponent<EfectosJugador>();

                if (efectos != null)
                {
                    efectos.AplicarEfecto(id);
                }
                break;

            case "fragmento":
                break;

            default:
                return;
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
