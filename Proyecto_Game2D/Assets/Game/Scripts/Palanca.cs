using UnityEngine;

public class Palanca : MonoBehaviour, IInteractable
{
    [SerializeField] string idPlataforma; 
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Color colorActivada = Color.green;
    [SerializeField] GameObject avisoInteractuar; 
    bool usada;

    void Start()
    {
        if (avisoInteractuar != null) avisoInteractuar.SetActive(false);
    }

    public void Interactuar()
    {
        if (usada) return;
        usada = true;

        GameManager.Instance.EncolarEvento(TipoEvento.ActivarPlataforma, idPlataforma);
        GameManager.Instance.EncolarEvento(TipoEvento.MostrarMensaje, "Plataforma activada");

        if (sprite != null) sprite.color = colorActivada;
        if (avisoInteractuar != null) avisoInteractuar.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (usada) return;
        if (other.GetComponentInParent<PlayerController>() != null && avisoInteractuar != null)
            avisoInteractuar.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null && avisoInteractuar != null)
            avisoInteractuar.SetActive(false);
    }
}