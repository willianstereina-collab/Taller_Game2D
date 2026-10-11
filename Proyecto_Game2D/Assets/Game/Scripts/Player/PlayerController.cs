using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public Transform origenSuelo;
    public float distanciaSuelo = 0.15f;
    public LayerMask capaSuelo;

    public Transform origenFrontal;
    public float distanciaFrontal = 0.6f;
    public LayerMask capaInteractuable;
    public KeyCode teclaInteractuar = KeyCode.E;

    public SpriteRenderer sprite;
    public Animator animator;

    Rigidbody2D rb;
    JugadorConfig jugador;
    float movimientoHorizontal;
    bool enSuelo;
    Vector3 posicionInicial;

    bool invulnerable;
    float tiempoInvulnerable;          // segundos que le quedan de invulnerabilidad
    float tiempoParpadeo;              // segundos que faltan para el siguiente parpadeo
    const float intervaloParpadeo = 0.1f;

    float multiplicadorVelocidad = 1;
    float multiplicadorSalto = 1;
    float tiempoEfectoVelocidad;       // segundos que le quedan al efecto de velocidad
    float tiempoEfectoSalto;           // segundos que le quedan al efecto de salto

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (GameManager.Instance.EscenaActual == null)
        {
            GameManager.Instance.IniciarPartida();                       //gameManager temporal
            GameManager.Instance.IniciarEscena("Mina");                  //gameManager temporal
        }
        jugador = GameManager.Instance.Config.jugador;

        posicionInicial = transform.position;
    }

    void OnDisable()
    {
        if (sprite != null) sprite.enabled = true;
        invulnerable = false;
        tiempoInvulnerable = 0;
    }

    void Update()
    {
        LeerInput();
        DetectarSuelo();
        ActualizarAnimator();
        ActualizarInvulnerabilidad();
        ActualizarEfectos();

        if (Input.GetKeyDown(teclaInteractuar))
            DetectarInteraccion();
    }

    void FixedUpdate()
    {
        float velocidad = jugador.velocidad * multiplicadorVelocidad;
        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidad, rb.linearVelocity.y);
    }

    void LeerInput()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        if (movimientoHorizontal != 0)
        {
            if (sprite != null)
                sprite.flipX = movimientoHorizontal < 0;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (enSuelo)
            {
                float fuerza = jugador.fuerzaSalto * multiplicadorSalto;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerza);
            }
        }
    }

    void DetectarSuelo()
    {
        enSuelo = Physics2D.Raycast(origenSuelo.position, Vector2.down, distanciaSuelo, capaSuelo);
    }

    void DetectarInteraccion()
    {
        Vector2 direccion = Vector2.right;

        if (sprite != null)
        {
            if (sprite.flipX)
                direccion = Vector2.left;
        }

        RaycastHit2D hit = Physics2D.Raycast(origenFrontal.position, direccion, distanciaFrontal, capaInteractuable);

        if (hit.collider == null) return;

        if (hit.collider.TryGetComponent(out IInteractable interactuable))
            interactuable.Interactuar();
    }

    void ActualizarAnimator()
    {
        if (animator == null) return;
        float velocidadX = movimientoHorizontal;
        if (velocidadX < 0) velocidadX = -velocidadX;

        animator.SetFloat("VelocidadX", velocidadX);
        animator.SetBool("EnSuelo", enSuelo);
    }


    public void RecibirGolpe(string causa, int dano)
    {
        if (invulnerable) return;

        if (animator != null) animator.SetTrigger("Herido");

        bool murio = GameManager.Instance.RegistrarGolpe(causa, dano);

        if (murio)
        {
            Respawn();
        }
        else
        {
            invulnerable = true;
            tiempoInvulnerable = jugador.invulnerabilidad;
            tiempoParpadeo = 0;
        }
    }

    public void CaerAlVacio()
    {
        GameManager.Instance.RegistrarGolpe("caida", 1);
        Respawn();
    }

    void Respawn()
    {
        if (GameManager.Instance.TieneCheckpoint(out Vector3 posicion))
            transform.position = posicion;
        else
            transform.position = posicionInicial;

        rb.linearVelocity = Vector2.zero;
    }

    void ActualizarInvulnerabilidad()
    {
        if (!invulnerable) return;

        tiempoInvulnerable -= Time.deltaTime;
        tiempoParpadeo -= Time.deltaTime;

        if (tiempoParpadeo <= 0)
        {
            if (sprite != null) sprite.enabled = !sprite.enabled;
            tiempoParpadeo = intervaloParpadeo;
        }

        if (tiempoInvulnerable <= 0)
        {
            invulnerable = false;
            if (sprite != null) sprite.enabled = true;
        }
    }


    public void AplicarEfecto(RecursoConfig recurso)
    {
        if (recurso.duracion <= 0) return;

        switch (recurso.efecto)
        {
            case "velocidad":
                multiplicadorVelocidad = recurso.valor;
                tiempoEfectoVelocidad = recurso.duracion;
                break;
            case "salto":
                multiplicadorSalto = recurso.valor;
                tiempoEfectoSalto = recurso.duracion;
                break;
        }
    }

    void ActualizarEfectos()
    {
        if (tiempoEfectoVelocidad > 0)
        {
            tiempoEfectoVelocidad -= Time.deltaTime;
            if (tiempoEfectoVelocidad <= 0) multiplicadorVelocidad = 1;
        }

        if (tiempoEfectoSalto > 0)
        {
            tiempoEfectoSalto -= Time.deltaTime;
            if (tiempoEfectoSalto <= 0) multiplicadorSalto = 1;
        }
    }
}
