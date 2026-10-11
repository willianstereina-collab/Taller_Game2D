using UnityEngine;

public class Peligro1 : MonoBehaviour
{
    [SerializeField] string idPeligro = "pinchos";   
    [SerializeField] float distanciaPatrulla = 3f;   
    [SerializeField] SpriteRenderer sprite;

    PeligroConfig config;
    float centroX;
    int direccion = 1;

    void Start()
    {
        config = GameManager.Instance.Config.BuscarPeligro(idPeligro);
        if (config == null)
        {
            Debug.LogError("Peligro: no existe '" + idPeligro + "' en config.json");
            enabled = false;
            return;
        }
        centroX = transform.position.x;
    }

    void Update()
    {
        if (config.velocidad <= 0f) return;   
        Vector3 pos = transform.position;
        pos.x += direccion * config.velocidad * Time.deltaTime;

        if (pos.x > centroX + distanciaPatrulla) { pos.x = centroX + distanciaPatrulla; direccion = -1; }
        else if (pos.x < centroX - distanciaPatrulla) { pos.x = centroX - distanciaPatrulla; direccion = 1; }

        transform.position = pos;
        if (sprite != null) sprite.flipX = direccion < 0;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (config == null) return;
        PlayerController jugador = other.GetComponentInParent<PlayerController>();
        if (jugador != null) jugador.RecibirGolpe(config.tipo, config.dano);
    }
}