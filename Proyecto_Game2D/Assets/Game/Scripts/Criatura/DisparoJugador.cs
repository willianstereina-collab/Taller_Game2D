using UnityEngine;

public class DisparoJugador : MonoBehaviour
{
    [SerializeField] Proyectil prefabProyectil;
    [SerializeField] KeyCode teclaDisparo = KeyCode.F;
    [SerializeField] float cadencia = 0.4f;
    [SerializeField] float velocidadProyectil = 12f;
    [SerializeField] float vidaProyectil = 2f;
    [SerializeField] int danoProyectil = 5;
    [SerializeField] float separacion = 0.8f;

    PlayerController jugador;
    bool habilitado;
    float proximoDisparo;

    void Start()
    {
        jugador = FindFirstObjectByType<PlayerController>();
    }

    public void Habilitar(bool valor)
    {
        habilitado = valor;
    }

    void Update()
    {
        if (!habilitado || jugador == null || prefabProyectil == null) return;

        if (Input.GetKeyDown(teclaDisparo) && Time.time >= proximoDisparo)
        {
            proximoDisparo = Time.time + cadencia;
            Disparar();
        }
    }

    void Disparar()
    {
        Vector2 dir = (jugador.sprite != null && jugador.sprite.flipX) ? Vector2.left : Vector2.right;
        Vector3 pos = jugador.transform.position + (Vector3)(dir * separacion);
        Proyectil p = Instantiate(prefabProyectil, pos, Quaternion.identity);
        p.Configurar(dir, velocidadProyectil, danoProyectil, vidaProyectil);
    }
}